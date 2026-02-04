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
    public int population;
    public int income;
    public int balance;
    // Stats:
    public double jobs;
    public double education;
    public double enjoyment;
    public double safety;


}
