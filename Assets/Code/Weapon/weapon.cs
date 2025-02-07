using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 10f;
    public float attackSpeed = 1f; // Attacks per second
    public float bulletSpeed = 20f;

    public GameObject ballPrefab;
    public GameObject boomerangPrefab; 

    public Transform shootPoint;

    private float attackCooldown = 0f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        attackCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0) && attackCooldown <= 0f)
        {
            Shoot();
            ShootBoomerang();
            attackCooldown = 1f / attackSpeed;
        }
    }

    void Shoot()
    {
        if (ballPrefab && shootPoint)
        {
            // Cast a ray from the center of the screen
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.origin + ray.direction * 100f;
            }

            // Calculate direction
            Vector3 direction = (targetPoint - shootPoint.position).normalized;

            // Instantiate the ball
            GameObject ball = Instantiate(ballPrefab, shootPoint.position, Quaternion.LookRotation(direction));
            Ball ballScript = ball.GetComponent<Ball>();
            if (ballScript)
            {
                ballScript.damage = damage;
                ballScript.speed = bulletSpeed;
            }

            // Set velocity
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.velocity = direction * bulletSpeed;
            }
        }
        else
        {
            Debug.LogWarning("BallPrefab or ShootPoint not assigned.");
        }
    }
    void ShootBoomerang()
{
    if (boomerangPrefab && shootPoint)
    {
        GameObject boomerang = Instantiate(boomerangPrefab, shootPoint.position, shootPoint.rotation);
        BoomerangBullet bb = boomerang.GetComponent<BoomerangBullet>();
        if (bb != null)
        {
            bb.damage = damage;
            bb.shooter = shootPoint;  // Set the shooter so the boomerang knows where to return
        }
    }
}
}
