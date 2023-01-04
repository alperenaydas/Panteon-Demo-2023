using UnityEngine;

public class Tile : MonoBehaviour
{
    private bool m_TileEmpty; //used to build structures or spawn characters

    public bool TileEmpty => m_TileEmpty;

    [SerializeField] private SpriteRenderer m_Renderer;

    #region a* pathfinding
    
    public int x { get; set; }
    public int y { get; set; }
    public int gCost { get; set; }
    public int hCost { get; set; }
    public int fCost { get; set; }

    public Tile cameFromTile;

    public void CalculateFCost()
    {
        fCost = hCost + gCost;
    }
    
    #endregion
    
    private void Start()
    {
        m_TileEmpty = true;
    }
    
    public void SetEmpty(bool isEmpty)
    {
        m_TileEmpty = isEmpty;
    }

    public void Init(int x, int y, Color defaultColor)
    {
        gameObject.name = $"Tile {x} {y}";
        m_Renderer.color = defaultColor;
        this.x = x;
        this.y = y;
    }
    
    
}