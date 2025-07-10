using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public struct GridCell
{
    public bool isOccupied;
    public char type; //may be useful later
    public GameObject blockObject;
}

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridHeight = 19;
    public Vector3Int origin = new Vector3Int(0, 0, 0);

    [Header("Block Prefabs")]
    public GameObject blockPrefab;

    public static GridManager Instance;

    private GridCell[,] grid;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new GridCell[gridWidth, gridHeight];
    }

    // Helper methods for accessing the grid
    public GridCell GetCell(int x, int y)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
            return grid[x, y];
        return default(GridCell);
    }

    public int SetCell(Transform blockTransform, char type, Color color)
    {
        if (blockTransform == null)
            return -1;

        int x = Mathf.RoundToInt(blockTransform.position.x - origin.x);
        int y = Mathf.RoundToInt(blockTransform.position.y - origin.y);
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight || grid[x, y].isOccupied == true)
            return -1;

        grid[x, y].isOccupied = true;
        grid[x, y].type = type;
        grid[x, y].blockObject = blockTransform.gameObject;

        return y;
    }

    public async Task CheckLines(int minY = 0, int MaxY = 1)
    {
        var linesToClear = new List<int>();

        await Task.Run(() =>
        {
            for (int y = minY; y <= MaxY; y++)
            {
                bool isLineOccupied = true;
                for (int x = 0; x < gridWidth; x++)
                {
                    if (grid[x, y].isOccupied == false)
                    {
                        isLineOccupied = false;
                        break;
                    }
                }

                if (isLineOccupied)
                {
                    linesToClear.Add(y);
                }
            }
        });

        if (linesToClear.Count == 0)
            return;
        else if (linesToClear.Count > 1)
        {
            ClearLines(linesToClear.First(), linesToClear.Last());
        }
    }

    private void ClearLines(int firstY, int lastY)
    {
        if (firstY == lastY)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Destroy(grid[x, firstY].blockObject);
                grid[x, firstY].blockObject = null;
                grid[x, firstY].isOccupied = false;
            }
        }
        else
        {
            for (int y = firstY; y <= lastY; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    Destroy(grid[x, y].blockObject);
                    grid[x, y].blockObject = null;
                    grid[x, y].isOccupied = false;
                }
            }
        }

        for (; y < gridHeight - 1; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y + 1].blockObject != null)
                {
                    // grid[x, shiftY - 1] = grid[x, shiftY];
                    // grid[x, shiftY].blockObject = null;
                    // grid[x, shiftY].type = 'f';
                }
            }
        }
    }

    public bool RemoveBlock(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
            return false;

        if (grid[x, y].blockObject != null)
        {
            Destroy(grid[x, y].blockObject);
            grid[x, y].blockObject = null;
        }

        grid[x, y].type = 'f'; // Mark as free
        return true;
    }

    // public GameObject GetBlockAt(int x, int y)
    // {
    //     if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
    //         return grid[x, y].blockObject;
    //     return null;
    // }

    // public Vector3 GridToWorldPosition(int x, int y)
    // {
    //     return new Vector3(x + origin.x, y + origin.y, origin.z);
    // }

    // public Vector2Int WorldToGridPosition(Vector3 worldPos)
    // {
    //     return new Vector2Int(
    //         Mathf.RoundToInt(worldPos.x - origin.x),
    //         Mathf.RoundToInt(worldPos.y - origin.y)
    //     );
    // }
}
