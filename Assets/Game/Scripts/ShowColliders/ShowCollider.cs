using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCollider : MonoBehaviour
{

    private BoxCollider2D collider;

    void OnDrawGizmos()
    {
        collider = GetComponent<BoxCollider2D>();
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + new Vector3(collider.offset.x, collider.offset.y, 0f), new Vector3(collider.size.x, collider.size.y, 0f));
    }
}