using UnityEngine;

public class RandomEventData
{
    public City.StatType affectedStat;
    public float effectAmount;

    public RandomEventData(City.StatType stat, float amount)
    {
        affectedStat = stat;
        effectAmount = amount;
    }
}

public class RandomEffectEvents
{
    private BulidingManager buildingManager;

    public RandomEffectEvents(BulidingManager manager)
    {
        buildingManager = manager;
    }

    public void update() //should be triggered once per payout
    {
        Debug.Log("Checking for random event...");
        if (UnityEngine.Random.Range(0, 1) == 0 && !GameUIController.IsEventPopupActive)
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
                buildingManager.randomEvents.Add(new RandomEventData(City.StatType.Balance, 1.5f));
                buildingManager.RecalculateStats();
                break;
            case 2:
                message = "Neighboring circuses pass through!  +Enjoyment";
                buildingManager.randomEvents.Add(new RandomEventData(City.StatType.Enjoyment, 1.2f));
                buildingManager.RecalculateStats();
                break;
            case 3:
                message = "People apply to Hustler University in masses!  +Education";
                buildingManager.randomEvents.Add(new RandomEventData(City.StatType.Education, 1.2f));
                buildingManager.RecalculateStats();
                break;
            case 4:
                message = "You start handing out guns to newborns!  +Safety";
                buildingManager.randomEvents.Add(new RandomEventData(City.StatType.Safety, 1.2f));
                buildingManager.RecalculateStats();
                break;
            case 5:
                message = "Private investors kill the wildlife to make room for factories!  +Jobs";
                buildingManager.randomEvents.Add(new RandomEventData(City.StatType.Jobs, 1.2f));
                buildingManager.RecalculateStats();
                break;
            case 6:
                message = "Your neighbor stalks you when you shower. Nothing happens.";
                break;
        }

        if (!string.IsNullOrEmpty(message))
        {
            GameUIController.instance?.ShowEventPopup(message);
        }
    }
}
