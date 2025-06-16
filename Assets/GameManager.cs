using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public struct SpawnEntry
{
    public GameObject piece;
    public Vector3 spawnOffset;
}


public class GameManager : MonoBehaviour
{

    public List<SpawnEntry> pieces;
    public Vector3 spawnPosition = new Vector3(-1, 20, 0);
    public int points = 0;
    public Time gameTime = new Time();

    public float spawnInterval = 3f;

    void Start() { }

    void SpawnRandomPiece()
    {
        if (pieces == null || pieces.Count == 0)
            return;

        var piceSelected = pieces[Random.Range(0, pieces.Count)];
        Instantiate(piceSelected.piece, spawnPosition + piceSelected.spawnOffset, Quaternion.identity);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // replace with any key you want
        {
            SpawnRandomPiece();
        }
    }
}
