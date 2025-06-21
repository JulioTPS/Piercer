using UnityEngine;
using System;

public class Piece : MonoBehaviour
{

    public static event Action OnPiecePlaced;
    public Color blockColor;
    private bool isDragging = false;
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
            PlacePiece();
            OnPiecePlaced?.Invoke();
        }
    }

    private void PlacePiece()
    {
        rb.isKinematic = false;
        rb.useGravity = false;
        isDragging = false;
        Vector3 euler = transform.eulerAngles;
        float z = euler.z;
        float snappedZ = Mathf.Round(z / 90f) * 90f;
        if (Mathf.Abs(Mathf.DeltaAngle(z, snappedZ)) <= 20f)
        {
            targetRotation = Quaternion.Euler(euler.x, euler.y, snappedZ);
            transform.SetPositionAndRotation(new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z), targetRotation);
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
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }
    void OnMouseDown()
    {
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
        isDragging = false;
        rb.isKinematic = false;
        rb.useGravity = true;
    }
}
