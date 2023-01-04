using System;
using UnityEngine;

[Serializable]
public class SoldierStats
{
    public string SoldierName;
    public int HealthPoints;
    public int DamagePoints;
    public float AttackRate;
    public string SoldierInfo;

    public Sprite SoldierSprite;
    
    //change with soldier
    public Soldier SoldierPrefab;
}