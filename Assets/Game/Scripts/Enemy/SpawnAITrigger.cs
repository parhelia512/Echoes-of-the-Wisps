
using UnityEngine;

public class SpawnAITrigger : MonoBehaviour
{
    private BoxCollider2D collider;
    [SerializeField] private EnemyMoving[] enemies;




   

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Actions.OnEnemySpawning?.Invoke();


            foreach (EnemyMoving enemy in enemies)
            {
                enemy.anim.SetTrigger("TriggerSpawn");
                enemy.smallAnim.SetTrigger("TriggerSpawn");
            }

            Destroy(gameObject);  //SHOULD BE ONE SHOT FOR THE SHAKE!!!!!!!! SHOULD NOT BE TRIGGERED WHEN WALKING BACK
        }
    }



    void OnDrawGizmos()
    {
        collider = GetComponent<BoxCollider2D>();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + new Vector3(collider.offset.x, collider.offset.y, 0f), new Vector3(collider.size.x, collider.size.y, 0f));
    }
}
