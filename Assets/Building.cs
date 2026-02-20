using UnityEngine;


public class Building : MonoBehaviour
{
    public PrimaryCategory primaryCategory;
    public BuildingType buildingType;
    public string name;
    public Vector3 location;
    public BuildingEffects effects;
    public GameObject gameObject;

    public Building(BuildingType type, Vector3 location)
    public BuildingDefinition definition;
    public Vector2 location;

    public void Initialize(BuildingDefinition definition, Vector2 location)
    {
        this.definition = definition;
        this.location = location;
    }
}
