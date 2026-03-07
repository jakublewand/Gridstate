using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BulidingManager : MonoBehaviour
{
    [SerializeField] City city;
    [SerializeField] public List<BuildingDefinition> buildingDefinitions = new List<BuildingDefinition>();
    [SerializeField] AudioScript audioScript;
    [SerializeField] AudioSource uiSounds;
    [SerializeField] GameUIController ui;
    public List<Building> buildings = new List<Building>();
    public List<RandomEventData> randomEvents = new List<RandomEventData>();
    public BuildingDefinition selectedBuilding;
    private BuildingDefinition lastSelectedBuilding;
    public GameObject selectedBuildingObject;
    private Ray ray;
    private RaycastHit hit;
    public Collider planeCollider;
    private Vector2 mouseDownLocation;
    private Vector2 mouseUpLocation;
    public PlaneGenerator PG;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (buildingDefinitions.Count == 0 || buildingDefinitions[0] == null || buildingDefinitions[0].prefab == null)
        {
            Debug.LogWarning("No default building definition/prefab set on BulidingManager.");
            return;
        }

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownLocation = Input.mousePosition;
        } else if(Input.GetMouseButtonUp(0)) {mouseUpLocation = Input.mousePosition;}

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

            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("nmousebutton UP on BM");
                if (IsPointerOverUI())
                {
                    Debug.Log("if1 nmousebutton UP on BM");
                    return;
                }
                if (Vector3.Distance(mouseDownLocation, Input.mousePosition) > 5f && !EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log("if 2nmousebutton UP on BM");
                    return;
                }

                Vector3 mousePos = Input.mousePosition;
                ray = Camera.main.ScreenPointToRay(mousePos);
                if (planeCollider.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Debug.Log("inside buildingloop");
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
        } else if (Input.GetMouseButtonUp(0) && Vector3.Distance(mouseDownLocation, Input.mousePosition) <= 5f)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (planeCollider.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Vector3 alignedPos = hit.point - new Vector3(-0.5f, 0f, -0.5f);
                alignedPos.x = Mathf.Floor(alignedPos.x);
                alignedPos.z = Mathf.Floor(alignedPos.z);
                bool found = false;
                foreach (var building in buildings)
                    {
                        if (building.gameObject.transform.position.x == alignedPos.x &&
                            building.gameObject.transform.position.z == alignedPos.z && building.definition.primaryCategory!=PrimaryCategory.TownHall)
                        {
                            ui.ShowDetails(building.gameObject);
                            found = true;
                            break;
                        }
                    }
                if (!found) ui.CloseDetails();
            }
        }
    }


    public void Build(BuildingDefinition buildingDefinition, Vector3 position)
    {
        if (buildingDefinition == null || buildingDefinition.prefab == null)
            return;

        if (buildingDefinition.effects.cost > city.GetStat(City.StatType.Balance))
        {
            AnnouncementManager.instance.msgAnnounce(AnnounceColor.Red, "Can't afford this building!");
            return;
        }

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
        city.ModifyStat(City.StatType.Population, buildingDefinition.effects.housing);
        building.Initialize(buildingDefinition, gridPos);
        buildings.Add(building);
        PG.UpdatePlane();
        RecalculateStats();
        audioScript.PlaySound(audioScript.build);
    }
    public void PlaceTownHall(int ch)
    {
        var townHalls = buildings.FindAll(b => b.definition.primaryCategory == PrimaryCategory.TownHall);
        foreach (var building in townHalls)
        {
            Demolish(building);
        }
        Build(
            buildingDefinitions[ch],
            new Vector3(0, buildingDefinitions[ch].prefab.transform.localScale.y / 2, 0)
        );
    }

    public void Demolish(Building building)
    {
        if (building.definition == null || building.definition.prefab == null)
            return;
        city.ModifyStat(City.StatType.Population, -building.definition.effects.housing);
        Destroy(building.gameObject);
        buildings.Remove(building);
        PG.UpdatePlane();
        RecalculateStats();
        audioScript.PlaySound(audioScript.demolish);
        
    }
    
    private bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    public void RecalculateStats()
    {
        float k = 1.386f;
        city.SetStat(City.StatType.Jobs, 0);
        city.SetStat(City.StatType.Education, 0);
        city.SetStat(City.StatType.Enjoyment, 0);
        city.SetStat(City.StatType.Safety, 0);
        float jobsTotal = 0;
        float educationTotal = 0;
        float enjoymentTotal = 0;
        float safetyTotal = 0;
        float population = city.GetStat(City.StatType.Population);

        foreach (var building in buildings)
        {
            jobsTotal += building.definition.effects.jobs;
            educationTotal += building.definition.effects.education;
            enjoymentTotal += building.definition.effects.enjoyment;
            safetyTotal += building.definition.effects.safety;
        }


        city.SetStat(City.StatType.Jobs, 1f - Mathf.Exp(-k * jobsTotal / population));
        city.SetStat(City.StatType.Education, 1f - Mathf.Exp(-k * educationTotal / population));
        city.SetStat(City.StatType.Enjoyment, 1f - Mathf.Exp(-k * enjoymentTotal / population));
        city.SetStat(City.StatType.Safety, 1f - Mathf.Exp(-k * safetyTotal / population));
        foreach (var randomEvent in randomEvents)
        {
            city.ModifyStat(randomEvent.affectedStat, randomEvent.effectAmount);
        }
    }
}
