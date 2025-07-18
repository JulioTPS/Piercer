using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PieceController : MonoBehaviour
{
    private struct RotationState
    {
        public bool isRotating;
        public bool isPressingQ;

        public RotationState(bool isRotating = false, bool isPressingQ = false)
        {
            this.isRotating = isRotating;
            this.isPressingQ = isPressingQ;
        }

        public void Reset()
        {
            isRotating = false;
            isPressingQ = false;
        }
    }

    public static PieceController Instance;
    private RotationState rotationState = new();

    [Header("Movement Settings")]
    public float movementStrength = 50f;
    public float linearDamping = 8f;
    public float linearDampingDefault = 0f;

    [Header("Rotation Settings")]
    public float torqueStrength = 12f;
    public float angularDamping = 2f;
    public float angularDampingDefault = 0.1f;


    private bool isPlacingPiece = false;
    private bool isDragging = false;

    private Camera mainCamera;
    private Vector3 mouseOffset;
    private Vector3 movementDirection = Vector3.zero;
    private Rigidbody activePieceRb;
    private Transform activePieceTransform;

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
        mainCamera = Camera.main;
        SetActivePiece(PieceManager.Instance.SpawnNewPiece());
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     SetActivePiece(PieceManager.Instance.SpawnNewPiece());
        // }

        if (isPlacingPiece || activePieceTransform == null)
            return;

        HandleMouseInput();

        bool isPressingQ = Input.GetKey(KeyCode.Q);
        if (isPressingQ || Input.GetKey(KeyCode.E))
        {
            rotationState.isRotating = true;
            rotationState.isPressingQ = isPressingQ;
            activePieceRb.angularDamping = angularDamping;
        }
        else if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.E))
        {
            rotationState.isRotating = false;
            rotationState.isPressingQ = false;
            if (!isDragging)
                activePieceRb.angularDamping = angularDampingDefault;
        }

        if (Input.GetKeyDown(KeyCode.F) && PieceManager.Instance.CanPlacePiece())
        {
            isPlacingPiece = true;
            isDragging = false;
            rotationState.Reset();
            SetActivePiece(PieceManager.Instance.PlacePiece());
            isPlacingPiece = false;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            isPlacingPiece = true;
            isDragging = false;
            rotationState.Reset();
            activePieceRb.isKinematic = true;
            activePieceRb.useGravity = false;
            SetActivePiece(PieceManager.Instance.KeepPiece());
        }
    }
    void FixedUpdate()
    {
        if (isPlacingPiece)
            return;

        if (rotationState.isRotating)
        {
            Vector3 torque = new(0f, 0f, (rotationState.isPressingQ ? 1f : -1f) * torqueStrength);
            activePieceRb.AddTorque(torque, ForceMode.Force);
        }

        if (isDragging)
        {
            activePieceRb.AddForce(movementDirection, ForceMode.Force);
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Piece hitPiece = hit.transform.GetComponentInParent<Piece>();
                if (hitPiece != null && hitPiece.transform == activePieceTransform)
                {
                    OnPieceMouseDown();
                }
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            OnPieceMouseHold();
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            OnPieceMouseUp();
        }
    }

    private void OnPieceMouseDown()
    {
        isDragging = true;
        activePieceRb.useGravity = false;
        activePieceRb.angularDamping = angularDamping;
        activePieceRb.linearDamping = linearDamping;
        mouseOffset = Input.mousePosition - mainCamera.WorldToScreenPoint(activePieceTransform.position);
    }

    private void OnPieceMouseHold()
    {
        Vector3 targetWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition - mouseOffset);
        Vector3 direction = targetWorldPos - activePieceTransform.position;
        direction.z = 0f;
        movementDirection = direction * movementStrength;
    }

    private void OnPieceMouseUp()
    {
        isDragging = false;
        activePieceRb.useGravity = true;
        activePieceRb.linearDamping = linearDampingDefault;
        activePieceRb.angularDamping = angularDampingDefault;
    }

    private void SetActivePiece(Piece newPiece)
    {
        activePieceTransform = newPiece.transform;
        activePieceRb = newPiece.GetComponent<Rigidbody>();
        activePieceRb.linearDamping = linearDampingDefault;
        activePieceRb.angularDamping = angularDampingDefault;
        isDragging = false;
        rotationState.Reset();
        isPlacingPiece = false;
        Debug.Assert(activePieceTransform != null, "Active piece does not have a Transform component.");
        Debug.Assert(activePieceRb != null, "Active piece does not have a Rigidbody component.");
    }
}
