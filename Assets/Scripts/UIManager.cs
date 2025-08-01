using System.Collections;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject gameSystems;

    public GameObject menu;
    public RectTransform menuRecTransform;
    public GameObject pauseButton;
    public GameObject playButton;
    public GameObject resetButton;
    public TextMeshProUGUI startButtonText;
    private bool isPlaying = false;

    void Awake()
    {
        Time.timeScale = 0f;
        pauseButton.SetActive(false);
    }

    void Start() { }

    void Update()
    {
        if (!isPlaying)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu.activeSelf)
            {
                OnGameResume();
            }
            else
            {
                OnGamePause();
            }
        }
    }

    public void OnGameExit()
    {
        Application.Quit();
    }

    public void OnGamePause()
    {
        Time.timeScale = 0f;
        pauseButton.SetActive(false);
        menu.SetActive(true);
        playButton.SetActive(true);
    }

    public void OnGameResume()
    {
        Time.timeScale = 1f;
        pauseButton.SetActive(true);
        menu.SetActive(false);
        playButton.SetActive(false);

        if (isPlaying == true)
            return;

        PieceController.Instance.OnPressStart();
        startButtonText.text = "Continue";
        resetButton.SetActive(true);
        isPlaying = true;
        menuRecTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200);
    }

    public void OnGameReset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        OnGameResume();
    }

    // private IEnumerator ResetGameCoroutine()
    // {
    //     Debug.Log("Resetting game...");
    //     Time.timeScale = 0f;
    //     isPlaying = false;
    //     pauseButton.SetActive(false);
    //     playButton.SetActive(true);
    //     menu.SetActive(true);
    //     startButtonText.text = "Start";
    //     resetButton.SetActive(false);

    //     Destroy(gameSystems);
    //     yield return null;

    //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    // }

    public void Logger()
    {
        Debug.Log("test");
    }
}
