using UnityEngine;
using System.Collections.Generic;

public class Consts : MonoBehaviour
{
    public static Dictionary<BuildingType, BuildingEffects> buildingEffectsDatabase = new Dictionary<BuildingType, BuildingEffects>()
    {
        { BuildingType.TownHall, new BuildingEffects { cost = 1000, maintenance = 10, housing = 0, jobs = 5, education = 0, safety = 0, enjoyment = 0 } },
        { BuildingType.House, new BuildingEffects { cost = 200, maintenance = 2, housing = 4, jobs = 0, education = 0, safety = 0, enjoyment = 0 } },
    };
    public static Dictionary<BuildingType, PrimaryCategory> buildingCategoryDatabase = new Dictionary<BuildingType, PrimaryCategory>()
    {
        { BuildingType.TownHall, PrimaryCategory.TownHall },
        { BuildingType.House, PrimaryCategory.Housing },
    };
}