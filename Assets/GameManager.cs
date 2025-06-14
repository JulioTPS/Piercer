using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] piecesPrefabs;
    public Vector3 spawnPosition = new Vector3(-1, 20, 0);
    public int points = 0;
    public Time gameTime = new Time();

    public float spawnInterval = 3f;

    void Start() { }

    void SpawnRandomPiece()
    {
        Instantiate(piecesPrefabs[Random.Range(0, 6)], spawnPosition, Quaternion.identity);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // replace with any key you want
        {
            SpawnRandomPiece();
        }

        // timer += Time.deltaTime;
        // if (timer >= spawnInterval)
        // {

        //     timer = 0f;
        // }
    }
}
