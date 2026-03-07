using UnityEngine;

public class rotatePlanet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public float rotationsPerMinute = -10f;     
    void Update()
    {
        transform.Rotate(2f * Time.deltaTime, 6f * rotationsPerMinute * Time.deltaTime, 0f);
    }
}
