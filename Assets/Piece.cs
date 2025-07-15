using System;
using UnityEngine;

public struct RotationState
{
    public bool isRotating;
    public bool isPressingQ;

    public RotationState(bool isRotating = false, bool isPressingQ = false)
    {
        this.isRotating = false;
        this.isPressingQ = false;
    }

    public void Set(bool isRotating, bool isPressingQ)
    {
        this.isRotating = isRotating;
        this.isPressingQ = isPressingQ;
    }
}

public class Piece : MonoBehaviour
{
    public Vector3 spawnOffset;
    public Color blockColor;
    public char type;
    public float movementStrenght = 50f;
    public float movementDamping = 8f;
    public float movementDampingDefault = 0f;
    public float torqueStrenght = 16f;
    public float torqueDamping = 2f;
    public float torqueDampingDefault = 0.01f;
    private bool isBeingPlaced = false;
    private bool isDragging = false;
    private Vector3 movementDirection = Vector3.zero;
    private Camera mainCamera;
    private Vector3 mouseOffset;
    private Rigidbody rb;
    private Quaternion targetRotation;
    private RotationState rotationState = new();
    // private float timer;

    void Start()
    {
        targetRotation = transform.rotation;
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        rb.angularDamping = torqueDampingDefault;
        rb.linearDamping = movementDampingDefault;

        foreach (Block block in GetComponentsInChildren<Block>())
        {
            block.SetColor(blockColor);
        }
    }

    void Update()
    {
        if (!isBeingPlaced)
        {
            bool isPressingQ = Input.GetKey(KeyCode.Q);
            if (isPressingQ || Input.GetKey(KeyCode.E))
            {
                rotationState.isRotating = true;
                rotationState.isPressingQ = isPressingQ;
            }
            else if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.E))
            {
                rotationState.isRotating = false;
                rotationState.isPressingQ = false;
                if (!isDragging)
                    rb.angularDamping = torqueDampingDefault;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                PlacePiece();
            }
        }
    }

    void FixedUpdate()
    {
        // timer += Time.fixedDeltaTime;
        // if (timer >= 2.0f)
        // {
        //     Debug.Log("toruqe damping: " + rb.angularDamping);
        //     timer = 0f;
        // }
        if (!isBeingPlaced)
        {
            if (rotationState.isRotating)
            {
                Vector3 torque = new(0f, 0f, (rotationState.isPressingQ ? 1f : -1f) * torqueStrenght);
                rb.AddTorque(torque, ForceMode.Force);

                rb.angularDamping = torqueDamping;
            }

            if (isDragging)
            {
                rb.AddForce(movementDirection, ForceMode.Force);
            }
        }
    }

    void OnMouseDown()
    {
        if (isBeingPlaced)
            return;
        rb.useGravity = false;
        mouseOffset = Input.mousePosition - mainCamera.WorldToScreenPoint(transform.position);

        rb.angularDamping = torqueDamping;
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        Vector3 targetWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition - mouseOffset);
        Vector3 direction = targetWorldPos - transform.position;
        direction.z = 0f;
        movementDirection = direction * movementStrenght;
        rb.linearDamping = movementDamping;
    }

    private void OnMouseUp()
    {
        if (isBeingPlaced)
            return;
        isDragging = false;
        rb.useGravity = true;
        rb.linearDamping = movementDampingDefault;
        rb.angularDamping = torqueDampingDefault;
    }

    private void PlacePiece()
    {
        Vector3 euler = transform.eulerAngles;
        float z = euler.z;
        float snappedZ = Mathf.Round(z / 90f) * 90f;
        if (Mathf.Abs(Mathf.DeltaAngle(z, snappedZ)) <= 20f)
        {
            isBeingPlaced = true;
            rb.isKinematic = false;
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

            int minY = int.MaxValue;
            int maxY = int.MinValue;
            while (transform.childCount > 0)
            {
                Transform blockTransform = transform.GetChild(0);
                int gridY = GridManager.Instance.SetCell(blockTransform, type);
                if (gridY < 0)
                {
                    blockTransform.SetParent(null);
                    Destroy(blockTransform.gameObject);
                    continue;
                }
                blockTransform.SetParent(GridManager.Instance.transform, true);

                if (gridY < minY)
                    minY = gridY;
                if (gridY > maxY)
                    maxY = gridY;
            }
            GridManager.Instance.CheckLines(minY, maxY);

            Destroy(transform.root.gameObject);
            return;
        }
    }
}
