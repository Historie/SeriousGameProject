using System;
using UnityEngine;

namespace RTSBuildingSystem
{
    public class Building : MonoBehaviour
    {
        public float cost;
        public float production; // Amount of currency produced per tick
        public float plotSize = 1; // (Relative to the grid size) - if the plot fits in one grid square, the size should be one
        [HideInInspector] public BoxCollider coreCollider;
        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public Animator animator;
        [HideInInspector] public GameObject GFX;
        [HideInInspector] public GameObject overlay;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            coreCollider = GetComponent<BoxCollider>();
            coreCollider.enabled = false;

            // Find graphics and overlay gameobject
            GFX = transform.GetChild(0).gameObject;
            overlay = transform.GetChild(1).gameObject;

            meshRenderer = GFX.GetComponent<MeshRenderer>();
            GFX.SetActive(false);
        }

        public void CollectCurrency()
        {
            if (production <= 0) return;

            // Play effect
            animator.SetTrigger("Interact");

            // Display money made
            PlacementManager.instance.DisplayIncome(transform.position, production);

            // Add money
            PlacementManager.instance.money += production;
            PlacementManager.instance.moneyText.text = PlacementManager.instance.money.ToString();
        }
    }
}