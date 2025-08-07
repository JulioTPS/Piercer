using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private struct GridCell
    {
        public bool isOccupied;
        public Transform blockTransform;
    }

    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridHeight = 19;
    public Vector3Int origin = new Vector3Int(0, 0, 0);

    public static GridManager Instance;

    private GridCell[,] grid;

    public float animationDuration = 0.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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

    public int SetCell(Transform blockTransform)
    {
        int x = Mathf.FloorToInt(blockTransform.position.x - origin.x + 0.001f);
        int y = Mathf.FloorToInt(blockTransform.position.y - origin.y + 0.001f);

        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight || grid[x, y].isOccupied == true)
        {
            // Debug.LogWarning(
            //     $"Invalid position ({x}, {y}) for block: {blockTransform.name}. Out of bounds or already occupied."
            // );
            return -1;
        }

        grid[x, y].isOccupied = true;
        grid[x, y].blockTransform = blockTransform;

        return y;
    }

    public void CheckAndClearLines(int minY = 0, int maxY = 0)
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
            GameManager.Instance.AddScore(linesToClear.Count * 100);
        }
    }

    private void ClearLines(int firstY, int lastY)
    {
        for (int y = firstY; y <= lastY; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y].blockTransform != null)
                {
                    Destroy(grid[x, y].blockTransform.gameObject);
                    grid[x, y].blockTransform = null;
                    grid[x, y].isOccupied = false;
                }
            }
        }

        List<(Transform, Vector3)> blocksToMove = new();
        int moveAmount = lastY - firstY + 1;
        for (int y = firstY; y < gridHeight - moveAmount; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y + moveAmount].blockTransform != null)
                {
                    grid[x, y].blockTransform = grid[x, y + moveAmount].blockTransform;
                    blocksToMove.Add(
                        (grid[x, y].blockTransform, grid[x, y].blockTransform.position)
                    );
                    grid[x, y].isOccupied = true;

                    grid[x, y + moveAmount].blockTransform = null;
                    grid[x, y + moveAmount].isOccupied = false;
                }
            }
        }

        StartCoroutine(MoveBlocks(blocksToMove, moveAmount));
    }

    private System.Collections.IEnumerator MoveBlocks(
        List<(Transform blockTransform, Vector3 startPosition)> blocksToMove,
        int moveAmount
    )
    {
        Vector3 moveVector = Vector3.down * moveAmount;

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float progress = elapsedTime / animationDuration;
            progress *= progress;

            foreach (var (blockTransform, startPosition) in blocksToMove)
            {
                if (blockTransform != null)
                {
                    blockTransform.position = Vector3.Lerp(
                        startPosition,
                        startPosition + moveVector,
                        progress
                    );
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (var (blockTransform, startPosition) in blocksToMove)
        {
            if (blockTransform != null)
            {
                blockTransform.position = startPosition + moveVector;
            }
        }
    }
}
