using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whoPlayer;
    public float health;

    public GameObject damageText3DPrefab;
    public Transform damageTextSpawnPoint;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public int attackdamage;

    // States to see if any of these conditions are met
    public float sightRange, attackRange;
    public bool playerInSightRange, PlayerInAttackRange;

    public Animator animator;
    public int moneyReward = 10; // Money reward for killing this enemy

    private void Awake()
    {
        player = GameObject.Find("PlayerCapsule").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whoPlayer);
        PlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whoPlayer);

        if (playerInSightRange && !PlayerInAttackRange) Chase();
        if (playerInSightRange && PlayerInAttackRange) Attack();
        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f);
    }

    private void Chase()
    {
        agent.SetDestination(player.position);
    }

    private void Attack()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        if (!alreadyAttacked)
        {
            animator.SetTrigger("attack");

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, attackRange, whoPlayer))
            {
                Debug.DrawRay(transform.position, transform.forward * attackRange, Color.red, 1f);

                ThirdPersonController playerHealth = hit.collider.GetComponent<ThirdPersonController>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackdamage);
                }
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void Damaged(float damage)
    {
        health -= damage;
        animator.SetTrigger("damaged");

        ShowDamageNumber(damage);

        if (health <= 0)
        {
            agent.isStopped = true;
            alreadyAttacked = true;
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            animator.SetBool("isDead", true);
            GivePlayerMoney();
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

    private void GivePlayerMoney()
    {
        if (player != null)
        {
            ThirdPersonController playerController = player.GetComponent<ThirdPersonController>();
            if (playerController != null)
            {
                playerController.AddMoney(moneyReward);
                Debug.Log("Player received $" + moneyReward + " for killing EnemyAI.");
            }
        }
    }

    private void destroyenemy()
    {
        Destroy(gameObject);
    }
}
