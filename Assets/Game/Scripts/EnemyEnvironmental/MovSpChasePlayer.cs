using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovSpChasePlayer : MonoBehaviour
{
    [SerializeField] private float speed;
    private Transform target;

    private bool isActive = false;
    [SerializeField] PlayerDetector playerDetector;

    [SerializeField] private float waitTimeBeforeStart;
    [SerializeField] private float autoDestroyTime;


    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        if (playerDetector.PlayerDetected && !isActive)
        {

            StartCoroutine(WaitBeforeStartCoroutine());
        }


    }


    private void FixedUpdate()
    {
        if (isActive)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Dead by moving spikes");
        }
    }

    IEnumerator WaitBeforeStartCoroutine()
    {
        yield return new WaitForSeconds(waitTimeBeforeStart);
        isActive = true;
        Destroy(gameObject, autoDestroyTime);


    }



}
