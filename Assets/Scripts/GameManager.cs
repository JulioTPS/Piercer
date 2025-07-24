using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public Transform sunTransform;
    public float dayTimeSpeed = 1f;
    private float currentDayAngle = 0f;
    private float timer;
    private const float SUN_DOWN_ANGLE = 50f;
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
        timer += Time.deltaTime;
        currentDayAngle = dayTimeSpeed * timer;

        Quaternion dayAngle = Quaternion.Euler(SUN_DOWN_ANGLE, currentDayAngle, 0);
        sunTransform.localRotation = dayAngle;

        if (currentDayAngle > 3600f)
        {
            currentDayAngle -= 3600f;
            timer = currentDayAngle / dayTimeSpeed;
        }

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
