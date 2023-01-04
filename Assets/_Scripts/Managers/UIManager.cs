using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    #region Singleton

    private static UIManager m_Instance;

    public static UIManager Instance => m_Instance;

    private void Awake()
    {
        m_Instance = this;
    }

    #endregion

    [SerializeField] private InformationTab m_InformationTab;
    [SerializeField] private SpawnableTab m_SpawnableTab;
    
    public bool MouseOverUI => IsPointerOverUI();

    private void Start()
    {
        EventManager.SelectedBuildingForInformation.AddListener(OpenBuildingInformationTab);
        EventManager.SelectedSoldierForInformation.AddListener(OpenSoldierInformationTab);
        EventManager.SelectedBuildingForSpawning.AddListener(OpenBuildingSpawnTab);
    }

    public void SelectBuilding(string buildingName)
    {
        EventManager.SelectedBuildingForProduction.Invoke(buildingName);
        GameManager.Instance.StartBuilding(buildingName);
    }

    private void OpenBuildingInformationTab(string buildingName)
    {
        CloseInformationTab();
        var stats = GameManager.Instance.BuildingsStats.GetStats(buildingName);
        m_InformationTab.SetObjectInformation(stats.BuildingName, stats.BuildingSprite, stats.BuildingInfo);
    }

    private void OpenBuildingSpawnTab(Barracks spawningBarracks)
    {
        var spawnables = spawningBarracks.Spawnables;
        m_SpawnableTab.SetSpawnableInformation(spawnables, spawningBarracks);
    }
    
    private void OpenSoldierInformationTab(Soldier soldier)
    {
        CloseInformationTab();
        var stats = GameManager.Instance.SoldiersStats.GetStats(soldier.ObjectName);
        m_InformationTab.SetObjectInformation(stats.SoldierName, stats.SoldierSprite, stats.SoldierInfo);
    }

    public void CloseInformationTab()
    {
        m_InformationTab.CloseInformationTab();
        m_SpawnableTab.CloseSpawnableTab();
    }
    
    private bool IsPointerOverUI() {
        if (EventSystem.current.IsPointerOverGameObject()) {
            return true;
        } 
        PointerEventData pe = new PointerEventData(EventSystem.current);
        pe.position = Input.mousePosition;
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pe, hits);
        return hits.Count > 0;
    }
}