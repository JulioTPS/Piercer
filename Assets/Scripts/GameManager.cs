using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public int score = 0;

    [Header("Day/Night Cycle")]
    public GameObject sunObject;
    public float dayTimeSpeed = 1f;
    public bool enableDayNightCycle = true;
    public float dayTimeoffset = 3f;
    private float currentDayAngle = 0f;
    private float timer = 0f;
    private const float DAY_TEMPERATURE = 5700f;
    private const float DAY_INTENSITY = 3f;
    private const float NIGHT_TEMPERATURE = 15000f;
    private const float NIGHT_INTENSITY = 0.5f;
    private const float DAY_END_ANGLE = 90f;
    private const float DAY_START_ANGLE = -90f;
    private bool isNight = false;
    private Light sunRenderSettings;
    public UniversalAdditionalLightData directLightComponent;
    public Vector2 cloudSpeed = new Vector2(3f, 0.8f);

    public static GameManager Instance;
    public TextMeshPro scoreTMPro;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            sunRenderSettings = RenderSettings.sun;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        timer = currentDayAngle / dayTimeSpeed + dayTimeoffset;
    }

    void Update()
    {
        if (enableDayNightCycle)
        {
            timer += Time.deltaTime;
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
                }
                else
                {
                    sunRenderSettings.colorTemperature = DAY_TEMPERATURE;
                    sunRenderSettings.intensity = DAY_INTENSITY;
                }
            }
            sunObject.transform.localRotation = Quaternion.Euler(0, currentDayAngle, 0);

            if (timer > float.MaxValue - 10f * dayTimeSpeed)
            {
                timer = 0f;
            }
        }

        if (!directLightComponent)
        {
            directLightComponent = GetComponent<UniversalAdditionalLightData>();
        }
        directLightComponent.lightCookieOffset = cloudSpeed * Time.time;
    }

    public void AddScore(int _score)
    {
        score += _score * 100;

        scoreTMPro.text = "Score: " + score;
    }
}
