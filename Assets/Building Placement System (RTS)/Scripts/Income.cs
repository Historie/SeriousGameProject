using UnityEngine;
using UnityEngine.UI;

namespace RTSBuildingSystem
{ 

    public class Income : MonoBehaviour
    {
        public float amount;
        public float lifetime = 5;
        public float speed;

        private Text amountText;
        private Transform cam;
        private CanvasGroup group;

        void Start()
        {
            cam = Camera.main.transform;
            amountText = GetComponentInChildren<Text>();
            group = amountText.GetComponent<CanvasGroup>();
            amountText.color = amount < 0 ? Color.red : Color.green;
            amountText.text = amount < 0 ? "" : "+";
            amountText.text += amount.ToString();
            amountText.text += "$";
            Destroy(gameObject, lifetime);
        }

        void FixedUpdate()
        {
            // Float up and look at the camera
            transform.Translate(Vector3.up * speed * Time.fixedDeltaTime);
            transform.LookAt(cam);
            lifetime -= Time.fixedDeltaTime;
            group.alpha = Mathf.Clamp01(lifetime);
        }
    }
}

