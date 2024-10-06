using System.Collections.Generic;
using UnityEngine;

namespace RTSBuildingSystem
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public float collectTime;

        public List<Building> buildings;

        private float baseCollectTime;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            baseCollectTime = collectTime;
        }

        void Update()
        {
            if (collectTime > 0) collectTime -= Time.deltaTime;
            else
            {
                CollectCurrency();
                collectTime = baseCollectTime;
            }
        }

        public void CollectCurrency()
        {
            foreach (Building building in buildings)
                building.CollectCurrency();
        }
    }
}