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
    public float cost;
    public float maintenance;
    public float housing;
    public float jobs;
    public float education;
    public float safety;
    public float enjoyment;
}

[CreateAssetMenu(menuName = "Game/Buildings/Building Definition")]
public class BuildingDefinition : ScriptableObject
{
    public string displayName;
    public PrimaryCategory primaryCategory;
    public BuildingEffects effects;
    public GameObject prefab;
}
