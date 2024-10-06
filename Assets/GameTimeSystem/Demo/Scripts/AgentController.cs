using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DenisAlipov.GameTime
{
    /// <summary>
    /// Simple wandering script
    /// </summary>
    public class AgentController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 3f;
        public float stayTime = 3f;
        
        [Header("Behavior")]
        public int hourSleep = 23;
        public int hourWakeUp = 6;

        [Header("References")]
        [SerializeField]
        private GameObject sleepIcon;

        private int lastActionHour;

        void Start()
        {
            sleepIcon.SetActive(false);
            StartCoroutine(MoveToNextPoint(Random.value));
            GameTime.AddOnTimeChanged(UpdateBehavior);
        }

        private IEnumerator MoveToNextPoint(float delay)
        {
            yield return new WaitForSeconds(delay);

            //take next random point
            float randomX = Random.Range(-10, 10);
            float randomZ = Random.Range(-10, 10);
            Vector3 targetPoint = new Vector3(randomX, 1, randomZ);

            while (Vector3.Distance(transform.position, targetPoint) > 0.1f) 
            {
                //if not arrived to target point - move towards it
                transform.position = Vector3.MoveTowards(transform.position, targetPoint, moveSpeed * Time.deltaTime);
                yield return null;
            }

            //when arrived to target point - start moving to another point with delay
            StartCoroutine(MoveToNextPoint(stayTime));
        }
        
        private void UpdateBehavior(GameDayTime newTime)
        {
            int newTimeHours = newTime.Hours;
            if (lastActionHour == newTimeHours)
            {
                return;
            }

            if (newTimeHours == hourSleep)
            {
                sleepIcon.SetActive(true);
                StopAllCoroutines();
                lastActionHour = newTimeHours;
                return;
            }
        
            if (newTimeHours == hourWakeUp)
            {
                sleepIcon.SetActive(false);
                StartCoroutine(MoveToNextPoint(Random.value));
                lastActionHour = newTimeHours;
                return;
            }
        }
    }
}
