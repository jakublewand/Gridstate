using UnityEngine;

public class City : MonoBehaviour
    //god object for game progress
{
    private int SECONDS_PER_DAY = 20; // what should it be???
    public static City instance; //singleton to reference manager everywhere

    [SerializeField] private GameState _gameState;
    public GameState gameState => _gameState; //not decided if to make private: gamestate has only values for now

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
        if (_gameState.income == 0)
            _gameState.income = 1000;
        if (_gameState.balance == 0)
            _gameState.balance = 100;
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
            gameState.dayProgress += (time / SECONDS_PER_DAY) * 100;
            if (100 <= gameState.dayProgress) //dayprogress full -> new day
            { 
                newDay();
            }
        }
    }

    public void newDay()
    {
        gameState.dayProgress = 0;
        gameState.dayCount++;
        gameState.balance += gameState.income;
    }

    public enum StatType
    {
        Education,
        Safety,
        Enjoyment,
        Jobs,
        Population,
        Income,
        Balance

    }

    public void ModifyStat(StatType stat, int amount)
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
        }
    }

    public double GetStat(StatType stat)
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

    public string getCityName()
    { return gameState.cityName; }

    public int getDayCount()
    { return gameState.dayCount; }

    public void renameCity(string newName) { gameState.cityName = newName; }
    //more methods related to state...
}
