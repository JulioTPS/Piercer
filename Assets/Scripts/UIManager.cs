using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject menu;
    public GameObject pauseButton;
    public GameObject playButton;
    public TextMeshProUGUI startButtonText;
    private bool isPlaying = false;

    void awake(){
        Time.timeScale = 0f;
        pauseButton.SetActive(false);
    }

    void Start()
    {
        
    }

    void Update()
    {
    }

    public void OnGameExit(){
        Application.Quit();
    }
    public void OnGamePause(){
        Time.timeScale = 0f;
        pauseButton.SetActive(false);
        menu.SetActive(true);
        playButton.SetActive(true);
    }
    public void OnGameResume(){
        if (isPlaying == false){
            startButtonText.text = "Continue";
            isPlaying = true;
        }
        Time.timeScale = 1f;
        pauseButton.SetActive(true);
        menu.SetActive(false);
        playButton.SetActive(false);
    }

    public void Logger(){
        Debug.Log("test");
    }
}
