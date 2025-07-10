using System;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Vector3 spawnOffset;
    public static event Action OnPiecePlaced;
    public Color blockColor;
    public char type;
    private bool isDragging = false;
    private bool isBeingPlaced = false;
    private Vector3 offset;
    private Camera mainCamera;

    private Rigidbody rb;

    private Quaternion targetRotation;

    void Start()
    {
        mainCamera = Camera.main;
        targetRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        foreach (Block block in GetComponentsInChildren<Block>())
        {
            block.SetColor(blockColor);
        }
    }

    void Update()
    {
        if (!isBeingPlaced)
        {
            if (isDragging)
            {
                HandleRotation();
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = Mathf.Abs(mainCamera.WorldToScreenPoint(transform.position).z);
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos) + offset;
                transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                isDragging = false;
                PlacePiece();
                OnPiecePlaced?.Invoke();
            }
        }
    }

    private async void PlacePiece()
    {
        Vector3 euler = transform.eulerAngles;
        float z = euler.z;
        float snappedZ = Mathf.Round(z / 90f) * 90f;
        if (Mathf.Abs(Mathf.DeltaAngle(z, snappedZ)) <= 20f)
        {
            isBeingPlaced = true;
            rb.isKinematic = true;
            rb.useGravity = false;
            targetRotation = Quaternion.Euler(euler.x, euler.y, snappedZ);
            Vector3 rotatedOffset = targetRotation * spawnOffset;
            transform.SetPositionAndRotation(
                new Vector3(
                    Mathf.Round(transform.position.x + rotatedOffset.x) - rotatedOffset.x,
                    Mathf.Round(transform.position.y + rotatedOffset.y) - rotatedOffset.y,
                    transform.position.z
                ),
                targetRotation
            );

            int minY = 0;
            int maxY = 0;
            while (transform.childCount > 0)
            {
                Transform blockTransform = transform.GetChild(0);
                int gridY = GridManager.Instance.SetCell(blockTransform, type, blockColor);
                blockTransform.SetParent(GridManager.Instance.transform, true);

                if (gridY < minY)
                    minY = gridY;
                else if (gridY > maxY)
                    maxY = gridY;
            }
            Destroy(gameObject);

            await GridManager.Instance.CheckLines(minY, maxY);
        }
    }

    private void HandleRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            targetRotation *= Quaternion.Euler(0f, 0f, 90f * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            targetRotation *= Quaternion.Euler(0f, 0f, -90f * Time.deltaTime);
        }
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * 10f
        );
    }

    void OnMouseDown()
    {
        if (isBeingPlaced)
            return;
        isDragging = true;
        targetRotation = transform.rotation;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCamera.WorldToScreenPoint(transform.position).z);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        offset = transform.position - worldPos;

        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void OnMouseUp()
    {
        if (isBeingPlaced)
            return;
        isDragging = false;
        rb.isKinematic = false;
        rb.useGravity = true;
    }
}
