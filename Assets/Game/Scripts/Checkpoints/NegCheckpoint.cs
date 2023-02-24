using UnityEngine;

public class NegCheckpoint : MonoBehaviour
{
    [SerializeField] private AudioSource negCheckpointReachedSfx;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(Player.instance.CurrentMana > 0)
            {
                Player.instance.SetMana(0);
                negCheckpointReachedSfx.Play();
            }
        }
    }
}
