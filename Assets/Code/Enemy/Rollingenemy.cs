using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using StarterAssets;

public class Rollingenemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whoPlayer;
    public float health;

    public GameObject damageText3DPrefab;
    public Transform damageTextSpawnPoint;

    // Charging & Exploding
    public float chargeSpeed = 10f;
    public float explosionRadius = 3f;
    public int explosionDamage = 50;
    public GameObject Goomba, head, Explosion;

    // State Checks
    public float detectionRange = 5f; // Distance at which the enemy detects the player
    private bool isCharging = false;
    public Animator animator;
    public int moneyReward = 10;

    private AudioSource source;

    private void Awake()
    {
        player = GameObject.Find("PlayerCapsule")?.transform; // Ensure correct player reference
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Explosion.SetActive(false);
        Goomba.SetActive(true);
        head.SetActive(true);

        source = GetComponent<AudioSource>();
    }
    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        Collider[] playerColliders = playerObj.GetComponentsInChildren<Collider>(); // Get all player colliders
        Collider enemyCollider = GetComponent<Collider>();

        foreach (Collider playerCollider in playerColliders)
        {
            Physics.IgnoreCollision(playerCollider, enemyCollider);
        }
    }

    void Update()
    {
        if (player == null) return; // Prevent null reference errors

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Chase the player if not charging
        if (!isCharging)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }

        // Start charging if within detection range
        if (distanceToPlayer <= detectionRange && !isCharging)
        {
            StartCoroutine(ChargeAtPlayer());
        }

        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f);
    }

    private IEnumerator ChargeAtPlayer()
    {
        isCharging = true;
        agent.speed = chargeSpeed;

        float chargeDuration = 3f;
        float timer = 0f;

        // Keep updating the destination while charging
        while (timer < chargeDuration)
        {
            if (player == null) break; // Prevent errors if player disappears
            agent.SetDestination(player.position);
            timer += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = true; // Stop movement safely
        Explode();
    }

    private void Explode()
    {

        exploding();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                ThirdPersonController playerHealth = hit.GetComponent<ThirdPersonController>();
                if (playerHealth != null)
                {
                    
                    playerHealth.TakeDamage(explosionDamage);
                }
            }
        }

        
        Goomba.SetActive(false);
        head.SetActive(false);
    }
    public void exploding()
    {
        Explosion.SetActive(true);
        source.Play();
        StartCoroutine(WaitForExplosionAndDestroy());
    }

    public void Damaged(float damage)
    {
        health -= damage;
        ShowDamageNumber(damage);

        if (health <= 0)
        {
            GivePlayerMoney();
            Explosion.SetActive(true);
            Goomba.SetActive(false);
            head.SetActive(false);
            source.Play();
            StartCoroutine(WaitForExplosionAndDestroy());
        }
    }

    private void ShowDamageNumber(float damage)
    {
        if (damageText3DPrefab != null && damageTextSpawnPoint != null)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f));
            GameObject dmg = Instantiate(damageText3DPrefab, damageTextSpawnPoint.position + randomOffset, Quaternion.identity);
            dmg.GetComponent<DamageNumber3D>().SetDamage(damage);
        }
    }
    private IEnumerator WaitForExplosionAndDestroy()
    {
        // Wait for the explosion effect to finish 
        yield return new WaitForSeconds(Explosion.GetComponent<ParticleSystem>().main.duration);

        // destroy the enemy after the explosion effect is done
        Destroy(gameObject);
    }
    private void GivePlayerMoney()
    {
        if (player != null)
        {
            ThirdPersonController playerController = player.GetComponent<ThirdPersonController>();
            if (playerController != null)
            {
                playerController.AddMoney(moneyReward);
                Debug.Log("Player received $" + moneyReward + " for killing RollingEnemy.");
            }
        }
    }
}
