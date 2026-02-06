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
        quality = (Enjoyment + Education + Safety + Jobs)/4;
    }

    private void CalcOvershot()
    {
        overshot = Enjoyment;
        if (Education > overshot)
        {
            overshot = Education;
        }
        else if (Safety > overshot)
        {
            overshot = Safety;
        }
        else if (Jobs > overshot)
        {
            overshot = Jobs;
        }

        overshot = overshot - quality; // should be 0 if balanced perfectly

        if (overshot <= 0)
        {
            overshot = 1;
        }
    }

    private void CalcIncome()
    {
        double multiplier = Mathf.Log(Mathf.Max((float)Population / overshot, 1f));
        newIncome = (int)(Population*multiplier);
    }

}
