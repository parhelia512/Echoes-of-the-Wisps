using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsMenu : MonoBehaviour
{
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject pauseMenu;

    public void GoToPauseMenu()
    {
        pauseMenu?.SetActive(true);
        controlsMenu?.SetActive(false);
    }

}

