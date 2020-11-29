using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    public GameObject pauseMenu;
    public GameObject victoryScreen;

    private bool isPaused = false;
    // Start is called before the first frame update

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1;
        AudioListener.volume = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            if (isPaused)
                ActivatePauseMenu();
            else
                DeactivatePauseMenu();
        }
    }

    public void ActivateVictoryScreen()
    {
        AudioManager.Instance.PlayOneShot("Victory");
        victoryScreen.SetActive(true);
    }

    private void ActivatePauseMenu()
    {
        Time.timeScale = 0;
        AudioListener.volume = 0.2f;
        pauseMenu.SetActive(true);
    }

    private void DeactivatePauseMenu()
    {
        Time.timeScale = 1;
        AudioListener.volume = 1f;
        pauseMenu.SetActive(false);
    }


    // Functions called by buttons
    public void ResumeButton()
    {
        isPaused = false;
    }
    public void RestartButton()
    {
        Time.timeScale = 1;
        AudioListener.volume = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void QuitButton()
    {
        Application.Quit();
    }
}
