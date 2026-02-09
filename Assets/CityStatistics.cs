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
    [SerializeField] private double money;
    [SerializeField] private double population;

    //creator stats
    [SerializeField] private double qualityofLife;
    [SerializeField] private double education;
    [SerializeField] private double safety;
    [SerializeField] private double energy;
    [SerializeField] private double jobs;

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

    public void ModifyStat(StatType stat, double amount)
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

    public double GetStat(StatType stat)
    {
        return stat switch
        {
            StatType.QualityOfLife => qualityofLife,
            StatType.Education => education,
            StatType.Safety => safety,
            StatType.Energy => energy,
            StatType.Jobs => jobs,
            StatType.Population => population,
            _ => 0d
        };
    }

}
