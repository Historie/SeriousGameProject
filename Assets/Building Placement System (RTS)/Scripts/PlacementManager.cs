using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RTSBuildingSystem
{
    public class PlacementManager : MonoBehaviour
    {
        public static PlacementManager instance;

        [Header("Currency")]
        public float money;
        public Text moneyText;
        public ParticleSystem buyEffect;
        public ParticleSystem placeEffect;
        public GameObject incomePrefab;

        [Header("Building")]
        public bool demolishMode;
        public BuildingCard currentCard;
        public GameObject currentBuilding;
        public LayerMask buildingMask;
        public LayerMask demolishMask;

        [Header("Grid")]
        public bool grid;
        public float size = 4;
        public int width = 50;
        public int height = 50;

        private float rot;
        private Camera cam;
        private Building buildingScript;
        private Vector3 prevPoint;
        private Building lastBuilding;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            cam = Camera.main;
            moneyText.text = money.ToString();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T)) grid = !grid;

            if (demolishMode)
            {
                // If right click - cancel demolish mode
                if (Input.GetMouseButtonDown(1))
                {
                    demolishMode = false;

                    // Disable Overlay of last selected building
                    if (lastBuilding != null)
                    {
                        lastBuilding.GFX.SetActive(true);
                        lastBuilding.overlay.SetActive(false);
                        lastBuilding = null;
                    }

                    currentCard.Select(false);
                    currentCard = null;
                    Destroy(currentBuilding);
                    return;
                }

                // Follow mouse
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1225, demolishMask))
                {
                    // If mouse raycast hits a building
                    if (hit.collider.gameObject.TryGetComponent(out Building building))
                    {
                        // Enable overlay
                        building.overlay.SetActive(true);
                        building.GFX.SetActive(false);

                        // Red Overlay
                        foreach (Material mat in building.overlay.GetComponent<MeshRenderer>().materials)
                            mat.color = new Color(1, 0, 0, .3f); // Red

                        // Temporary store current building so we can remove the blueprint material if the mouse gets off of it
                        if (lastBuilding != building)
                        {
                            // If last building isn't null, disable overlay
                            if (lastBuilding != null)
                            {
                                lastBuilding.GFX.SetActive(true);
                                lastBuilding.overlay.SetActive(false);
                            }

                            lastBuilding = building;
                        }

                        // If left button is clicked and over a building - demolish it
                        if (Input.GetMouseButtonDown(0))
                        {
                            // Return if mouse clicked over UI element
                            if (EventSystem.current.IsPointerOverGameObject()) return;

                            Demolish(building);
                        }
                    }
                    else // If hitting something without building prefab
                    {
                        if (lastBuilding != null && buildingScript != null)
                        {
                            // Disable Overlay
                            lastBuilding.GFX.SetActive(true);
                            lastBuilding.overlay.SetActive(false);
                            lastBuilding = null;
                        }
                    }
                }
                else // If not hitting anything
                {
                    if (lastBuilding != null)
                    {
                        // Disable Overlay
                        lastBuilding.GFX.SetActive(true);
                        lastBuilding.overlay.SetActive(false);
                        lastBuilding = null;
                    }
                }

                return;
            }

            if (currentBuilding != null)
            {
                // If right click - cancel placing a building
                if (Input.GetMouseButtonDown(1))
                {
                    currentCard.Select(false);
                    currentCard = null;
                    Destroy(currentBuilding);
                }

                // Follow mouse
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1225, buildingMask))
                {
                    // Snap to grid
                    Vector3 buildingPosition = grid ? GetNearestGridPoint(hit.point) : hit.point;
                    buildingPosition += Vector3.up * .1f;
                    currentBuilding.transform.position = buildingPosition;

                    // Update blueprint material only when building moves in grid
                    if (prevPoint != currentBuilding.transform.position)
                    {
                        prevPoint = currentBuilding.transform.position;
                    }
                    CheckIntersect();

                    // Place
                    if (Input.GetMouseButtonDown(0) && money >= buildingScript.cost)
                    {
                        // Return if mouse clicked over UI element
                        if (EventSystem.current.IsPointerOverGameObject()) return;

                        // Make sure it doesn't intersect with another building
                        if (!Physics.CheckBox(currentBuilding.transform.position, buildingScript.coreCollider.size / 2,
                            currentBuilding.transform.rotation, ~buildingMask))
                            PlaceBuilding(currentBuilding.transform.position);
                    }
                }

                // Rotate with R
                if (Input.GetButtonDown("RotateProp"))
                {
                    rot += 90;
                    currentBuilding.transform.Rotate(Vector3.up, 90);
                }
            }
        }

        public void SelectBuilding()
        {
            demolishMode = false;

            // If building isn't instantiated, spawn it
            if (currentBuilding == null)
            {
                // If card has building prefab - spawn that building
                if (currentCard.buildingPrefab != null) currentBuilding = Instantiate(currentCard.buildingPrefab, cam.ScreenToWorldPoint(Input.mousePosition), Quaternion.Euler(Vector3.up * rot));
                else // If card doesn't have building prefab - assume it's demolish card
                {
                    demolishMode = true;
                    return;
                }
            }

            buildingScript = currentBuilding.GetComponent<Building>();
            CheckIntersect();
        }

        public void CheckIntersect()
        {
            // Make sure it doesn't intersect with another building
            if (!Physics.CheckBox(currentBuilding.transform.position, buildingScript.coreCollider.size / 2,
                currentBuilding.transform.rotation, ~buildingMask))
            {
                // Green Overlay
                foreach (Material mat in buildingScript.overlay.GetComponent<MeshRenderer>().materials)
                    mat.color = new Color(0, 1, 0, .3f); // Green
            }
            else
            {
                // Red Overlay
                foreach (Material mat in buildingScript.overlay.GetComponent<MeshRenderer>().materials)
                    mat.color = new Color(1, 0, 0, .3f); // Red
            }
        }

        public void Demolish(Building building)
        {
            // Remove from buildings
            GameManager.instance.buildings.Remove(building);

            // Get back 50% of the cost
            money += building.cost / 2;
            moneyText.text = money.ToString();

            // Disable Overlay
            lastBuilding.GFX.SetActive(true);
            lastBuilding.overlay.SetActive(false);

            // Parent plot to GFX
            Transform plot = lastBuilding.transform.Find("Plot");
            if (plot != null) plot.SetParent(lastBuilding.GFX.transform);

            // Play Sell Effect
            buyEffect.transform.position = building.transform.position;
            buyEffect.transform.localScale = Vector3.one * building.plotSize;
            buyEffect.Play();

            // Play Demolish Effect
            placeEffect.transform.position = building.transform.position;
            placeEffect.transform.localScale = Vector3.one * building.plotSize;
            placeEffect.Play();

            // Play Animation
            building.animator.SetTrigger("Demolish");

            // Display income
            DisplayIncome(building.transform.position, building.cost / 2);

            // Destroy building gameobject
            Destroy(building.gameObject, .5f);

            // Destroy script
            Destroy(building);
        }

        public void DisplayIncome(Vector3 position, float amount)
        {
            GameObject income = Instantiate(incomePrefab, position + Vector3.up * 14, Quaternion.identity);
            income.GetComponent<Income>().amount = amount;
        }

        public void PlaceBuilding(Vector3 raw)
        {
            Vector3 buildingPosition = grid ? GetNearestGridPoint(raw) : raw;

            // Place it slightly above ground
            buildingPosition += Vector3.up * .1f;

            GameObject building = Instantiate(currentBuilding, buildingPosition, Quaternion.Euler(Vector3.up * rot));

            // Update material and reset color
            Building newBuilding = building.GetComponent<Building>();
            newBuilding.coreCollider.enabled = true;

            // Enable Overlay
            newBuilding.GFX.SetActive(true);
            newBuilding.overlay.SetActive(false);

            // Subtract cost
            money -= newBuilding.cost;
            moneyText.text = money.ToString();

            // Add to buildings list
            GameManager.instance.buildings.Add(newBuilding);

            // Update next building
            SelectBuilding();

            // Play animation
            newBuilding.animator.SetTrigger("Interact");

            // Play Buy Effect
            buyEffect.transform.position = buildingPosition;
            buyEffect.transform.localScale = Vector3.one * newBuilding.plotSize;
            buyEffect.Play();

            // Display income
            DisplayIncome(building.transform.position, -newBuilding.cost);

            // Play Demolish Effect
            placeEffect.transform.position = building.transform.position;
            placeEffect.transform.localScale = Vector3.one * newBuilding.plotSize;
            placeEffect.Play();
        }

        public Vector3 GetNearestGridPoint(Vector3 raw)
        {
            raw -= transform.position;

            int x = Mathf.RoundToInt(raw.x / size);
            int z = Mathf.RoundToInt(raw.z / size);
            Vector3 point = new Vector3((float)x * size, raw.y, (float)z * size);
            point += transform.position;

            return point;
        }

        private void OnDrawGizmos()
        {
            // Testing Gizmos - Can be used for Debugging

            //if (currentBuilding != null)
            //{
            //    Gizmos.DrawWireCube(currentBuilding.transform.position, buildingScript.coreCollider.size / 2);
            //}

            //Gizmos.color = Color.blue;
            //if (grid && currentBuilding != null)
            //{
            //    for (int i = -width / 2; i < width / 2; i++)
            //    {
            //        for (int j = -height / 2; j < height / 2; j++)
            //        {
            //            Gizmos.DrawWireCube(new Vector3(i * size, 2, j * size), new Vector3(size, size / 2, size));
            //        }
            //    }
            //}
        }
    }
}