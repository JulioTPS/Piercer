using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject menu;
    public GameObject pauseButton;
    public GameObject playButton;
    public GameObject resetButton;
    public GameObject exitButton;
    public TextMeshProUGUI attributionText;
    public TextAsset attributionTextFile;
    public TextMeshProUGUI startButtonText;
    private bool isPlaying = false;

    void Awake()
    {
        Time.timeScale = 0f;
        pauseButton.SetActive(false);

#if UNITY_WEBGL
        exitButton.SetActive(false);
#endif
    }

    void Start()
    {
        if (attributionText == null)
        {
            Debug.LogError("Credits text file is not assigned in the UIManager.");
            return;
        }
        attributionText.richText = true;
        attributionText.text = attributionTextFile.text;
    }

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
    }

    public void OnGameReset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        OnGameResume();
    }
}
