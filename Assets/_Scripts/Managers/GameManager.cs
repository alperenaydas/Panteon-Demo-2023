using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager m_Instance;

    public static GameManager Instance => m_Instance;

    private void Awake()
    {
        m_Instance = this;
    }

    #endregion

    private GameState m_CurrentState;

    public GameState CurrentState
    {
        get
        {
            return m_CurrentState;
        }
        set
        {
            m_CurrentState = value;
            switch (m_CurrentState)
            {
                case GameState.Idle:
                    break;
                case GameState.Building:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public BuildingsStats BuildingsStats;
    public SoldiersStats SoldiersStats;

    private void Start()
    {
        m_CurrentState = GameState.Idle;
        EventManager.SelectedBuildingForProduction.AddListener(StartBuilding);
        EventManager.ProductionBuildingCompleted.AddListener(EndBuilding);
    }

    private void OnDestroy()
    {
        EventManager.SelectedBuildingForProduction.RemoveListener(StartBuilding);
        EventManager.ProductionBuildingCompleted.RemoveListener(EndBuilding);
    }

    public void StartBuilding(string buildingName)
    {
        CurrentState = GameState.Building;
    }

    public void EndBuilding()
    {
        CurrentState = GameState.Idle;
    }
}

public enum GameState
{
    Idle,
    Building,
}
