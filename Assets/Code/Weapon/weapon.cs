using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 10f;
    public float attackSpeed = 1f; // Attacks per second
    public float bulletSpeed = 20f;

    public GameObject ballPrefab;
    public Transform shootPoint;

    private float attackCooldown = 0f;

    void Update()
    {
        attackCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0) && attackCooldown <= 0f)
        {
            Shoot();
            attackCooldown = 1f / attackSpeed;
        }
    }

    void Shoot()
    {
        if (ballPrefab && shootPoint)
        {
            Debug.Log("Left Click Detected");
            GameObject ball = Instantiate(ballPrefab, shootPoint.position, shootPoint.rotation);
            Ball ballScript = ball.GetComponent<Ball>();
            if (ballScript)
            {
                ballScript.damage = damage;
                ballScript.speed = bulletSpeed;
            }

            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.velocity = shootPoint.forward * bulletSpeed;
            }
        }
    }
}
