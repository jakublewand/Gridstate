using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform Sun;
    [SerializeField] private Transform Moon;

    private float deg;
    private City city;

    void Start()
    {
        city = City.instance;
    }

    // Update is called once per frame
    void Update()
    {
        deg = 360f * (city.getDayProgress()/100f);
        Sun.transform.rotation = Quaternion.Euler(deg, 30f, 0);
        Moon.transform.rotation = Quaternion.Euler(180f + deg, 30f, 0);
    }
}
