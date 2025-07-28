using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int score = 0;

    [Header("Day/Night Cycle")]
    public GameObject sunObject;
    public float dayTimeSpeed = 1f;
    public bool dayNightCycle = true;
    public float lightSkipTime = 3f;
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
    private Light sunRenderSettings;

    public static GameManager Instance;
    public TextMeshPro scoreTMPro;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            sunRenderSettings = RenderSettings.sun;
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
        if (dayNightCycle && timer > lightSkipTime)
        {
            currentDayAngle = dayTimeSpeed * timer;

            if (currentDayAngle > DAY_END_ANGLE)
            {
                isNight = !isNight;
                currentDayAngle = DAY_START_ANGLE + currentDayAngle % DAY_END_ANGLE;
                timer = currentDayAngle / dayTimeSpeed;
                if (isNight)
                {
                    sunRenderSettings.colorTemperature = NIGHT_TEMPERATURE;
                    sunRenderSettings.intensity = NIGHT_INTENSITY;
                    // sunRenderSettings.colorTemperature = Mathf.Lerp(NIGHT_TEMPERATURE, FLIP_TEMPERATURE, Mathf.Abs(currentDayAngle) / DAY_END_ANGLE);
                    // sunRenderSettings.intensity = Mathf.Lerp(NIGHT_INTENSITY, FLIP_INTENSITY, Mathf.Abs(currentDayAngle) / DAY_END_ANGLE);
                }
                else
                {
                    sunRenderSettings.colorTemperature = DAY_TEMPERATURE;
                    sunRenderSettings.intensity = DAY_INTENSITY;
                    // sunRenderSettings.colorTemperature = Mathf.Lerp(DAY_TEMPERATURE, FLIP_TEMPERATURE, Mathf.Abs(currentDayAngle) / DAY_END_ANGLE);
                    // sunRenderSettings.intensity = Mathf.Lerp(DAY_INTENSITY, FLIP_INTENSITY, Mathf.Abs(currentDayAngle) / DAY_END_ANGLE);
                }
            }
            sunObject.transform.localRotation = Quaternion.Euler(0, currentDayAngle, 0);
        }

        if (timer > float.MaxValue - 10f)
        {
            timer = 0f;
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
