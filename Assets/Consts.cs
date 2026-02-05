using UnityEngine;
using System.Collections.Generic;

public class Consts : MonoBehaviour
{
    public static List<BuildingType> buildingTypes = new List<BuildingType>() { BuildingType.TownHall, BuildingType.Apartament, BuildingType.House};
    public static Dictionary<BuildingType, BuildingEffects> buildingEffectsDatabase = new Dictionary<BuildingType, BuildingEffects>()
    {
        { BuildingType.TownHall, new BuildingEffects { cost = 0, maintenance = 0, housing = 0, jobs = 5, education = 0, safety = 0, enjoyment = 0 } },
        { BuildingType.Apartament, new BuildingEffects { cost = 200, maintenance = 2, housing = 4, jobs = 0, education = 0, safety = 0, enjoyment = 0 } },
        { BuildingType.House, new BuildingEffects { cost = 100, maintenance = 2, housing = 4, jobs = 0, education = 0, safety = 0, enjoyment = 0 } },
    };
    public static Dictionary<BuildingType, PrimaryCategory> buildingCategoryDatabase = new Dictionary<BuildingType, PrimaryCategory>()
    {
        { BuildingType.TownHall, PrimaryCategory.TownHall },
        { BuildingType.Apartament, PrimaryCategory.Housing },
        { BuildingType.House, PrimaryCategory.Housing },
    };
    public static Dictionary<BuildingType, string> buildingNameDatabase = new Dictionary<BuildingType, string>()
    {
        { BuildingType.TownHall, "Town Hall" },
        { BuildingType.Apartament, "Apartament" },
        { BuildingType.House, "House" },
    };
}