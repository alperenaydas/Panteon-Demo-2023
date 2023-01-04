using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Tile m_TilePrefab;
    [Tooltip("Scale factor of tile prefab, 1 is best results")]
    public float CellSize;
    
    [SerializeField] private int m_Width, m_Height;
    public int Width => m_Width;
    public int Height => m_Height;

    [SerializeField] private Color m_IndexZeroColor, m_IndexOneColor;

    private Tile[,] m_GridTiles;

    private Transform m_GridParent;

    private List<Tile> openList;
    private List<Tile> closedList;

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    #region Singleton

    private static GridManager m_Instance;

    public static GridManager Instance => m_Instance;


    private void Awake()
    {
        m_Instance = this;
    }

    #endregion

    private void Start()
    {
        m_GridTiles = new Tile[m_Width, m_Height];

        m_GridParent = new GameObject("Grid").transform;

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < m_Width; x++)
        {
            for (int y = 0; y < m_Height; y++)
            {
                var newTile = Instantiate(m_TilePrefab, new Vector3(x * CellSize, y * CellSize, 1),
                    Quaternion.identity, m_GridParent);
                newTile.Init(x, y, (x + y) % 2 == 0 ? m_IndexZeroColor : m_IndexOneColor);
                m_GridTiles[x, y] = newTile;
            }
        }

        Camera.main.transform.position =
            new Vector3((m_Width * CellSize - CellSize) / 2f, (m_Height * CellSize - CellSize) / 2f,
                -10f); // set cam pos to middle of grid.
    }

    public Tile GetTile(int x, int y)
    {
        var xIndex = (int)(x / CellSize);
        var yIndex = (int)(y / CellSize);
        if (xIndex >= m_Width || xIndex < 0 || yIndex >= m_Height || yIndex < 0) return null;
        return m_GridTiles[xIndex, yIndex];
    }

    public Tile GetTile(Vector2 posVector)
    {
        var xIndex = (int)posVector.x;
        var yIndex = (int)posVector.y;
        return GetTile(xIndex, yIndex);
    }

    public Tile GetClosestTile(Vector2 pos)
    {
        var tile = GetTile(pos);
        if (tile != null && tile.TileEmpty) return tile;
        var tempCounterX = 0;
        var tempCounterY = 0;
        var tempCounterMinusX = 0;
        var tempCounterMinusY = 0;
        var squareOffset = 1;
        var tempPos = pos - Vector2.right;
        tile = GetTile(tempPos);
        var maxSearchDepth = m_Width*m_Height;
        var counter = 0;
        while (tile == null || !tile.TileEmpty)
        {
            if (counter == maxSearchDepth)
            {
                Debug.Log("path not found in range of search depth.");
                return null;
            }
            counter++;
            
            if (tempCounterY < squareOffset)
            {
                tempPos += Vector2.up;
                tempCounterY++;
            }
            else if (tempCounterX < squareOffset + 1)
            {
                tempPos += Vector2.right;
                tempCounterX++;
            }
            else if (tempCounterMinusY < squareOffset + 1)
            {
                tempPos -= Vector2.up;
                tempCounterMinusY++;
            }
            else if (tempCounterMinusX < squareOffset + 2)
            {
                tempPos -= Vector2.right;
                tempCounterMinusX++;
            }
            else
            {
                squareOffset += 2;
                tempCounterX = 0;
                tempCounterY = 0;
                tempCounterMinusY = 0;
                tempCounterMinusX = 0;
            }
            tile = GetTile(tempPos);
            
        }
        return tile;
    }

    #region A* Pathfinding

    public List<Tile> FindPath(int startX, int startY, int endX, int endY)
    {
        Tile startTile = GetTile(startX, startY);
        Tile endTile = GetTile(endX, endY);
        openList = new List<Tile> { startTile };
        closedList = new List<Tile>();

        for (int x = 0; x < m_Width; x++)
        {
            for (int y = 0; y < m_Height; y++)
            {
                var tile = GetTile(x, y);
                tile.gCost = Int32.MaxValue;
                tile.CalculateFCost();
                tile.cameFromTile = null;
            }
        }

        startTile.gCost = 0;
        startTile.hCost = CalculateDistanceCost(startTile, endTile);
        startTile.CalculateFCost();

        while (openList.Count > 0)
        {
            var currentTile = GetLowestFCostTile(openList);
            if (currentTile == endTile)
            {
                return CalculatePath(endTile);
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            foreach (var neighbourTile in GetNeighbourList(currentTile))
            {
                if(closedList.Contains(neighbourTile))continue;
                if (!neighbourTile.TileEmpty)
                {
                    closedList.Add(neighbourTile);
                    continue;
                }

                int tentativeGCost = currentTile.gCost + CalculateDistanceCost(currentTile, neighbourTile);
                if (tentativeGCost < neighbourTile.gCost)
                {
                    neighbourTile.cameFromTile = currentTile;
                    neighbourTile.gCost = tentativeGCost;
                    neighbourTile.hCost = CalculateDistanceCost(neighbourTile, endTile);
                    neighbourTile.CalculateFCost();

                    if (!openList.Contains(neighbourTile))
                    {
                        openList.Add(neighbourTile);
                    }
                }
            }
        }
        
        //out of nodes on openlist
        return null;
    }

    private int CalculateDistanceCost(Tile tileA, Tile tileB)
    {
        int xDistance = Mathf.Abs(tileA.x - tileB.x);
        int yDistance = Mathf.Abs(tileA.y - tileB.y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private Tile GetLowestFCostTile(List<Tile> tiles)
    {
        var lowestFCostTile = tiles[0];
        for (int i = 1; i < tiles.Count; i++)
        {
            if (tiles[i].fCost < lowestFCostTile.fCost)
            {
                lowestFCostTile = tiles[i];
            }
        }

        return lowestFCostTile;
    }

    private List<Tile> CalculatePath(Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        path.Add(endTile);
        var currentTile = endTile;
        while (currentTile.cameFromTile != null)
        {
            path.Add(currentTile.cameFromTile);
            currentTile = currentTile.cameFromTile;
        }
        
        path.Reverse();
        return path;
    }

    private List<Tile> GetNeighbourList(Tile tile)
    {
        List<Tile> neighbourList = new List<Tile>();
        if (tile.x - 1 >= 0)
        {
            neighbourList.Add(GetTile(tile.x - 1, tile.y));
            if (tile.y - 1 >= 0)
            {
                neighbourList.Add(GetTile(tile.x - 1, tile.y - 1));
            }

            if (tile.y + 1 < m_Height)
            {
                neighbourList.Add(GetTile(tile.x - 1, tile.y + 1));
            }
        }

        if (tile.x + 1 < m_Width)
        {
            neighbourList.Add(GetTile(tile.x + 1, tile.y));
            if (tile.y - 1 >= 0)
            {
                neighbourList.Add(GetTile(tile.x + 1, tile.y - 1));
            }

            if (tile.y + 1 < m_Height)
            {
                neighbourList.Add(GetTile(tile.x + 1, tile.y + 1));
            }
        }

        if (tile.y - 1 >= 0) neighbourList.Add(GetTile(tile.x, tile.y - 1));
        if (tile.y + 1 < m_Height) neighbourList.Add(GetTile(tile.x, tile.y + 1));
        return neighbourList;
    }

    #endregion
    
}