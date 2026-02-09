using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

public class BulidingManager : MonoBehaviour
{
    [SerializeField] City city;
    [SerializeField] public List<BuildingDefinition> buildingDefinitions = new List<BuildingDefinition>();
    List<Building> buildings = new List<Building>();
    public BuildingDefinition selectedBuilding;
    private BuildingDefinition lastSelectedBuilding;
    public GameObject selectedBuildingObject;
    private Ray ray;
    private RaycastHit hit;
    public Collider planeCollider;
    private Vector2 mouseDownLocation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (City.instance != null)
        {
            city = City.instance;
        }
    }

    void Start()
    {
        if (city == null)
        {
            city = City.instance;
        }

        if (buildingDefinitions.Count == 0 || buildingDefinitions[0] == null || buildingDefinitions[0].prefab == null)
        {
            Debug.LogWarning("No default building definition/prefab set on BulidingManager.");
            return;
        }

        Build(
            buildingDefinitions[0],
            new Vector3(0, buildingDefinitions[0].prefab.transform.localScale.y / 2, 0)
        );
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedBuilding != null) {
            if (selectedBuildingObject == null) {
                selectedBuildingObject = Instantiate(selectedBuilding.prefab, Vector3.zero, Quaternion.identity);
            }
            if (lastSelectedBuilding != selectedBuilding) {
                Destroy(selectedBuildingObject);
                selectedBuildingObject = Instantiate(selectedBuilding.prefab, Vector3.zero, Quaternion.identity);
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
                if (planeCollider.Raycast(ray, out hit, Mathf.Infinity))
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
                        selectedBuilding = null;
                    }   
                }
            }
        }
    }


    public void Build(BuildingDefinition buildingDefinition, Vector3 position)
    {
        if (buildingDefinition == null || buildingDefinition.prefab == null)
            return;

        if (buildingDefinition.effects.cost > city.GetStat(City.StatType.Balance))
            return;

        Vector2 gridPos = new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        GameObject buildingObject = Instantiate(buildingDefinition.prefab, position, Quaternion.identity);
        Building building = buildingObject.GetComponent<Building>();

        if (building == null)
        {
            Debug.LogWarning($"Instantiated prefab '{buildingDefinition.prefab.name}' has no Building component.");
            Destroy(buildingObject);
            return;
        }

        city.ModifyStat(City.StatType.Balance, -buildingDefinition.effects.cost);
        building.Initialize(buildingDefinition, gridPos);
        buildings.Add(building);


        city.SetStat(City.StatType.Education, 0d);
        city.SetStat(City.StatType.Safety, 0d);
        city.SetStat(City.StatType.Enjoyment, 0d);
        city.SetStat(City.StatType.Jobs, 0d);
        city.ModifyStat(City.StatType.Population, buildingDefinition.effects.housing);

        foreach (Building builtBuilding in buildings)
        {
            double population = Math.Max(city.GetStat(City.StatType.Population), 1d);

            city.ModifyStat(City.StatType.Education,
            (1d - city.GetStat(City.StatType.Education) / population) * builtBuilding.definition.effects.education);
            city.ModifyStat(City.StatType.Safety,
            (1d - city.GetStat(City.StatType.Safety) / population) * builtBuilding.definition.effects.safety);
            city.ModifyStat(City.StatType.Enjoyment,
            (1d - city.GetStat(City.StatType.Enjoyment) / population) * builtBuilding.definition.effects.enjoyment);
            city.ModifyStat(City.StatType.Jobs,
            (1d - city.GetStat(City.StatType.Jobs) / population) * builtBuilding.definition.effects.jobs);
        }
    }
}
