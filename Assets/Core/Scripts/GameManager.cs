using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public GameState gameState { get; private set; }

    // Awake is called when loading the script, good to ensure existing data like gamestate
    void Awake()    
    {
        if (instance != Null)
        {
            Destroy(gameObject);
            return
            // destroy?
        }
        instance = this;
        DontDestroyOnLoad(gameObject)
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState = new GameState();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
