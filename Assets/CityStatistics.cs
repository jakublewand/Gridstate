using UnityEngine;

public class CityStatistics : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*
    void Start()
    {
        
    }
    */

    //created by other stats
    [SerializeField] private int money;
    [SerializeField] private int population;

    //creator stats
    [SerializeField] private int qualityofLife;
    [SerializeField] private int education;
    [SerializeField] private int safety;
    [SerializeField] private int energy;
    [SerializeField] private int jobs;

    //information stats
    public float totalTime;
    public string cityName;


    void Update()
    {
        totalTime += Time.deltaTime;
    }

    public enum StatType
    {
        QualityOfLife,
        Education,
        Safety,
        Energy,
        Jobs,
        Population
    }

    public void ModifyStat(StatType stat, int amount)
    {
        switch (stat)
        {
            case StatType.QualityOfLife: qualityofLife += amount; break;
            case StatType.Education: education += amount; break;
            case StatType.Safety: safety += amount; break;
            case StatType.Energy: energy += amount; break;
            case StatType.Jobs: jobs += amount; break;
            case StatType.Population: population += amount; break;
        }
    }

    public int GetStat(StatType stat)
    {
        return stat switch
        {
            StatType.QualityOfLife => qualityofLife,
            StatType.Education => education,
            StatType.Safety => safety,
            StatType.Energy => energy,
            StatType.Jobs => jobs,
            StatType.Population => population,
            _ => 0
        };
    }

}