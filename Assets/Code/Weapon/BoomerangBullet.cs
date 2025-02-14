using UnityEngine;

public class BoomerangBullet : MonoBehaviour
{
    public float damage = 10f;
    public float speed = 20f;
    public float boomerangDelay = 2f;   // Time (in seconds) before the boomerang starts returning
    public float returnSpeed = 25f;     // Speed while returning
    public Transform shooter;           // Reference to the shooter (set this in the Inspector)
    public float destroyThreshold = 1f; // Distance from shooter at which the boomerang destroys itself

    private bool returning = false;
    private Rigidbody rb;
    private float timer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Fire forward from the current orientation
        rb.velocity = transform.forward * speed;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!returning && timer >= boomerangDelay)
        {
            returning = true;
        }

        if (returning && shooter != null)
        {
            // Calculate the direction from the boomerang back to the shooter
            Vector3 direction = (shooter.position - transform.position).normalized;
            rb.velocity = direction * returnSpeed;
            transform.rotation = Quaternion.LookRotation(direction);

            // Destroy the boomerang when it's close enough to the shooter
            if (Vector3.Distance(transform.position, shooter.position) < destroyThreshold)
            {
                Destroy(gameObject);
            }
        }
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
                enemyAI.Damaged(damage);
            }

            // Optionally destroy the boomerang on collision
            // Destroy(gameObject);
        }

    }
}
