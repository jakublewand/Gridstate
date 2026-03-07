using System.Collections;
using UnityEngine;

public class RandomEffectEvents
{
    private int randomEvent;

    public void update() //should be triggered once per payout
    {
        if(UnityEngine.Random.Range(0, 10) == 0 && !GameUIController.IsEventPopupActive)
        {
            createEvent();
        }
    }

    private void createEvent()
    {
        int randomEvent = UnityEngine.Random.Range(0, 7);
        string message = "";
        switch (randomEvent)
        {
            case 0:
                message = "Obama visited town! Nothing happens.";
                break;
            case 1:
                message = "People become more productive!  +Money";
                City.instance.ModifyStat(City.StatType.Balance, (City.instance.GetStat(City.StatType.Balance))*1.5f);
                break;
            case 2:
                message = "Neighboring circuses pass through!  +Enjoyment";
                City.instance.ModifyStat(City.StatType.Enjoyment, (City.instance.GetStat(City.StatType.Enjoyment))*1.2f);
                break;
            case 3:
                message = "People apply to Hustler University in masses!  +Education";
                City.instance.ModifyStat(City.StatType.Education, (City.instance.GetStat(City.StatType.Education))*1.2f);
                break;
            case 4:
                message = "You start handing out guns to newborns!  +Safety";
                City.instance.ModifyStat(City.StatType.Safety, (City.instance.GetStat(City.StatType.Safety))*1.2f);
                break;
            case 5:
                message = "Private investors kill the wildlife to make room for factories!  +Jobs";
                City.instance.ModifyStat(City.StatType.Jobs, (City.instance.GetStat(City.StatType.Jobs))*1.2f);
                break;
            case 6:
                message = "Your neighbor stalks you when you shower. Nothing happens.";
                break;
        }
        
        if (!string.IsNullOrEmpty(message))
        {
            ShowEventPopup(message);
        }
    }

    private void ShowEventPopup(string message)
    {
        // This will be implemented in GameUIController
        GameUIController.instance?.ShowEventPopup(message);
    }
}
