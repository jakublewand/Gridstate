using System;
using System.Collections.Generic;
using UnityEngine;

public static class AnnouncementMessages
{
    public static string GetRandom()
    {
        var all = GetAll();
        var valid = new List<string>();
        foreach (var (message, condition) in all)
        {
            if (condition())
            {
                valid.Add(message);
            }
        }
        if (valid.Count > 0)
        {
            return valid[UnityEngine.Random.Range(0, valid.Count)];
        }
        return "";
    }

//weird wrapping method for evaluating and casting lambda into  expression
    static (string, Func<bool>) Msg(string text, Func<bool> condition) => (text, condition);

    private static List<(string message, Func<bool> condition)> GetAll()
    {
        var city = City.instance;

        return new List<(string, Func<bool>)>
        {
            //guide for making new message: 
            //1. wrap in Msg(), 
            //2. write message as value 1. 
            // 3. write condition as lambda based on gamestate (or other) stats in valeu 2
            Msg("This is the worst city I have ever seen...", () => 500 > city.GetStat(City.StatType.Income)),
            Msg("Cool city... For a 5 year old", () => 500 <= city.GetStat(City.StatType.Income) && city.GetStat(City.StatType.Income) < 1000),
            Msg("Getting somewhere", () => 1000 <= city.GetStat(City.StatType.Income) && city.GetStat(City.StatType.Income) < 10000),
            Msg("Pretty cool city!", () => 10000 <= city.GetStat(City.StatType.Income) && city.GetStat(City.StatType.Income) < 100000),
            Msg("Coolest city I've ever seen!!!", () => 100000 < city.GetStat(City.StatType.Income)),
        };
    }
}
