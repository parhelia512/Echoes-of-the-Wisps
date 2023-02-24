using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private int id;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Actions.OnPickupCollected?.Invoke(id);
        }
    }
}
