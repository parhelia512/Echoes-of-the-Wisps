using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer myMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    public const string MIXER_MUSIC = "MusicVolume";
    public const string MIXER_SFX = "SFXVolume";

    //this script does the SAVING of the volume values

    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat(AudioManager.MUSIC_KEY, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(AudioManager.SFX_KEY, 1f);
    }

    private void OnDisable()
    {
        //here happens the saving
        PlayerPrefs.SetFloat(AudioManager.MUSIC_KEY, musicSlider.value);
        PlayerPrefs.SetFloat(AudioManager.SFX_KEY, sfxSlider.value);
    }

    void SetMusicVolume(float value)
    {
        myMixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value) * 20);

        //here happens the saving
        PlayerPrefs.SetFloat(AudioManager.MUSIC_KEY, musicSlider.value);

    }


    void SetSFXVolume(float value)
    {
        myMixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20);

        //here happens the saving
        PlayerPrefs.SetFloat(AudioManager.SFX_KEY, sfxSlider.value);
    }


}
