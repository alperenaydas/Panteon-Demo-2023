using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingStats", menuName = "ScriptableObjects/BuildingStats", order = 1)]
public class BuildingsStats : ScriptableObject
{
    [SerializeField] private List<BuildingStats> Buildings;

    public BuildingStats GetStats(object buildingName)
    {
        foreach (var buildingStats in Buildings)
        {
            if (buildingStats.BuildingName.Equals(buildingName))
            {
                return buildingStats;
            }
        }

        return null;
    }
}
