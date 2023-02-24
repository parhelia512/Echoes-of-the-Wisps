using UnityEngine;

public class ButtonSfx : MonoBehaviour
{
    [SerializeField] private AudioSource hoverSfx;
    [SerializeField] private AudioSource clickSfx;


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
