using UnityEngine;
using System;

public class StatisticCalculation
{
    [SerializeField] private double growthFactor = 0.1d;
    public double quality;
    public double overshot;
    public double newIncome;
    private double Enjoyment;
    private double Education;
    private double Safety;
    private double Jobs;
    private double Population;
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
        city.SetStat(City.StatType.Income, newIncome);
    }

    private void CalcStats()
    {
        // get stats every tick
        Enjoyment = city.GetStat(City.StatType.Enjoyment);
        Education = city.GetStat(City.StatType.Education);
        Safety = city.GetStat(City.StatType.Safety);
        Jobs = city.GetStat(City.StatType.Jobs);
        Population = city.GetStat(City.StatType.Population);
    }

    private void CalcQuality()
    {
        quality = 1d + (Enjoyment + Education + Safety + Jobs) / 4d;
    }

    private void CalcOvershot()
    {
        overshot = Math.Max(Math.Max(Enjoyment, Education), Math.Max(Safety, Jobs));

        overshot = overshot - quality; // should be 0 if balanced perfectly

        if (overshot <= 0)
        {
            overshot = 1;
        }
    }

    private void CalcIncome()
    {
        double population = city.GetStat(City.StatType.Population);
        double education = city.GetStat(City.StatType.Education);
        double safety = city.GetStat(City.StatType.Safety);
        double enjoyment = city.GetStat(City.StatType.Enjoyment);
        double jobs = city.GetStat(City.StatType.Jobs);

        double summedSquares = education * education
                             + safety * safety
                             + enjoyment * enjoyment
                             + jobs * jobs;

        newIncome = growthFactor * population * summedSquares / 4d;

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
