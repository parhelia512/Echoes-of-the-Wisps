using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicContinue : MonoBehaviour
{
    public GameObject MenuMusic;

    void Awake()
    { 
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }


    void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("FinalesLvl"))
        {
            Destroy(MenuMusic);
        }
    }
}
