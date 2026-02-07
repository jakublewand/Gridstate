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
            // 3. write condition as lambda based on gamestate (or other) stats
            Msg("this is the worst city i have ever seen", () => true),
            Msg("pretty cool city!!!", () => 10000 < city.GetStat(City.StatType.Balance)),
            Msg("pretty cool city!!!!!!!", () => 100000 < city.GetStat(City.StatType.Balance)),
            Msg("pretty cool city!!!!!!!!!!!!!!!!!", () => 1000000 < city.GetStat(City.StatType.Balance)),
            Msg("pretty cool city!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", () => 10000000 < city.GetStat(City.StatType.Balance)),
        };
    }
}
