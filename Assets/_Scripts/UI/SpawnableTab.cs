using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnableTab : MonoBehaviour
{
    [SerializeField] private GameObject m_SpawnableParentObject;
    
    [SerializeField] private TMP_Text m_SelectedSpawnableNameText;
    [SerializeField] private Image m_SelectedSpawnableImage;

    private List<Spawnable> m_Spawnables;
    private Barracks m_SpawnableBuilding;
    
    private int m_LastUsedIndex;

    public void SetSpawnableInformation(List<Spawnable> spawnables, Barracks spawnableBuilding)
    {
        m_SpawnableBuilding = spawnableBuilding;
        m_Spawnables = spawnables;
        m_LastUsedIndex = 0;
        ChangeSpawnableType(0);
        m_SpawnableParentObject.SetActive(true);
    }

    public void CloseSpawnableTab()
    {
        if(m_SpawnableParentObject.activeSelf) m_SpawnableParentObject.SetActive(false);
    }

    public void ChangeSpawnableType(int spawnableIndex)
    {
        m_LastUsedIndex+=spawnableIndex;
        if (m_LastUsedIndex < 0) m_LastUsedIndex += m_Spawnables.Count;
        if (m_LastUsedIndex >= m_Spawnables.Count) m_LastUsedIndex = 0;
        m_SelectedSpawnableNameText.text = m_Spawnables[m_LastUsedIndex].ObjectName;
        m_SelectedSpawnableImage.sprite = m_Spawnables[m_LastUsedIndex].SpawnableSprite;
    }

    public void SpawnSpawnable()
    {
        var spawnPointPos = m_SpawnableBuilding.SpawnPoint.position;
        var spawnTile = GridManager.Instance.GetClosestTile(m_SpawnableBuilding.SpriteRenderer.transform.position); //spriterenderer position is center.
        var toGoTile = GridManager.Instance.GetClosestTile(spawnPointPos);

        if (!spawnTile || !spawnTile.TileEmpty) return;
        
        //we instantiate as soldier because there is no other spawnable type, if there was, we would check and then move.
        var newSolider = Instantiate(m_Spawnables[m_LastUsedIndex] as Soldier, spawnTile.transform.position - new Vector3(0.5f, 0.5f, 0f), Quaternion.identity);
        newSolider.OnTile = spawnTile;

        if (!toGoTile || !toGoTile.TileEmpty) return;
        newSolider.Move(toGoTile);
        toGoTile.SetEmpty(false);
    }
}