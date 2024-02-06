using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

    public static PauseManager instance;

    public GameObject pauseMenu;

    public GameObject pauseButton;

    public PlayerController playerController;

    public float totalTimeRestart = 0.0f;

    public static float lastRestartTime;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResumeGame();
    }


    void Start()
    {
        if (playerController != null)
        {
            pauseMenu.SetActive(playerController.isPaused);
        }
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        StartCoroutine(DelayedPause());
    }

    private IEnumerator DelayedPause()
    {
        yield return new WaitForEndOfFrame();
        bool isPaused = Time.timeScale == 0;
        Time.timeScale = isPaused ? 1 : 0;
        pauseMenu.SetActive(!isPaused);
        pauseButton.SetActive(false);

        if (playerController != null)
        {
            playerController.isPaused = !isPaused;
        }
    }

    public void ResumeGame()
    {
        playerController.isPaused = false;
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        lastRestartTime = Time.time;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ResumeGame();
    }

    public float GetTotalTimeRestart()
    {
        return totalTimeRestart;
    }
}
