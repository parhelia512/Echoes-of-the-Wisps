using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float speed;
    [SerializeField] private int currentTarget;


    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentTarget].position, speed * Time.deltaTime);
    }


    private void FixedUpdate()
    {
        if(transform.position == waypoints[currentTarget].position)
        {
            if(currentTarget == waypoints.Count - 1)
            {
                currentTarget = 0;
            }
            else
            {
                currentTarget += 1;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }

        Gizmos.DrawLine(waypoints[waypoints.Count - 1].position, waypoints[0].position);
    }
}
