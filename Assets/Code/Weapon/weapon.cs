using UnityEngine;
using StarterAssets; 
using UnityEngine.EventSystems;
using System.Collections.Generic;



public class Weapon : MonoBehaviour
{
    
    public static Weapon Instance;

    public float damage = 10f;
    public float attackSpeed = 1f; // Attacks per second
    public float bulletSpeed = 20f;

    public GameObject ballPrefab;
    public GameObject boomerangPrefab;
    public GameObject explosivePrefab;

    public Transform shootPoint;

    private float attackCooldown = 0f;
    private Camera mainCamera;
    private WeaponType selectedWeaponType;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count == 0)
            {
                //Debug.Log("Raycast hit NOTHING");
            }
            else
            {
                foreach (var result in results)
                {
                    //Debug.Log("Raycast hit: " + result.gameObject.name);
                }
            }
        
        }
        

        // Don't shoot when paused
        if (Time.timeScale == 0f)
            return;

        attackCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0) && attackCooldown <= 0f)
        {
            switch (selectedWeaponType)
            {
                case WeaponType.Ball:
                    Shoot();
                    break;
                case WeaponType.Boomerang:
                    ShootBoomerang();
                    break;
                case WeaponType.Explosive:
                    ShootExplosiveBullet();
                    break;
            }
            //Debug.Log(selectedWeaponType);
            attackCooldown = 1f / attackSpeed;
        }
    }

    public void SetWeaponType(WeaponType type)
    {
        selectedWeaponType = type;
        //Debug.Log("Weapon set to: " + type);
    }

    void Shoot()
    {
        if (ballPrefab && shootPoint)
        {
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            Vector3 targetPoint = (Physics.Raycast(ray, out hit, 100f)) 
                                  ? hit.point 
                                  : ray.origin + ray.direction * 100f;
            Vector3 direction = (targetPoint - shootPoint.position).normalized;

            GameObject ball = Instantiate(ballPrefab, shootPoint.position, Quaternion.LookRotation(direction));
            Ball ballScript = ball.GetComponent<Ball>();
            if (ballScript != null)
            {
                ballScript.damage = damage;
                ballScript.speed = bulletSpeed;
            }

            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed;
            }
        }
        else
        {
            //Debug.LogWarning("BallPrefab or ShootPoint not assigned.");
        }
    }

    void ShootBoomerang()
    {
        if (boomerangPrefab && shootPoint)
        {
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            Vector3 direction = ray.direction;

            GameObject boomerang = Instantiate(boomerangPrefab, shootPoint.position, Quaternion.LookRotation(direction));
            BoomerangBullet bb = boomerang.GetComponent<BoomerangBullet>();
            if (bb != null)
            {
                bb.damage = damage;
                bb.shooter = shootPoint;
            }

            Rigidbody rb = boomerang.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed;
            }
        }
    }

    void ShootExplosiveBullet()
    {
        if (explosivePrefab && shootPoint)
        {
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            Vector3 direction = ray.direction;

            GameObject bullet = Instantiate(explosivePrefab, shootPoint.position, Quaternion.LookRotation(direction));
            ExplosiveBullet bulletScript = bullet.GetComponent<ExplosiveBullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = damage;
                bulletScript.speed = bulletSpeed;
            }

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed;
            }
        }
    }
}
