using UnityEngine;

public class UiSfxController : MonoBehaviour
{
    public static UiSfxController instance;


    [SerializeField] private AudioSource hoverSfx;
    [SerializeField] private AudioSource clickSfx;




    private void Start()
    {
        hoverSfx.ignoreListenerPause = true;
        clickSfx.ignoreListenerPause = true;
    }

    public void HoverSound()
    {
        hoverSfx.Stop();
        hoverSfx.Play();
    }

    public void ClickSound()
    {
        clickSfx.Stop();
        clickSfx.Play();
    }

}
