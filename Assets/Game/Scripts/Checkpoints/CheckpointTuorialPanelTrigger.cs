using UnityEngine;
using UnityEngine.Events;

public class CheckpointTuorialPanelTrigger : MonoBehaviour
{
    public UnityEvent OpenTrigger;
    public UnityEvent CloseTrigger; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OpenTrigger.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CloseTrigger.Invoke();
        }
    }


}
