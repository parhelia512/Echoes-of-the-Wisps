
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    


    private void Awake()
    {
        instance = this;
    }


    public void LoadScene(string level)
    {
        SceneManager.LoadScene(level);
    }


    public void QuitApplication()
    {
        Application.Quit();
    }



    public void ReloadScene(InputAction.CallbackContext context)
    {
        if (context.performed && !PauseMenu.isPaused)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


    IEnumerator Start()
    {
        yield return new WaitForSeconds(0);
    }


    private void Update()
    {
        Awake();
    }
}
