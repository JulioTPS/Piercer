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

    public float animationDuration = 0.5f;

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

    public int SetCell(Transform blockTransform, char type)
    {
        if (blockTransform == null)
            return -1;

        int x = Mathf.FloorToInt(blockTransform.position.x - origin.x + 0.001f);
        int y = Mathf.FloorToInt(blockTransform.position.y - origin.y + 0.001f);

        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight || grid[x, y].isOccupied == true)
            return -1;

        grid[x, y].isOccupied = true;
        grid[x, y].type = type;
        grid[x, y].blockObject = blockTransform.gameObject;

        return y;
    }

    public void CheckLines(int minY = 0, int maxY = 0)
    {
        var linesToClear = new List<int>();

        for (int y = minY; y <= maxY; y++)
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

        if (linesToClear.Count > 0)
        {
            ClearLines(linesToClear.First(), linesToClear.Last());
            GameManager.Instance.AddScore(linesToClear.Count);
        }
    }

    private void ClearLines(int firstY, int lastY)
    {
        for (int y = firstY; y <= lastY; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y].blockObject != null)
                {
                    Destroy(grid[x, y].blockObject);
                    grid[x, y].blockObject = null;
                    grid[x, y].type = '\0';
                    grid[x, y].isOccupied = false;
                }
            }
        }

        List<(GameObject gameObject, Vector3 startingPosition)> blocksToMove = new List<(GameObject, Vector3)>();
        int moveAmount = lastY - firstY + 1;
        for (int y = firstY; y < gridHeight - moveAmount; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y + moveAmount].blockObject != null)
                {
                    grid[x, y].blockObject = grid[x, y + moveAmount].blockObject;
                    blocksToMove.Add((grid[x, y].blockObject, grid[x, y].blockObject.transform.position));
                    grid[x, y].type = grid[x, y + moveAmount].type;
                    grid[x, y].isOccupied = true;

                    grid[x, y + moveAmount].blockObject = null;
                    grid[x, y + moveAmount].type = '\0';
                    grid[x, y + moveAmount].isOccupied = false;
                }
            }
        }

        StartCoroutine(MoveBlocks(blocksToMove, moveAmount));
    }

    private System.Collections.IEnumerator MoveBlocks(List<(GameObject, Vector3)> blocks, int moveAmount)
    {
        List<(GameObject gameObject, Vector3 startPosition)> blocksToMove = blocks;
        Vector3 moveVector = Vector3.down * moveAmount;

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float progress = elapsedTime / animationDuration;
            progress = progress * progress;

            foreach (var block in blocksToMove)
            {
                if (block.gameObject != null)
                {
                    block.gameObject.transform.position = Vector3.Lerp(block.startPosition, block.startPosition + moveVector, progress);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (var block in blocksToMove)
        {
            if (block.gameObject != null)
            {
                block.gameObject.transform.position = block.startPosition + moveVector;
            }
        }
    }

    // public bool RemoveBlock(int x, int y)
    // {
    //     if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
    //         return false;

    //     if (grid[x, y].blockObject != null)
    //     {
    //         Destroy(grid[x, y].blockObject);
    //         grid[x, y].blockObject = null;
    //     }

    //     grid[x, y].type = 'f'; 
    //     return true;
    // }

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
