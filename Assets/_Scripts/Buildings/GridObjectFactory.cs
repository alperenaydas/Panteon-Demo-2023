using UnityEngine;

public static class GridObjectFactory
{
    public static GridObject GetGridObject(string buildingName)
    {
        switch (buildingName)
        {
            case Constants.BarracksBuildingName:
                Debug.Log("Barracks selected");
                return GameManager.Instance.BuildingsStats.GetStats(Constants.BarracksBuildingName).BuildingPrefab;
            case Constants.TownCenterBuildingName:
                Debug.Log("TownCenter selected");
                return GameManager.Instance.BuildingsStats.GetStats(Constants.TownCenterBuildingName).BuildingPrefab;
            case Constants.PowerPlantBuildingName:
                Debug.Log("PowerPlant selected");
                return GameManager.Instance.BuildingsStats.GetStats(Constants.PowerPlantBuildingName).BuildingPrefab;
            case Constants.LevelOneSoldierName:
                Debug.Log("LevelOneSoldier selected");
                return GameManager.Instance.SoldiersStats.GetStats(Constants.LevelOneSoldierName).SoldierPrefab;
            case Constants.LevelTwoSoldierName:
                Debug.Log("LevelTwoSoldier selected");
                return GameManager.Instance.SoldiersStats.GetStats(Constants.LevelTwoSoldierName).SoldierPrefab;
            case Constants.LevelThreeSoldierName:
                Debug.Log("LevelThreeSoldier selected");
                return GameManager.Instance.SoldiersStats.GetStats(Constants.LevelThreeSoldierName).SoldierPrefab;
            default:
                return null;
        }
    }
}