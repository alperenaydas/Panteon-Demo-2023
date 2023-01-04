using System.Collections.Generic;
using UnityEngine;

public class Barracks : Building
{
    public List<Spawnable> Spawnables;
    public Transform SpawnPoint;

    public bool SpawnPointSet { get; set; }
    protected override void Start()
    {
        var stats = GameManager.Instance.BuildingsStats.GetStats(Constants.BarracksBuildingName);
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        ObjectName = stats.BuildingName;
        HealthPoints = stats.HealthPoints;
        SpriteRenderer.sprite = stats.BuildingSprite;
    }

    protected override void OnMouseDown()
    {
        if (GameManager.Instance.CurrentState == GameState.Idle && !InputManager.Instance.MouseOverUI)
        {
            base.OnMouseDown();
            EventManager.SelectedBuildingForSpawning.Invoke(this);
        }
    }
}