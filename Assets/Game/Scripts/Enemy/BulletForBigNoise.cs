
using UnityEngine;

public class BulletForBigNoise : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float speed;
    private Vector2 moveDirection;
    private Vector3 target;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveDirection = (target - transform.position).normalized;
    }


    private void FixedUpdate()
    {
        MoveBullet(moveDirection);
    }



    private void MoveBullet(Vector2 moveDirection)
    {
        rb.velocity = moveDirection * speed;
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Geist"))
        {
            Actions.OnGeistDeath?.Invoke();
        }


        if (other.CompareTag("Player"))
        {
            Actions.OnPlayerDeath?.Invoke();
        }
    }



    public void SetTarget(Vector3 _target)
    {
        target = _target;
    }

}
