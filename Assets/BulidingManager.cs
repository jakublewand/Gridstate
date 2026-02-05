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
    public GameObject selectedBuildingObject;
    private Ray ray;
    private RaycastHit hit;
    public Collider planeCollider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    buildingPrefabs = new Dictionary<BuildingType, GameObject>() {
        { BuildingType.None, null },
        { BuildingType.TownHall, townHallPrefab },

        { BuildingType.House, housePrefab },
    };
        Build(BuildingType.TownHall, new Vector2(0, 0));
        Build(BuildingType.House, new Vector2(2, 0));
        selectedBuilding = BuildingType.House;
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedBuilding != BuildingType.None) {
            if (selectedBuildingObject == null) {
                selectedBuildingObject = Instantiate(buildingPrefabs[selectedBuilding], Vector3.zero, Quaternion.identity);
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (planeCollider.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Vector3 alignedPos = hit.point;
                alignedPos.x = Mathf.Floor(alignedPos.x);
                alignedPos.z = Mathf.Floor(alignedPos.z);
                selectedBuildingObject.transform.position = alignedPos;
                selectedBuildingObject.transform.position += Vector3.up * selectedBuildingObject.transform.localScale.y / 2;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                Debug.Log(EventSystem.current.IsPointerOverGameObject());
                    return; // Ignore click – pointer is over UI
                }
                Vector3 mousePos = Input.mousePosition;
                ray = Camera.main.ScreenPointToRay(mousePos);
                if (Physics.Raycast(ray, out hit))
                {
                    Build(selectedBuilding, selectedBuildingObject.transform.position);
                    Destroy(selectedBuildingObject);
                    selectedBuilding = BuildingType.None;
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
