
using UnityEngine;

public class ShowPlayerDetectorOnMovingTrap : MonoBehaviour
{
    private BoxCollider2D collider;


    void OnDrawGizmos()
    {
        collider = GetComponent<BoxCollider2D>();
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + new Vector3(collider.offset.x, collider.offset.y, 0f), new Vector3(collider.size.x, collider.size.y, 0f));
    }
}
