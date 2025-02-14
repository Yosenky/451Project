using UnityEngine;

public class Ball : MonoBehaviour
{
    public float damage = 10f;
    public float speed = 20f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        // Destroy after 5 seconds to prevent clutter
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
       if (collision.gameObject.CompareTag("Enemy"))
        {
            // Get the EnemyAI script on the enemy
            EnemyAI enemyAI = collision.gameObject.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                // Call the Damaged method with the boomerang’s damage
                // Note: enemyAI.Damaged() expects an int,
                //       so we cast if boomerang damage is a float
                enemyAI.Damaged(Mathf.RoundToInt(damage));
            }

          
            
        }
        Destroy(gameObject);

    }
}
