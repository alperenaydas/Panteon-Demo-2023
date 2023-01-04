using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoldierStats", menuName = "ScriptableObjects/SoldierStats", order = 1)]
public class SoldiersStats : ScriptableObject
{
    [SerializeField] private List<SoldierStats> Soldiers;

    public SoldierStats GetStats(object soldierName)
    {
        foreach (var soldierStats in Soldiers)
        {
            if (soldierStats.SoldierName.Equals(soldierName))
            {
                return soldierStats;
            }
        }

        return null;
    }
}