using UnityEngine;

public class Piece : MonoBehaviour
{
    public Color blockColor;
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        foreach (Block block in GetComponentsInChildren<Block>())
        {
            block.SetColor(blockColor);
        }
    }

    void Update()
    {
        // Rotate with Q and E
        if (isDragging)
        {
            if (Input.GetKeyDown(KeyCode.Q))
                transform.Rotate(0, 0, 90);
            if (Input.GetKeyDown(KeyCode.E))
                transform.Rotate(0, 0, -90);
        }

        // Drag logic
        if (isDragging)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(mainCamera.WorldToScreenPoint(transform.position).z);
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos) + offset;
            transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCamera.WorldToScreenPoint(transform.position).z);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        offset = transform.position - worldPos;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }
}
