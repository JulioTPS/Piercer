using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> pieces;
    private List<GameObject> piecesBag = new List<GameObject>();
    public Vector3 spawnPosition = new Vector3(0, 21, 0);
    public int score = 0;
    public static GameManager Instance;
    public TextMeshPro scoreText;

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

    void Start() { }

    void SpawnRandomPiece()
    {
        if (piecesBag.Count == 0)
        {
            piecesBag.AddRange(pieces);
            for (int i = piecesBag.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (piecesBag[randomIndex], piecesBag[i]) = (piecesBag[i], piecesBag[randomIndex]);
            }
        }

        var piceSelected = piecesBag[0];
        Instantiate(
            piceSelected,
            spawnPosition - piceSelected.transform.GetChild(0).GetComponent<Piece>().spawnOffset,
            Quaternion.identity
        );
        piecesBag.RemoveAt(0);
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
