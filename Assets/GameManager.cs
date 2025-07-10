using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> pieces;
    public Vector3 spawnPosition = new Vector3(0, 21, 0);
    public int points = 0;
    public static GameManager Instance;

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
        if (pieces == null || pieces.Count == 0)
            return;

        var piceSelected = pieces[Random.Range(0, pieces.Count)];
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
}
