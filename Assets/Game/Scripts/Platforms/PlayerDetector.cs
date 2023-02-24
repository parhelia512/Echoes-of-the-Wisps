using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private bool playerDetected;
    public bool PlayerDetected => playerDetected;


    private void Start()
    {
        playerDetected = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerDetected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerDetected = false;
        }
    }
}
