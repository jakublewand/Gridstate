using Unity.Collections;
using UnityEngine;

public class StatisticCalculation
{
    public int quality;
     public int overshot;
    public int newIncome;
    private int Enjoyment;
    private int Education;
    private int Safety;
    private int Jobs;
    private int Population;
    public City city;

    public StatisticCalculation(City city)
    {
        this.city = city;
    }

    public void Recalculate()
    {
        CalcStats();
        CalcQuality();
        CalcOvershot();
        CalcIncome();
        city.ModifyStat(City.StatType.Income, newIncome);
    }

    private void CalcStats()
    {
        // get stats every tick
        Enjoyment = (int)city.GetStat(City.StatType.Enjoyment);
        Education = (int)city.GetStat(City.StatType.Education);
        Safety = (int)city.GetStat(City.StatType.Safety);
        Jobs = (int)city.GetStat(City.StatType.Jobs);  
        Population = (int)city.GetStat(City.StatType.Population); 
    }

    private void CalcQuality()
    {
        quality = 1+(Enjoyment + Education + Safety + Jobs)/4;
    }

    private void CalcOvershot()
    {
        overshot = Mathf.Max(Enjoyment, Education, Safety, Jobs);

        overshot = overshot - quality; // should be 0 if balanced perfectly

        if (overshot <= 0)
        {
            overshot = 1;
        }
    }

    private void CalcIncome()
    {
        float safeOvershot = Mathf.Max(overshot, 1f);
        newIncome = (int)(Population * quality * 5) / Mathf.Max((int)Mathf.Log(safeOvershot), 1);
        if (newIncome <= 0)
        {
            newIncome = 10;
        }
        // income = pop*quality / (log(overshot))

        /*  say 1000 population
            100 education
            40 jobs
            50 safety
            10 enjoyment
            quality =(100+40+50+10)/4 = 50
            overshot = 100-50 = 50
            income = (1000*50)/(log(50)) = 29 430
        */

    }

}
