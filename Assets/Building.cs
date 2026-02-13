using UnityEngine;

public enum PrimaryCategory
{
    TownHall,
    Housing,
    Jobs,
    Education,
    Safety,
    Enjoyment
}

public enum BuildingType
{
    None,
    TownHall,
    Apartament,
    House,
}

public struct BuildingEffects
{
    public double cost;
    public double maintenance;
    public double housing;
    public double jobs;
    public double education;
    public double safety;
    public double enjoyment;
}

public class Building : MonoBehaviour
{
    public PrimaryCategory primaryCategory;
    public BuildingType buildingType;
    public string name;
    public Vector3 location;
    public BuildingEffects effects;
    public GameObject gameObject;

    public Building(BuildingType type, Vector3 location)
    {
        this.buildingType = type;
        this.name = Consts.buildingNameDatabase[type];
        this.primaryCategory = Consts.buildingCategoryDatabase[type];
        this.effects = Consts.buildingEffectsDatabase[type];
        this.location = location;
        this.gameObject = null;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
