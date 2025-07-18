using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PieceManager : MonoBehaviour
{
    public List<Piece> pieces;
    public static PieceManager Instance;
    public Vector3 keepPosition = new(-10, 15, 0);
    public Vector3 spawnPosition = new(0, 21, 0);
    private Piece keptPiece;
    private Piece activePiece;
    private PieceBag pieceBag;
    private bool swapPieces = false;

    public float placingPieceSnappingMargin = 25f;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            pieceBag = new PieceBag(pieces);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (swapPieces)
        {
            if (keptPiece != null)
            {
                Rigidbody keptPieceRb = keptPiece.GetComponent<Rigidbody>();
                keptPieceRb.position = keepPosition;
                keptPieceRb.transform.rotation = Quaternion.identity;
                keptPieceRb.isKinematic = true;
                keptPieceRb.useGravity = false;
            }
            if (activePiece != null)
            {
                Rigidbody activePieceRB = activePiece.GetComponent<Rigidbody>();
                activePieceRB.position = spawnPosition;
                activePieceRB.isKinematic = false;
                activePieceRB.useGravity = true;
            }
            swapPieces = false;
        }
    }

    public Piece KeepPiece()
    {
        if (keptPiece == null)
        {
            keptPiece = activePiece;
            SpawnNewPiece();
        }
        else
        {
            (activePiece, keptPiece) = (keptPiece, activePiece);
        }
        swapPieces = true;
        return activePiece;
    }

    public Piece SpawnNewPiece()
    {
        activePiece = Instantiate(
            pieceBag.GetNewPiece(),
            spawnPosition,
            Quaternion.identity
        );
        return activePiece;
    }

    public bool CanPlacePiece()
    {
        float eulerZ = activePiece.transform.eulerAngles.z;
        float snappedZ = Mathf.Round(eulerZ / 90f) * 90f;
        return Mathf.Abs(Mathf.DeltaAngle(eulerZ, snappedZ)) <= placingPieceSnappingMargin;
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
        return SpawnNewPiece();
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

    public Piece GetNewPiece()
    {
        if (bag.Count == 0)
            FillBag();
        Piece piece = bag[0];
        bag.RemoveAt(0);
        return piece;
    }
}


