using UnityEngine;

public class LevelThreeSoldier : Soldier
{
    private void Start()
    {
        var stats = GameManager.Instance.SoldiersStats.GetStats(Constants.LevelThreeSoldierName);
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        ObjectName = stats.SoldierName;
        HealthPoints = stats.HealthPoints;
        DamagePoints = stats.DamagePoints;
        SpriteRenderer.sprite = stats.SoldierSprite;
    }
}