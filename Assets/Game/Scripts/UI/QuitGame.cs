using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void CloseGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }
}
