using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public struct RotationState
{
    public bool isRotating;
    public bool isPressingQ;

    public float torqueStrength;

    public RotationState(bool isRotating = false, bool isPressingQ = false, float torqueStrength = 16f)
    {
        this.isRotating = isRotating;
        this.isPressingQ = isPressingQ;
        this.torqueStrength = torqueStrength;
    }

    public void Reset()
    {
        isRotating = false;
        isPressingQ = false;
        torqueStrength = 16f;
    }
}

public class PieceController : MonoBehaviour
{

    public static PieceController Instance;
    private RotationState rotationState = new();

    [Header("Movement Settings")]
    public float movementStrength = 50f;
    public float linearDamping = 8f;
    public float linearDampingDefault = 0f;

    [Header("Rotation Settings")]
    public float torqueStrength = 16f;
    public float angularDamping = 2f;
    public float angularDampingDefault = 0.1f;


    private bool isPlacingPiece = false;
    private bool publicControllerLock = false;
    private bool isDragging = false;

    private Camera mainCamera;
    private Vector3 movementDirection = Vector3.zero;
    private Rigidbody activePieceRb;
    private Transform activePieceTransform;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            rotationState.torqueStrength = torqueStrength;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
        SetActivePiece(PieceManager.Instance.Initialize());
    }

    void Update()
    {
        if (isPlacingPiece || activePieceTransform == null || publicControllerLock)
            return;

        HandleMouseInput();

        bool isPressingQ = Input.GetKey(KeyCode.Q);
        if (isPressingQ || Input.GetKey(KeyCode.E))
        {
            rotationState.isRotating = true;
            rotationState.isPressingQ = isPressingQ;
            activePieceRb.angularDamping = angularDamping;
            PieceManager.Instance.SetRotationState(rotationState);
        }
        else if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.E))
        {
            rotationState.isRotating = false;
            rotationState.isPressingQ = false;
            PieceManager.Instance.SetRotationState(rotationState);
            if (!isDragging)
                activePieceRb.angularDamping = angularDampingDefault;
        }

        if (Input.GetKeyDown(KeyCode.F) && PieceManager.Instance.CanPlacePiece())
        {
            isPlacingPiece = true;
            isDragging = false;
            rotationState.Reset();
            SetActivePiece(PieceManager.Instance.PlacePiece());
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            isPlacingPiece = true;
            OnPieceMouseUp();
            rotationState.Reset();
            activePieceRb.isKinematic = true;
            activePieceRb.useGravity = false;
            SetActivePiece(PieceManager.Instance.KeepPiece());
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
                    OnPieceMouseDown(hit.point);
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

    private void OnPieceMouseDown(Vector3 worldHitPoint)
    {
        isDragging = true;

        Vector3 localHitPoint = activePieceTransform.InverseTransformPoint(worldHitPoint);
        localHitPoint.z = 0f;
        activePieceRb.centerOfMass = localHitPoint;

        activePieceRb.useGravity = false;
        activePieceRb.angularDamping = angularDamping;
        activePieceRb.linearDamping = linearDamping;
    }

    private void OnPieceMouseHold()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.WorldToScreenPoint(activePieceTransform.position).z));

        Vector3 centerOfMassWorld = activePieceTransform.TransformPoint(activePieceRb.centerOfMass);
        Vector3 direction = mouseWorldPos - centerOfMassWorld;
        direction.z = 0f;
        movementDirection = direction * movementStrength;
        PieceManager.Instance.SetMovement(movementDirection);
    }

    private void OnPieceMouseUp()
    {
        isDragging = false;
        activePieceRb.ResetCenterOfMass();
        activePieceRb.useGravity = true;
        activePieceRb.linearDamping = linearDampingDefault;
        activePieceRb.angularDamping = angularDampingDefault;
        PieceManager.Instance.SetMovement(Vector3.zero);
    }

    private void SetActivePiece(Piece newPiece)
    {
        activePieceTransform = newPiece.transform;
        activePieceRb = newPiece.GetComponent<Rigidbody>();
        activePieceRb.linearDamping = linearDampingDefault;
        activePieceRb.angularDamping = angularDampingDefault;
        StartCoroutine(EnablePhysicsNextFrame());
        isDragging = false;
        rotationState.Reset();
        isPlacingPiece = false;
        Debug.Assert(activePieceTransform != null, "Active piece does not have a Transform component.");
        Debug.Assert(activePieceRb != null, "Active piece does not have a Rigidbody component.");
    }

    private IEnumerator EnablePhysicsNextFrame()
    {
        yield return new WaitForFixedUpdate();
        if (activePieceRb != null)
        {
            activePieceRb.isKinematic = false;
        }
    }

    public void SwitchControllerLock(bool lockState)
    {
        publicControllerLock = lockState;
    }

    // void OnDrawGizmos()
    // {
    //     if (activePieceRb != null)
    //     {
    //         // Draw center of mass as a red sphere
    //         Gizmos.color = Color.red;
    //         Vector3 worldCenterOfMass = activePieceTransform.TransformPoint(activePieceRb.centerOfMass);
    //         Gizmos.DrawSphere(worldCenterOfMass, 0.1f);
    //     }
    // }

    // void OnDrawGizmosSelected()
    // {
    //     // Only show when piece is selected in editor
    //     if (activePieceRb != null)
    //     {
    //         Gizmos.color = Color.green;
    //         Vector3 worldCenterOfMass = activePieceTransform.TransformPoint(activePieceRb.centerOfMass);
    //         Gizmos.DrawWireSphere(worldCenterOfMass, 0.15f);
    //     }
    // }
}
