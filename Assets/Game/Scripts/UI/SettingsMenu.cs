using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject pauseMenu;





    public void GoToPauseMenu()
    {
        pauseMenu?.SetActive(true);
        settingsMenu?.SetActive(false);
    }

}
