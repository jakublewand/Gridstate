using UnityEngine;

public class City : MonoBehaviour
    //god object for game progress
{
    [SerializeField] private int SECONDS_PER_DAY = 20; // what should it be???
    [SerializeField] private int PAYOUTS_PER_DAY = 4; // what should it be???
    public static City instance; //singleton to reference manager everywhere

    [SerializeField] private GameState _gameState;
    public GameState gameState => _gameState; //not decided if to make private: gamestate has only values for now

    private float temp;
    private float product;

    // Awake is called when loading the script, good to ensure existing data like gamestate
    void Awake()    
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
            // destroy?
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // set some defaults for good standard sity values
        if (string.IsNullOrEmpty(_gameState.cityName))
            _gameState.cityName = "City Name";
        if (_gameState.balance == 0)
            _gameState.balance = 100;
        if (_gameState.topBalance == 0)
            _gameState.topBalance = _gameState.balance;
    }

    // Update is called once per frame
    void Update()
    {
        //game updates here (random events etc)
        tickDay(Time.deltaTime);
    }

    void tickDay(float time)
    {
        if (!gameState.paused)
        {
            RecalculateIncome(); //calculates your income each frame
            announcementHints();

            gameState.dayProgress += (time / SECONDS_PER_DAY) * 100;
            if (100 <= gameState.dayProgress) //dayprogress full -> new day
            { 
                newDay();
            }

            gameState.payoutProgress += ((time * PAYOUTS_PER_DAY)/ SECONDS_PER_DAY) * 100;
            if (100 <= gameState.payoutProgress ) //payoutproress full -> new payout
            { 
                newPayout();
            }
        }
    }

    public void newDay()
    {
        gameState.dayProgress = 0;
        gameState.dayCount++;
    }

    public void newPayout()
    {
        gameState.payoutProgress = 0;
        temp = gameState.balance;
        gameState.balance += gameState.income; 
        if(gameState.balance <= 0)
        {
            gameState.balance = temp;
        }
        if(gameState.balance > gameState.topBalance) {gameState.topBalance = gameState.balance;}

    }

    public enum Characters {king = 1, idol = 2, engineer = 3}

    public void SetCharacter(Characters ch)
    {
        gameState.character=(int)ch;
    }
    public Characters GetCharacter()
    {
        return (Characters)gameState.character;
    }

    public enum StatType
    {
        Education,
        Safety,
        Enjoyment,
        Jobs,
        Population,
        Income,
        Balance,
        TopBalance

    }

    public void SetStat(StatType stat, float amount)
    {
        switch (stat)
        {
            case StatType.Enjoyment: gameState.enjoyment = amount; break;
            case StatType.Education: gameState.education = amount; break;
            case StatType.Safety: gameState.safety = amount; break;
            case StatType.Jobs: gameState.jobs = amount; break;
            case StatType.Population: gameState.population = amount; break;
            case StatType.Income: gameState.income = amount; break;
            case StatType.Balance: gameState.balance = amount; break;
            case StatType.TopBalance: gameState.topBalance = amount; break;
        }
    }
    public void ModifyStat(StatType stat, float amount)
    {
        switch (stat)
        {
            case StatType.Enjoyment: gameState.enjoyment += amount; break;
            case StatType.Education: gameState.education += amount; break;
            case StatType.Safety: gameState.safety += amount; break;
            case StatType.Jobs: gameState.jobs += amount; break;
            case StatType.Population: gameState.population += amount; break;
            case StatType.Income: gameState.income += amount; break;
            case StatType.Balance: gameState.balance += amount; break;
            case StatType.TopBalance: gameState.topBalance += amount; break;
        }
    }

    public float GetStat(StatType stat)
    {
        switch (stat)
        {
            case StatType.Enjoyment: return gameState.enjoyment;
            case StatType.Education: return gameState.education;
            case StatType.Safety: return gameState.safety;
            case StatType.Jobs: return gameState.jobs;
            case StatType.Population: return gameState.population;
            case StatType.Income: return gameState.income;
            case StatType.Balance: return gameState.balance;
            case StatType.TopBalance: return gameState.topBalance;
            default: return 0;
        }
    }

    //Communications with the gamestate, should only be done through here in other files as of now
    public void playPauseGame()
    { gameState.paused = !gameState.paused; }

    public bool isPaused()
    { return gameState.paused; }

    public float getDayProgress()
    { return gameState.dayProgress; }

    public float getPayoutProgress()
    { return gameState.payoutProgress; }

    public string getCityName()
    { return gameState.cityName; }

    public int getDayCount()
    { return gameState.dayCount; }

    public void renameCity(string newName) { gameState.cityName = newName; }
    //more methods related to state...
    public void RecalculateIncome()
    {
        float k = 10f;
        product = gameState.jobs * gameState.education * gameState.enjoyment * gameState.safety;
        float quality = Mathf.Pow(product, 0.25f);
        gameState.income = quality * gameState.population * k;
    }

    public void announcementHints()
    {
        if (AnnouncementManager.instance == null)
        return;

        if (this.GetStat(City.StatType.Education) < 0.18 && this.GetStat(City.StatType.Income)>150)
        {
            AnnouncementManager.instance.msgAnnounce(
                AnnounceColor.Red,
                "Your city needs to fund more education!"
            );
        }
        else if(this.GetStat(City.StatType.Jobs) < 0.18 && this.GetStat(City.StatType.Income)>150)
        {
            AnnouncementManager.instance.msgAnnounce(
                AnnounceColor.Red,
                "Your population is unemployed! Create some job opportunities!"
            );
        }
        else if(this.GetStat(City.StatType.Enjoyment) < 0.18 && this.GetStat(City.StatType.Income)>150)
        {
            AnnouncementManager.instance.msgAnnounce(
                AnnounceColor.Red,
                "Your city is very depressing, maybe plant some trees!"
            );
        }
        else if(this.GetStat(City.StatType.Safety) < 0.18 && this.GetStat(City.StatType.Income)>150)
        {
            AnnouncementManager.instance.msgAnnounce(
                AnnounceColor.Red,
                "Your city isn't safe at all! Where is the police?"
            );
        }
    }
}
