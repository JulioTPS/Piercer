using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public static GameManager Instance;
    public TextMeshPro scoreTMPro;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddScore(1);
        }
    }

    public void AddScore(int _score)
    {
        score += _score * 100;

        scoreTMPro.text = "Score: " + score;
    }
}
