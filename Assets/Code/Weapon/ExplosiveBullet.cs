using UnityEngine;

public class ExplosiveBullet : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float damage = 15f;
    public float speed = 20f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyAI enemyAI = collision.gameObject.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.Damaged(Mathf.RoundToInt(damage));
            }
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.CompareTag("Enemy"))
            {
                EnemyAI enemyAI = nearbyObject.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.Damaged(Mathf.RoundToInt(damage));
                }
            }
        }
        Destroy(gameObject);
    }
}
