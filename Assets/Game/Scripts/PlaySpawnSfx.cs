using UnityEngine;

public class PlaySpawnSfx : MonoBehaviour
{
    [SerializeField] private AudioSource spawnSfx;


    public void PlaySpawnSound()
    {
        spawnSfx.Play();
    }

}
