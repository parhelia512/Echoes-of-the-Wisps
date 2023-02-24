using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform target { private get; set; }
    [SerializeField] private float range;

    private void Update()
    {
        if (!Player.instance.GeistIsActive) 
        {
            Destroy(gameObject);
        }

        var speed = 10.0f;
        if(target != null)
        {
            if (Vector2.Distance(transform.position, target.position) < range)
            {
                speed = 5.0f;
            }
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }

    }
}
