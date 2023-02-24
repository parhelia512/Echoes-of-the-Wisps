using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerOnCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Actions.OnPlayerDeath?.Invoke();
        }
    }
}
