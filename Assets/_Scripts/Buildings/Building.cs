using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : GridObject, IHaveHealth
{
    public int HealthPoints { get; set; }

    public virtual void TakeDamage(int damageVal)
    {
        HealthPoints -= damageVal;
        if (HealthPoints <= 0)
        {
            Die();
            return;
        }
        
        StartCoroutine(TakeDamageFlashSprite());
    }
    
    protected override void OnMouseDown()
    {
        if (GameManager.Instance.CurrentState == GameState.Idle && !InputManager.Instance.MouseOverUI)
        {
            base.OnMouseDown();
            EventManager.SelectedBuildingForInformation.Invoke(ObjectName);
        }
    }
    
    protected virtual void Start()
    {
        var stats = GameManager.Instance.BuildingsStats.GetStats(ObjectName);
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        HealthPoints = stats.HealthPoints;
        SpriteRenderer.sprite = stats.BuildingSprite;
    }

    private void OnMouseOver()
    {
        if (GameManager.Instance.CurrentState == GameState.Idle && !InputManager.Instance.MouseOverUI)
        {
            if (Input.GetMouseButtonDown(1))
            {
                EventManager.ToBeAttackedObjectSelected.Invoke(transform.position, this);
            }
        }
    }

    public void Die()
    {
        var buildingStats = GameManager.Instance.BuildingsStats.GetStats(ObjectName);
        var transformPos = transform.position;
        for (int x = 0; x < buildingStats.BuildingXSize; x++)
        {
            for (int y = 0; y < buildingStats.BuildingYSize; y++)
            {
                var tile = GridManager.Instance.GetTile((int)transformPos.x + x, (int)transformPos.y + y);
                tile.SetEmpty(true);
            }
        }
        Destroy(this);
        Destroy(gameObject);
    }
    
    IEnumerator TakeDamageFlashSprite()
    {
        var col = SpriteRenderer.color;
        var defAlpha = col.a;
        col.a = 0f;
        SpriteRenderer.color = col;
        yield return new WaitForSeconds(0.1f);
        col.a = defAlpha;
        SpriteRenderer.color = col;
    }
}