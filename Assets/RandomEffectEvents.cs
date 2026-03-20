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
    private int currentChance = 0;

    public RandomEffectEvents(BulidingManager manager)
    {
        buildingManager = manager;
    }

    public void update() //should be triggered once per payout
    {
        Debug.Log("Current chance: " + currentChance);
        currentChance += 20;
        if (UnityEngine.Random.Range(0, 1000) < currentChance && !GameUIController.IsEventPopupActive)
        {
            currentChance = 0;
            createEvent();
        }
    }

    private void createEvent()
    {
        int randomEvent = UnityEngine.Random.Range(0, 20);
        string message = "";
        string title = "";
        switch (randomEvent)
        {
            case 0:
                message = "VIPs visited town!";
                title = "Local News!";
                break;
            case 1:
                message = "Rich bankers enjoy your city!  +Money";
                title = "Finance Update!";
                City.instance.SetStat(City.StatType.Balance, City.instance.GetStat(City.StatType.Balance)*2f);
                break;
            case 2:
                message = "Neighboring circuses pass through!  +Enjoyment";
                title = "Joyful News!";
                buildingManager.randomEvents.Add(new RandomEventData(City.StatType.Enjoyment, 1.25f));
                buildingManager.RecalculateStats();
                break;
            case 3:
                message = "People apply to Hustler University in masses!  +Education";
                title = "Scholarly Research!";
                buildingManager.randomEvents.Add(new RandomEventData(City.StatType.Education, 1.25f));
                buildingManager.RecalculateStats();
                break;
            case 4:
                message = "You beat a drug kingpin in a dance battle!  +Safety";
                title = "Safety News!";
                buildingManager.randomEvents.Add(new RandomEventData(City.StatType.Safety, 1.25f));
                buildingManager.RecalculateStats();
                break;
            case 5:
                message = "Private investors kill the wildlife to make room for factories!  +Jobs";
                title = "Employment Alert!";
                buildingManager.randomEvents.Add(new RandomEventData(City.StatType.Jobs, 1.25f));
                buildingManager.RecalculateStats();
                break;
            case 6:
                message = "Nuclear winter approaches...";
                title = "Local News!";
                break;
            case 7:
                message = "Taxes increase, you buy private yachts!  +Money";
                title = "Finance Update!";
                City.instance.SetStat(City.StatType.Balance, City.instance.GetStat(City.StatType.Balance)*1.5f);
                break;
            case 8:
                message = "Fights break out at the local pubs!  +Enjoyment";
                title = "Joyful News!";
                City.instance.SetStat(City.StatType.Enjoyment, City.instance.GetStat(City.StatType.Enjoyment)*1.5f);
                buildingManager.RecalculateStats();
                break;
            case 9:
                message = "Your F students become inventors!  +Education";
                title = "Scholarly Research!";
                buildingManager.randomEvents.Add(new RandomEventData(City.StatType.Education, 1.5f));
                buildingManager.RecalculateStats();
                break;
            case 10:
                message = "You banned IRL streamers!  +Safety";
                title = "Safety News!";
                buildingManager.randomEvents.Add(new RandomEventData(City.StatType.Safety, 1.25f));
                buildingManager.RecalculateStats();
                break;
            case 11:
                message = "The mayor hires a bodyguard for their bodyguard!  +Jobs";
                title = "Employment Alert!";
                buildingManager.randomEvents.Add(new RandomEventData(City.StatType.Jobs, 1.25f));
                buildingManager.RecalculateStats();
                break;
            case 12:
                message = "The president disguises themself as a supervillain and robs people on the street!  +Money";
                title = "Finance Update!";
                City.instance.SetStat(City.StatType.Balance, City.instance.GetStat(City.StatType.Balance)*1.5f);
                break;
            case 13:
                message = "Meteors of gold hit your city!  +++Money";
                title = "Finance Update!";
                City.instance.SetStat(City.StatType.Balance, City.instance.GetStat(City.StatType.Balance)*5f);
                break;
            case 14:
                message = "The player clicked on a golden cookie!  +++Money";
                title = "Finance Update!";
                City.instance.SetStat(City.StatType.Balance, City.instance.GetStat(City.StatType.Balance)*5f);
                break;
            case 15:
                message = "The city is hit by the biggest tsunami to have ever existed.";
                title = "Local News!";
                break;
            case 16:
                message = "States critical of your city attempt to nuke it, passing birds absorb the impact.  +Safety ";
                title = "Safety News!";
                buildingManager.randomEvents.Add(new RandomEventData(City.StatType.Safety, 1.25f));
                buildingManager.RecalculateStats();
                break;
            case 17:
                message = "The devs doubled your money!  +Money";
                title = "Finance Update!";
                City.instance.SetStat(City.StatType.Balance, City.instance.GetStat(City.StatType.Balance)*2f);
                break;
            case 18:
                message = "The city's accountant misplaced a decimal point!  +++++Money";
                title = "Finance Update!";
                City.instance.SetStat(City.StatType.Balance, City.instance.GetStat(City.StatType.Balance)*10f);
                break;
            case 19:
                message = "Someone fell off the grid :(";
                title = "Local News!";
                break;
        }

        if (!string.IsNullOrEmpty(message))
        {
            GameUIController.instance?.ShowEventPopup(message,title);
        }
    }
}
