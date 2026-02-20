using UnityEngine;


public class Building : MonoBehaviour
{
    public BuildingDefinition definition;
    public Vector2 location;

    [SerializeField] GameObject refCube; //1x1 modeling cube aid
    [SerializeField] bool ShowRefCube = false;

    public void Initialize(BuildingDefinition definition, Vector2 location)
    {
        this.definition = definition;
        this.location = location;
    }

    private void OnValidate()
    {
        if(refCube!=null){refCube.SetActive(ShowRefCube);}
    }
}
