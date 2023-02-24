using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillGeistOnCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Geist"))
        {
            Actions.OnGeistDeath?.Invoke();
        }
    }
}
