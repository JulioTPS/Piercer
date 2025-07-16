using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PieceHandler
{
    private readonly List<GameObject> pieceReferences;
    private List<GameObject> bag;
    private GameObject previousPiece;
    private const float PIECE_REPEAT_CHANCE = 0.25f;
    private GameObject keptPiece;

    public PieceHandler(List<GameObject> pieceReferences)
    {
        this.pieceReferences = new List<GameObject>(pieceReferences);
        FillBag();
    }

    private void FillBag()
    {
        bag = new List<GameObject>(pieceReferences);
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

    public GameObject GetNextPiece()
    {
        if (bag.Count == 0)
            FillBag();
        GameObject piece = bag[0];
        bag.RemoveAt(0);
        return piece;
    }
}

public class GameManager : MonoBehaviour
{
    public List<GameObject> pieces;
    public Vector3 spawnPosition = new(0, 21, 0);
    public int score = 0;
    public static GameManager Instance;
    public TextMeshPro scoreText;
    private PieceHandler pieceHandler;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            pieceHandler = new PieceHandler(pieces);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start() { }

    void SpawnRandomPiece()
    {
        var piceSelected = pieceHandler.GetNextPiece();
        Instantiate(
            piceSelected,
            spawnPosition - piceSelected.transform.GetChild(0).GetComponent<Piece>().spawnOffset,
            Quaternion.identity
        );
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnRandomPiece();
        }
    }

    public void AddScore(int _score)
    {
        score += _score * 100;

        scoreText.text = "Score: " + score;
    }
}
