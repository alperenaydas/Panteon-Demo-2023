using System.Collections.Generic;
using UnityEngine;

public class Barracks : Building
{
    public List<Spawnable> Spawnables;
    public Transform SpawnPoint;

    public bool SpawnPointSet { get; set; }

    protected override void OnMouseDown()
    {
        if (GameManager.Instance.CurrentState == GameState.Idle && !InputManager.Instance.MouseOverUI)
        {
            base.OnMouseDown();
            EventManager.SelectedBuildingForSpawning.Invoke(this);
        }
    }
}