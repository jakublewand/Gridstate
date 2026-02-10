using UnityEngine;
using System;

public enum PrimaryCategory
{
    TownHall,
    Housing,
    Jobs,
    Education,
    Safety,
    Enjoyment
}

[Serializable]
public struct BuildingEffects
{
    public double cost;
    public double maintenance;
    public double housing;
    public double jobs;
    public double education;
    public double safety;
    public double enjoyment;
}

[CreateAssetMenu(menuName = "Game/Buildings/Building Definition")]
public class BuildingDefinition : ScriptableObject
{
    public string displayName;
    public PrimaryCategory primaryCategory;
    public BuildingEffects effects;
    public GameObject prefab;
}
