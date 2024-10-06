using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RTSBuildingSystem
{
    public class BuildingCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject buildingPrefab;
        public Text costText;

        private bool mouseOver;
        private bool selected;
        Vector3 clickScale = new Vector2(1.2f, 1.2f);
        Vector3 enterScale = new Vector2(1.1f, 1.1f);
        Vector3 exitScale = new Vector2(1f, 1f);
        private Coroutine runningCoroutine;

        void Start()
        {
            if (buildingPrefab == null) return;

            costText.text = buildingPrefab.GetComponent<Building>().cost.ToString();
        }

        void SmartCoroutine(IEnumerator newCoroutine) // Only one coroutine can run at a time
        {
            if (runningCoroutine != null) StopCoroutine(runningCoroutine);

            runningCoroutine = StartCoroutine(newCoroutine);
        }

        IEnumerator Click()
        {
            if (PlacementManager.instance.currentCard != null) PlacementManager.instance.currentCard.Select(false);
            Select(true);
            PlacementManager.instance.currentCard = this;
            PlacementManager.instance.SelectBuilding();

            YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();

            while (transform.localScale.x < clickScale.x - .03f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, clickScale, .5f);
                yield return waitForFixedUpdate;
            }
            transform.localScale = clickScale;
            runningCoroutine = null;
        }

        IEnumerator Enter()
        {
            YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();

            while (transform.localScale.x < enterScale.x - .02f && mouseOver)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, enterScale, .5f);
                yield return waitForFixedUpdate;
            }
            transform.localScale = enterScale;
            runningCoroutine = null;
        }

        IEnumerator Exit()
        {
            YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();

            while (transform.localScale.x > exitScale.x + .02f && !mouseOver)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, exitScale, .5f);
                yield return waitForFixedUpdate;
            }
            transform.localScale = exitScale;
            runningCoroutine = null;
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            // If card is already selected or gets right clicked, return
            if (selected || eventData.pointerId == -2) return;

            SmartCoroutine(Click());
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (selected) return;

            mouseOver = true;
            transform.localScale = exitScale;
            SmartCoroutine(Enter());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (selected) return;

            mouseOver = false;
            transform.localScale = enterScale;
            selected = false;
            SmartCoroutine(Exit());
        }

        public void Select(bool select)
        {
            if (select)
            {
                selected = true;

                // Destroy previous building blueprint
                Destroy(PlacementManager.instance.currentBuilding);
                PlacementManager.instance.currentBuilding = null;
            }
            else
            {
                selected = false;
                mouseOver = false;
                SmartCoroutine(Exit());
            }
        }
    }
}