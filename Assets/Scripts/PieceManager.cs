using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Internal;

public class PieceManager : MonoBehaviour
{
    public List<Piece> pieces;
    public Vector3 previewPiecesPosition = new(9, 18, 0);
    private List<Piece> previewPieces = new();
    public static PieceManager Instance;
    public Vector3 keepPosition = new(-10, 15, 0);
    public Vector3 spawnPosition = new(0, 21, 0);
    private Piece keptPiece;
    private Piece activePiece;
    private PieceBag pieceBag;
    private bool swapPieces = false;
    private bool isPlacingPiece = false;
    public RotationState rotationState;
    private Vector3 movementDirection;
    [SerializeField] private readonly int PreviewPiecesSpacing = 3;
    [SerializeField] private readonly int maxPreviewPieces = 3;

    [SerializeField] public readonly float placingPieceSnappingMargin = 25f;


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

    void FixedUpdate()
    {
        if (isPlacingPiece)
            return;

        if (!swapPieces)
        {
            if (rotationState.isRotating)
            {
                Vector3 torque = new(0f, 0f, (rotationState.isPressingQ ? 1f : -1f) * rotationState.torqueStrength);
                activePiece.GetComponent<Rigidbody>().AddTorque(torque, ForceMode.Force);
            }

            if (movementDirection.magnitude > 0)
            {
                activePiece.GetComponent<Rigidbody>().AddForce(movementDirection, ForceMode.Force);
            }
            return;
        }
        movementDirection = Vector3.zero;
        rotationState.Reset();
        if (keptPiece != null)
        {
            Rigidbody keptPieceRb = keptPiece.GetComponent<Rigidbody>();
            keptPieceRb.isKinematic = true;
            keptPieceRb.useGravity = false;
            keptPieceRb.transform.SetPositionAndRotation(keepPosition, Quaternion.identity);
        }
        if (activePiece != null)
        {
            Rigidbody activePieceRB = activePiece.GetComponent<Rigidbody>();

            activePieceRB.isKinematic = true;
            activePieceRB.useGravity = false;
            activePieceRB.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
        }
        swapPieces = false;
    }

    public Piece KeepPiece()
    {
        if (keptPiece == null)
        {
            keptPiece = activePiece;
            SpawnNextPiece();
        }
        else
        {
            (activePiece, keptPiece) = (keptPiece, activePiece);
        }
        swapPieces = true;
        return activePiece;
    }

    public bool CanPlacePiece()
    {
        float eulerZ = activePiece.transform.eulerAngles.z;
        float snappedZ = Mathf.Round(eulerZ / 90f) * 90f;
        bool isSnapped = Mathf.Abs(Mathf.DeltaAngle(eulerZ, snappedZ)) <= placingPieceSnappingMargin;
        if (isSnapped)
        {
            isPlacingPiece = true;
        }
        return isSnapped;
    }
    public Piece PlacePiece()
    {
        Transform activePieceTransform = activePiece.transform;
        Rigidbody activePieceRb = activePiece.GetComponent<Rigidbody>();
        float eulerZ = activePieceTransform.eulerAngles.z;
        float snappedZ = Mathf.Round(eulerZ / 90f) * 90f;

        activePieceRb.useGravity = false;
        activePieceTransform.SetPositionAndRotation(
            new Vector3(
                Mathf.Round(activePieceTransform.position.x),
                Mathf.Round(activePieceTransform.position.y),
                activePieceTransform.position.z
            ),
            Quaternion.Euler(0, 0, snappedZ)
        );

        int minY = int.MaxValue;
        int maxY = int.MinValue;
        while (activePieceTransform.childCount > 0)
        {
            Transform blockTransform = activePieceTransform.GetChild(0);
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
        GridManager.Instance.CheckAndClearLines(minY, maxY);
        Destroy(activePiece.transform.root.gameObject);
        activePiece = SpawnNextPiece();
        isPlacingPiece = false;
        return activePiece;
    }

    public Piece SpawnNewPiece(Vector3 spawnLocation)
    {
        Piece piece = Instantiate(
            pieceBag.GetNewPiece(),
            spawnLocation,
            Quaternion.identity
        );
        return piece;
    }

    public Piece SpawnNextPiece()
    {
        activePiece = previewPieces[0];
        Rigidbody activePieceeRb = activePiece.GetComponent<Rigidbody>();
        activePieceeRb.position = spawnPosition;
        for (int i = 0; i < maxPreviewPieces - 1; i++)
        {
            Piece movingPiece = previewPieces[i + 1];
            Rigidbody movingPieceRb = movingPiece.GetComponent<Rigidbody>();
            movingPieceRb.position += new Vector3(0, PreviewPiecesSpacing, 0);
            previewPieces[i] = previewPieces[i + 1];
        }
        Vector3 newPreviewPosition = previewPiecesPosition + new Vector3(0, -(maxPreviewPieces - 1) * PreviewPiecesSpacing, 0);
        Piece newPreviewPiece = SpawnNewPiece(newPreviewPosition);
        previewPieces[maxPreviewPieces - 1] = newPreviewPiece;
        return activePiece;
    }

    public Piece Initialize()
    {
        pieceBag ??= new PieceBag(pieces);

        activePiece = SpawnNewPiece(spawnPosition);
        for (int i = 0; i < maxPreviewPieces; i++)
        {
            Vector3 newPreviewPosition = previewPiecesPosition + new Vector3(0, -i * PreviewPiecesSpacing, 0);
            Piece newPreviewPiece = SpawnNewPiece(newPreviewPosition);
            previewPieces.Add(newPreviewPiece);
        }
        return activePiece;
    }

    public void SetRotationState(RotationState rotationState)
    {
        this.rotationState = rotationState;
    }

    public void SetMovement(Vector3 movementDirection)
    {
        this.movementDirection = movementDirection;
    }
}

public class PieceBag
{
    private readonly List<Piece> pieceReferences;
    private List<Piece> bag;
    private Piece previousPiece;
    private const float PIECE_REPEAT_CHANCE = 0.25f;

    public PieceBag(List<Piece> pieceReferences)
    {
        this.pieceReferences = new List<Piece>(pieceReferences);
        FillBag();
    }

    private void FillBag()
    {
        bag = new List<Piece>(pieceReferences);
        do
        {
            for (int i = bag.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (bag[randomIndex], bag[i]) = (bag[i], bag[randomIndex]);
            }
        } while (bag[0] == previousPiece && Random.value > PIECE_REPEAT_CHANCE);

        previousPiece = bag[0];
    }
    // Test commit
    public Piece GetNewPiece()
    {
        if (bag.Count == 0)
            FillBag();
        Piece piece = bag[0];
        bag.RemoveAt(0);
        return piece;
    }
}


