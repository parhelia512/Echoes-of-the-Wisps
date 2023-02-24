
using UnityEngine;

public class ActivateCastPos : MonoBehaviour
{
    [SerializeField] private Transform castPos;


    public void ActivateCast()
    {
        castPos.gameObject.SetActive(true);
    }
}
