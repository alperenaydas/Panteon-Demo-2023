using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingStats
{
    public string BuildingName;
    public int HealthPoints;
    public int BuildingXSize;
    public int BuildingYSize;
    
    [TextArea]
    public string BuildingInfo;

    public Sprite BuildingSprite;
    public Building BuildingPrefab;
}
