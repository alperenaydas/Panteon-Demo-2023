using UnityEngine;

public class LevelTwoSoldier : Soldier
{
    private void Start()
    {
        var stats = GameManager.Instance.SoldiersStats.GetStats(Constants.LevelTwoSoldierName);
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        ObjectName = stats.SoldierName;
        HealthPoints = stats.HealthPoints;
        DamagePoints = stats.DamagePoints;
        SpriteRenderer.sprite = stats.SoldierSprite;
    }
}