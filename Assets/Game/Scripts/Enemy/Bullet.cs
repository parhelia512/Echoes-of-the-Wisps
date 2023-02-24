
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float speed;
    private Vector2 moveDirection;
    private GameObject target;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player");
        moveDirection = (target.transform.position - transform.position).normalized * speed;
        rb.velocity = new Vector2(moveDirection.x, moveDirection.y);
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

}
