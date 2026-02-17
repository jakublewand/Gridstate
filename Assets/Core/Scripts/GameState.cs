using UnityEngine;
[System.Serializable]
public class GameState
{
    //Simple gamestate with important fields (should even help with saving correctly later)
    //I decided on no methods (handled through gameManager) and clean state for SRP
    public string cityName;
    public int dayCount;
    public bool paused;
    public float dayProgress;
    public float payoutProgress;
    public float population;
    public float income;
    public float balance;
    // Stats:
    public float jobs;
    public float education;
    public float enjoyment;
    public float safety;


}
