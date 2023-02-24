using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpikesWithStop : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float speed;
    [SerializeField] private int currentTarget;

    private bool isActive = false;
    [SerializeField] PlayerDetector playerDetector;

    [SerializeField] private float waitTimeBeforeStart;


    void Update()
    {
        if (playerDetector.PlayerDetected && !isActive)
        {
            StartCoroutine(WaitBeforeStartCoroutine());
        }

        if (isActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentTarget].position, speed * Time.deltaTime);
        }
    }


    private void FixedUpdate()
    {
        if (transform.position == waypoints[currentTarget].position)
        {
            if (currentTarget == waypoints.Count - 1) //if we reached the last cp
            {
                currentTarget = 0;
            }
            else
            {
                if (currentTarget == 0) //if we reached the starting cp after completing the route
                {
                    isActive = false;
                    currentTarget = 1;
                }
                else
                {
                    currentTarget += 1;
                }
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Actions.OnPlayerDeath?.Invoke();
        }
    }

    IEnumerator WaitBeforeStartCoroutine()
    {
        yield return new WaitForSeconds(waitTimeBeforeStart);
        isActive = true;
    }




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for(int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i+1].position);
        }

        Gizmos.DrawLine(waypoints[waypoints.Count - 1].position, waypoints[0].position);
    }
}
