using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Singleton

    private static InputManager m_Instance;

    public static InputManager Instance => m_Instance;

    private void Awake()
    {
        m_Instance = this;
    }

    #endregion

    private int m_BuildingXSize;
    private int m_BuildingYSize;

    [SerializeField] private SpriteRenderer m_BuildingSpriteRenderer;
    [SerializeField] private SpriteRenderer m_AvailableBGSpriteRenderer;

    private float m_CellOffset;

    private BuildingStats m_SelectedBuildingStats;

    private bool m_Dragging;
    private bool m_StartedDragging;
    private Vector3 m_CamDragDiff;
    private Vector3 m_CamOrigin;

    public bool MouseOverUI => UIManager.Instance.MouseOverUI;

    private bool m_SpawnerBuildingSelected;
    private Transform m_SpawnerBuildingsSpawnPoint;

    private bool m_SoldierSelected;
    private Soldier m_SelectedSoldier;

    private void Start()
    {
        m_CellOffset = GridManager.Instance.CellSize / 2f;
        EventManager.SelectedBuildingForProduction.AddListener(SelectBuilding);
        EventManager.SelectedBuildingForSpawning.AddListener(SpawnerBuildingSelected);
        EventManager.SelectedSoldierForInformation.AddListener(SoldierSelected);
        EventManager.ToBeAttackedObjectSelected.AddListener(AttackWithSoldier);
    }

    private void Update()
    {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (MouseOverUI) return;

        #region CameraControl

        if (Input.GetMouseButtonDown(0))
        {
            if (!m_StartedDragging)
            {
                m_StartedDragging = true;
                m_CamOrigin = mouseWorldPos;
            }
        }

        if (Input.GetMouseButton(0))
        {
            m_CamDragDiff = mouseWorldPos - Camera.main.transform.position;
        }

        if (m_StartedDragging)
        {
            if (!m_Dragging && Camera.main.transform.position != m_CamOrigin - m_CamDragDiff) m_Dragging = true;
            if (m_Dragging) Camera.main.transform.position = m_CamOrigin - m_CamDragDiff;
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize+1f, 5f, 10f);
            
        }
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize-1f, 5f, 10f);
        }

        //mouseUp check after building so no accidentally buildings created

        #endregion

        #region BuildingInputHandle

        if (GameManager.Instance.CurrentState == GameState.Building)
        {
            var intXPos = Mathf.RoundToInt(mouseWorldPos.x);
            var intYPos = Mathf.RoundToInt(mouseWorldPos.y);

            HighlightTiles(out var isTilesEmpty, intXPos, intYPos);


            if (Input.GetMouseButtonUp(0) && isTilesEmpty && !m_Dragging)
            {
                BuildBuilding(intXPos, intYPos);
            }

            if (Input.GetMouseButtonDown(1))
            {
                DeselectBuilding();
            }
        }

        #endregion
        
        #region IdleInputHandle

        if (GameManager.Instance.CurrentState == GameState.Idle)
        {
            if (Input.GetMouseButtonUp(0) && !m_Dragging)
            {
                var intXPos = Mathf.RoundToInt(mouseWorldPos.x);
                var intYPos = Mathf.RoundToInt(mouseWorldPos.y);

                if (GridManager.Instance.GetTile(intXPos, intYPos).TileEmpty)
                {
                    UIManager.Instance.CloseInformationTab();
                    if (m_SpawnerBuildingSelected)
                    {
                        SpawnerBuildingDeselect();
                    }

                    if (m_SoldierSelected)
                    {
                        SoldierDeselect();
                    }
                }
            }

            if (m_SpawnerBuildingSelected && Input.GetMouseButtonDown(1))
            {
                var intXPos = Mathf.RoundToInt(mouseWorldPos.x);
                var intYPos = Mathf.RoundToInt(mouseWorldPos.y);

                var tile = GridManager.Instance.GetTile(intXPos, intYPos); 
                if (tile && tile.TileEmpty)
                {
                    m_SpawnerBuildingsSpawnPoint.position = tile.transform.position;
                    m_SpawnerBuildingsSpawnPoint.gameObject.SetActive(true);
                }
            }

            if (m_SoldierSelected && Input.GetMouseButtonDown(1))
            {
                var intXPos = Mathf.RoundToInt(mouseWorldPos.x);
                var intYPos = Mathf.RoundToInt(mouseWorldPos.y);

                var tile = GridManager.Instance.GetTile(intXPos, intYPos);
                if (tile.TileEmpty)
                {
                    m_SelectedSoldier.StopAttacking();
                    m_SelectedSoldier.Move(tile);
                }
            }
        }
        
        #endregion

        if (Input.GetMouseButtonUp(0))
        {
            m_Dragging = false;
            m_StartedDragging = false;
        }
    }

    public void SelectBuilding(string buildingName)
    {
        m_SelectedBuildingStats = GameManager.Instance.BuildingsStats.GetStats(buildingName);
        m_BuildingXSize = m_SelectedBuildingStats.BuildingXSize;
        m_BuildingYSize = m_SelectedBuildingStats.BuildingYSize;
        m_BuildingSpriteRenderer.transform.localScale = new Vector3(m_BuildingXSize, m_BuildingYSize, 1);
        m_BuildingSpriteRenderer.sprite = m_SelectedBuildingStats.BuildingSprite;
        SpawnerBuildingDeselect();
        SoldierDeselect();
    }

    public void DeselectBuilding()
    {
        m_BuildingSpriteRenderer.sprite = null;
        m_AvailableBGSpriteRenderer.color = Color.clear;
        m_BuildingSpriteRenderer.gameObject.SetActive(false);
        GameManager.Instance.CurrentState = GameState.Idle;
    }

    public void BuildBuilding(int intXPos, int intYPos)
    {
        // using factory pattern here is unnecessary because of the scriptable system created, but here you go
        // its simplified version of factory pattern where factory is static and objects are still
        //on their scriptable stats object but in correct usage of factory pattern, you can store your
        //factory objects on factory. 
        var gridObject = GridObjectFactory.GetGridObject(m_SelectedBuildingStats.BuildingName);
        Instantiate(gridObject, new Vector2(intXPos, intYPos), Quaternion.identity);
        m_BuildingSpriteRenderer.sprite = null;
        m_AvailableBGSpriteRenderer.color = Color.clear;
        m_BuildingSpriteRenderer.gameObject.SetActive(false);
        
        for (int x = 0; x < m_BuildingXSize; x++)
        {
            for (int y = 0; y < m_BuildingYSize; y++)
            {
                var tile = GridManager.Instance.GetTile(intXPos + x, intYPos + y);
                tile.SetEmpty(false);
            }
        }
        
        EventManager.ProductionBuildingCompleted.Invoke();
    }

    private void HighlightTiles(out bool isTilesEmpty, int intXPos, int intYPos)
    {
        if(!m_BuildingSpriteRenderer.gameObject.activeSelf) m_BuildingSpriteRenderer.gameObject.SetActive(true);
            isTilesEmpty = true;
        for (int x = 0; x < m_BuildingXSize; x++)
        {
            for (int y = 0; y < m_BuildingYSize; y++)
            {
                var tile = GridManager.Instance.GetTile(intXPos + x, intYPos + y);
                if (!tile)
                {
                    isTilesEmpty = false;
                    break;
                }
                if (tile.TileEmpty) continue;
                isTilesEmpty = false;
                break;
            }

            if (!isTilesEmpty) break;
        }

        m_BuildingSpriteRenderer.transform.position = new Vector2(intXPos + m_BuildingXSize / 2f - m_CellOffset,
            intYPos + m_BuildingYSize / 2f - m_CellOffset);
        var availableCol = isTilesEmpty ? Color.green : Color.red;
        availableCol.a = 0.35f;
        m_AvailableBGSpriteRenderer.color = availableCol;
    }

    public void SpawnerBuildingSelected(Barracks selectedBarracks)
    {
        m_SpawnerBuildingsSpawnPoint = selectedBarracks.SpawnPoint;
        m_SpawnerBuildingSelected = true;
        SoldierDeselect();
    }

    public void SoldierSelected(Soldier soldier)
    {
        m_SelectedSoldier = soldier;
        m_SoldierSelected = true;
        SpawnerBuildingDeselect();
    }

    private void SoldierDeselect()
    {
        if (m_SelectedSoldier != null)
        {
            m_SoldierSelected = false;
            m_SelectedSoldier = null;
        }
    }

    private void SpawnerBuildingDeselect()
    {
        if (m_SpawnerBuildingsSpawnPoint != null)
        {
            m_SpawnerBuildingSelected = false;
            m_SpawnerBuildingsSpawnPoint.gameObject.SetActive(false);
            m_SpawnerBuildingsSpawnPoint = null;
        }
    }

    public void AttackWithSoldier(Vector3 toBeAttackedUnitPos, IHaveHealth toBeAttackedUnit)
    {
        if (m_SoldierSelected && !ReferenceEquals(toBeAttackedUnit, m_SelectedSoldier))
        {
            m_SelectedSoldier.Attack(toBeAttackedUnitPos, toBeAttackedUnit);
        }
    }
}