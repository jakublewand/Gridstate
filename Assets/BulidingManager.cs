using UnityEngine;
using System.Collections.Generic;

public class BulidingManager : MonoBehaviour
{
    [SerializeField] GameObject bulidingPrefab;
    List<Building> buildings = new List<Building>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Build(BuildingType.TownHall, new Vector2(0, 0));
        Build(BuildingType.House, new Vector2(2, 0));
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Building building in buildings)
        {
        }
    }


    public void Build(BuildingType buildingType, Vector2 location)
    {
        Building building = new Building(buildingType, location);
        GameObject buildingObject = Instantiate(bulidingPrefab, new Vector3(location.x, location.y, 0), Quaternion.identity);
        if (buildingType != BuildingType.TownHall)
        {
            buildingObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        }
        building.gameObject = buildingObject;
        buildings.Add(building);
    }
}
