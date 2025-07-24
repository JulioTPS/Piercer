using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public GameObject sunObject;
    public float dayTimeSpeed = 1f;
    private float currentDayAngle = 0f;
    private float timer = 0f;
    private const float DAY_TEMPERATURE = 5700f;
    private const float DAY_INTENSITY = 2f;
    private const float NIGHT_TEMPERATURE = 15000f;
    private const float NIGHT_INTENSITY = 0.5f;
    private const float FLIP_INTENSITY = 0.3f;
    private const float FLIP_TEMPERATURE = 10000f;
    private const float DAY_END_ANGLE = 90f;
    private const float DAY_START_ANGLE = -90f;
    private bool isNight = false;
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
        timer = currentDayAngle / dayTimeSpeed;
    }

    void Update()
    {
        timer += Time.deltaTime;
        currentDayAngle = dayTimeSpeed * timer;

        if (currentDayAngle > DAY_END_ANGLE)
        {
            Debug.Log("flipped");
            isNight = !isNight;
            currentDayAngle = DAY_START_ANGLE + currentDayAngle % DAY_END_ANGLE;
            timer = currentDayAngle / dayTimeSpeed;
        }
        if (isNight)
        {
            RenderSettings.sun.colorTemperature = NIGHT_TEMPERATURE;
            // RenderSettings.sun.intensity = NIGHT_INTENSITY;
            // RenderSettings.sun.colorTemperature = Mathf.Lerp(NIGHT_TEMPERATURE, FLIP_TEMPERATURE, Mathf.Abs(currentDayAngle) / DAY_END_ANGLE);
            RenderSettings.sun.intensity = Mathf.Lerp(NIGHT_INTENSITY, FLIP_INTENSITY, Mathf.Abs(currentDayAngle) / DAY_END_ANGLE);
        }
        else
        {
            RenderSettings.sun.colorTemperature = DAY_TEMPERATURE;
            // RenderSettings.sun.intensity = DAY_INTENSITY;
            // RenderSettings.sun.colorTemperature = Mathf.Lerp(DAY_TEMPERATURE, FLIP_TEMPERATURE, Mathf.Abs(currentDayAngle) / DAY_END_ANGLE);
            RenderSettings.sun.intensity = Mathf.Lerp(DAY_INTENSITY, FLIP_INTENSITY, Mathf.Abs(currentDayAngle) / DAY_END_ANGLE);
        }
        sunObject.transform.localRotation = Quaternion.Euler(0, currentDayAngle, 0);


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
