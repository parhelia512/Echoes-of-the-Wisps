using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockTp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CheckpointController.instance.hasUnlockedTp = true;
        }
    }
}
