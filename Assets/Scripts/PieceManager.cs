using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PieceManager : MonoBehaviour
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

    public List<Piece> pieces;
    public static PieceManager Instance;
    private PieceBag pieceBag;
    private RotationState rotationState = new();

    [Header("Movement Settings")]
    public float movementStrength = 50f;
    public float linearDamping = 8f;
    public float linearDampingDefault = 0f;

    [Header("Rotation Settings")]
    public float torqueStrength = 12f;
    public float angularDamping = 2f;
    public float angularDampingDefault = 0.1f;

    public Vector3 spawnPosition = new(0, 21, 0);

    private bool isPlacingPiece = false;
    private bool isDragging = false;

    private Camera mainCamera;
    private Vector3 mouseOffset;
    private Vector3 movementDirection = Vector3.zero;
    private Rigidbody activePieceRb;
    private Piece activePiece;
    private const float PLACING_PIECE_SNAPING_MARGIN = 0.25f;

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
        pieceBag = new PieceBag(pieces);
        mainCamera = Camera.main;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnRandomPiece();
        }

        if (isPlacingPiece)
            return;

        HandleMouseInput();

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
                activePieceRb.angularDamping = angularDampingDefault;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            PlacePiece();
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

            activePieceRb.angularDamping = angularDamping;
        }

        if (isDragging)
        {
            activePieceRb.AddForce(movementDirection, ForceMode.Force);
        }
    }

    private void HandleMouseInput()
    {
        if (activePiece == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.IsChildOf(activePiece.transform) || hit.transform == activePiece.transform)
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
        mouseOffset = Input.mousePosition - mainCamera.WorldToScreenPoint(activePiece.transform.position);
    }

    private void OnPieceMouseHold()
    {
        Vector3 targetWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition - mouseOffset);
        Vector3 direction = targetWorldPos - activePiece.transform.position;
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

    private void PlacePiece()
    {
        float eulerZ = activePiece.transform.eulerAngles.z;
        float snappedZ = Mathf.Round(eulerZ / 90f) * 90f;
        if (Mathf.Abs(Mathf.DeltaAngle(eulerZ, snappedZ)) > PLACING_PIECE_SNAPING_MARGIN)
            return;

        isPlacingPiece = true;
        activePieceRb.isKinematic = false;
        activePieceRb.useGravity = false;
        activePiece.transform.SetPositionAndRotation(
            new Vector3(
                Mathf.Round(activePiece.transform.position.x),
                Mathf.Round(activePiece.transform.position.y),
                activePiece.transform.position.z
            ),
            Quaternion.Euler(0, 0, snappedZ)
        );

        int minY = int.MaxValue;
        int maxY = int.MinValue;
        while (activePiece.transform.childCount > 0)
        {
            Transform blockTransform = activePiece.transform.GetChild(0);
            int gridY = GridManager.Instance.SetCell(blockTransform, activePiece.type);
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
        StartCoroutine(DestroyAndReset());
        return;
    }

    private IEnumerator DestroyAndReset()
    {
        Destroy(activePiece.transform.root.gameObject);
        yield return null;
        isPlacingPiece = false;
        isDragging = false;
        activePiece = null;
        activePieceRb = null;
        rotationState.Reset();
    }

    void SpawnRandomPiece()
    {
        Piece piceSelected = pieceBag.GetNewPiece();
        activePiece = Instantiate(
            piceSelected,
            spawnPosition,
            Quaternion.identity
        );
        activePieceRb = activePiece.GetComponent<Rigidbody>();
    }
}
