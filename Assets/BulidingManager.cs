using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BulidingManager : MonoBehaviour
{
    [SerializeField] GameObject housePrefab;
    [SerializeField] GameObject townHallPrefab;
    public Dictionary<BuildingType, GameObject> buildingPrefabs = new Dictionary<BuildingType, GameObject>();
    List<Building> buildings = new List<Building>();
    public BuildingType selectedBuilding;
    private BuildingType lastSelectedBuilding;
    public GameObject selectedBuildingObject;
    private Ray ray;
    private RaycastHit hit;
    public Collider planeCollider;
    private Vector2 mouseDownLocation;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buildingPrefabs = new Dictionary<BuildingType, GameObject>() {
            { BuildingType.None, null },
            { BuildingType.TownHall, townHallPrefab },
            { BuildingType.House, housePrefab },
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedBuilding != BuildingType.None) {
            if (selectedBuildingObject == null) {
                selectedBuildingObject = Instantiate(buildingPrefabs[selectedBuilding], Vector3.zero, Quaternion.identity);
            }
            if (lastSelectedBuilding != selectedBuilding) {
                Destroy(selectedBuildingObject);
                selectedBuildingObject = Instantiate(buildingPrefabs[selectedBuilding], Vector3.zero, Quaternion.identity);
                lastSelectedBuilding = selectedBuilding;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (planeCollider.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Vector3 alignedPos = hit.point - new Vector3(-0.5f, 0f, -0.5f);
                alignedPos.x = Mathf.Floor(alignedPos.x);
                alignedPos.z = Mathf.Floor(alignedPos.z);
                selectedBuildingObject.transform.position = alignedPos;
                selectedBuildingObject.transform.position += Vector3.up * selectedBuildingObject.transform.localScale.y / 2;
            }

            if (Input.GetMouseButtonDown(0))
            {
                mouseDownLocation = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                if (Vector3.Distance(mouseDownLocation, Input.mousePosition) > 5f)
                {
                    return;
                }


                Vector3 mousePos = Input.mousePosition;
                ray = Camera.main.ScreenPointToRay(mousePos);
                if (Physics.Raycast(ray, out hit))
                {
                    bool occupied = false;
                    foreach (var building in buildings)
                    {
                        if (building.gameObject.transform.position.x == selectedBuildingObject.transform.position.x &&
                            building.gameObject.transform.position.z == selectedBuildingObject.transform.position.z)
                        {
                            occupied = true;
                            break;
                        }
                    }
                    if (!occupied) {
                        Build(selectedBuilding, selectedBuildingObject.transform.position);
                        Destroy(selectedBuildingObject);
                        selectedBuilding = BuildingType.None;
                    }   
                }
            }
        }
    }


    public void Build(BuildingType buildingType, Vector3 position)
    {
        Vector2 gridPos = new Vector2(0, 0);
        Building building = new Building(buildingType, gridPos);
        GameObject buildingObject = Instantiate(buildingPrefabs[buildingType], position, Quaternion.identity);
        building.gameObject = buildingObject;
        buildings.Add(building);
    }
}
