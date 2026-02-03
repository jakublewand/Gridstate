using UnityEngine;

public class GameManager : MonoBehaviour
    //god object for game progress
{
    private int SECONDS_PER_DAY = 20; // what should it be???
    public static GameManager instance; //singleton to reference manager everywhere

    public GameState gameState { get; private set; } //not decided if to make private: gamestate has only values for now

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
        //create gamestate with starting values
        gameState = new GameState
        {
            cityName = "City Name",
            dayCount = 0,
            paused = false,
            dayProgress = 0,
            population = 0,
            income = 1000,
            balance = 100,
        };
        
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


    //Communications with the gamestate, should only be done through here in other files as of now
    public void playPauseGame()
    { gameState.paused = !gameState.paused; }

    public bool isPaused()
    { return gameState.paused; }

    public float getDayProgress()
    { return gameState.dayProgress; }


    public int getBalance()
    { return gameState.balance; }

    public string getCityName()
    { return gameState.cityName; }

    public int getDayCount()
    { return gameState.dayCount; }

    public int getPopulation()
    { return gameState.population; }

    public int getIncome()
    { return gameState.income; }

    public void renameCity(string newName) { gameState.cityName = newName; }
    //more methods related to state...
}
