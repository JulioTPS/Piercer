using UnityEngine;

public struct GridCell
{
    public char type; //may have more uses later
    public Vector3 worldPosition;
}

public class GridManager : MonoBehaviour
{
    public GridCell[,] grid = new GridCell[10, 19];
    public Vector3Int origin = new Vector3Int(0, 0, 0);

    void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 19; y++)
            {
                grid[x, y].type = 'f'; // 'f' for free
                grid[x, y].worldPosition = new Vector3(x + origin.x, y + origin.y, origin.z);
            }
        }
    }

    void Update()
    {
    }

    void OnEnable()
    {
        Piece.OnPiecePlaced += HandlePiecePlaced;
    }

    void OnDisable()
    {
        Piece.OnPiecePlaced -= HandlePiecePlaced;
    }

    void HandlePiecePlaced()
    {
    }
}
