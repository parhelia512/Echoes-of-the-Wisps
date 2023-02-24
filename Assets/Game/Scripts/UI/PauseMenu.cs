using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject victoryMenu;
    [SerializeField] private GameObject controlsMenu;



    [HideInInspector] public static bool isPaused;




    void Start()
    {
        pauseMenu.SetActive(false);
    }


    public void PauseGame()
    {
        isPaused = true;
        pauseMenu?.SetActive(true);
        Time.timeScale = 0f; //stops animation, updates etc.
        AudioListener.pause = true;
    }


    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu?.SetActive(false);
        settingsMenu?.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void GoToSettingsMenu()
    {
        settingsMenu?.SetActive(true);
        pauseMenu?.SetActive(false);
    }

    public void GoToControlsMenu()
    {
        controlsMenu?.SetActive(true);
        pauseMenu?.SetActive(false);
    }

    public void GoToMainMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("MenuScene");
    }


    public void QuitGame()
    {
        Application.Quit();
    }


    public void TogglePause(InputAction.CallbackContext context)
    {
        if (context.performed && !victoryMenu.activeInHierarchy)
        {
            if (isPaused)
            {
                if (controlsMenu.activeInHierarchy)
                {
                    controlsMenu.SetActive(false);
                }
                if (settingsMenu.activeInHierarchy)
                {
                    settingsMenu.SetActive(false);
                }
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

  




}
