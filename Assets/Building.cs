using UnityEngine;


public class Building : MonoBehaviour
{
    public BuildingDefinition definition;
    public Vector2 location;

    public void Initialize(BuildingDefinition definition, Vector2 location)
    {
        this.definition = definition;
        this.location = location;
    }
}
