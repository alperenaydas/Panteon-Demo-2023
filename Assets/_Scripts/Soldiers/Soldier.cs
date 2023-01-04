using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Soldier : Spawnable, IHaveHealth, ICanDamage
{
    public int HealthPoints { get; set; }
    public int DamagePoints { get; set; }
    
    public float AttackRate { get; set; }
    
    public void TakeDamage(int damageVal)
    {
        HealthPoints -= damageVal;
        if (HealthPoints <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(TakeDamageFlashSprite());
    }

    public void InflictDamage(int damageVal, IHaveHealth target)
    {
        target.TakeDamage(damageVal);
    }

    public Tile OnTile { get; set; }

    private List<Tile> m_Path;
    private bool m_IsMoving;

    private bool m_Attacking;

    private void OnEnable()
    {
        var stats = GameManager.Instance.SoldiersStats.GetStats(ObjectName); //ObjectName only serialized before initialization on soldiers.
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        HealthPoints = stats.HealthPoints;
        DamagePoints = stats.DamagePoints;
        AttackRate = stats.AttackRate;
        SpriteRenderer.sprite = stats.SoldierSprite;
    }

    public void Move(Tile toGoTile)
    {
        if (m_IsMoving) return;
        m_Path = GridManager.Instance.FindPath(OnTile.x, OnTile.y, toGoTile.x, toGoTile.y);
        if (m_Path == null) return;
        m_IsMoving = true;
        StartCoroutine(StartMoving());
    }

    IEnumerator StartMoving()
    {
        OnTile.SetEmpty(true);
        OnTile = m_Path[m_Path.Count-1];
        OnTile.SetEmpty(false);
        var cellOffset = GridManager.Instance.CellSize / 2f;
        while (m_Path.Count > 0)
        {
            var moveDuration = 0.25f;
            for (float t = 0; t < moveDuration; t += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(transform.position, m_Path[0].transform.position - new Vector3(cellOffset, cellOffset, 0f), t/moveDuration);
                yield return null;
            }
            m_Path.RemoveAt(0);
            yield return null;
        }
        
        m_IsMoving = false;
    }
    
    protected override void OnMouseDown()
    {
        if (GameManager.Instance.CurrentState == GameState.Idle && !InputManager.Instance.MouseOverUI)
        {
            base.OnMouseDown();
            EventManager.SelectedSoldierForInformation.Invoke(this);
        }
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

    public void Attack(Vector3 attackingPos, IHaveHealth attackingUnit)
    {
        var movingTile = GridManager.Instance.GetClosestTile(attackingPos);
        Move(movingTile);
        m_Attacking = true;
        StartCoroutine(StartAttacking(attackingUnit));
    }

    IEnumerator StartAttacking(IHaveHealth attackingUnit)
    {
        while (m_IsMoving)
        {
            yield return null;
        }
        while (attackingUnit as Object && m_Attacking && !m_IsMoving)
        {
            InflictDamage(DamagePoints, attackingUnit);
            if (attackingUnit.HealthPoints <= 0)
            {
                m_Attacking = false;
                yield break;
            }
            yield return new WaitForSeconds(AttackRate);
        }
    }

    public void StopAttacking()
    {
        m_Attacking = false;
    }

    public void Die()
    {
        OnTile.SetEmpty(true);
        PoolManager.Instance.CoolObject(gameObject, PoolObjectType);
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
