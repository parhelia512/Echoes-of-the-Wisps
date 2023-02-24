using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform playerSpawnPoint;
    public Transform PlayerSpawnPoint => playerSpawnPoint;
    [SerializeField] private int manaValue;

    [SerializeField] private AudioSource checkpointReachedSfx;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Player.instance.CurrentMana < manaValue)
            {
                Player.instance.SetMana(manaValue);
            }
            Actions.OnCheckpointReached?.Invoke(playerSpawnPoint.position, manaValue);
            checkpointReachedSfx.Play();
        }
    }
}
