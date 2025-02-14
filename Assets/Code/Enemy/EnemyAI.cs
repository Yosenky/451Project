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

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public int attackdamage;

    //states to see if any of these conditions are met and I am able to change these values 
    public float sightRange, attackRange;
    public bool playerInSightRange, PlayerInAttackRange;

    public Animator animator;


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
            //Attack code that could be change on different enemies! 
            animator.SetTrigger("attack");

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, attackRange, whoPlayer))
            {
               
                //PLayer health component is not yet implemented, so I am using debug logs to state if the player is being hit
                Debug.DrawRay(transform.position, transform.forward * attackRange, Color.red, 1f);
                
                
                ThirdPersonController playerHealth = hit.collider.GetComponent<ThirdPersonController>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackdamage); // Apply the attack damage
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
    private void Damaged(int damage)
    {
        health -= damage;
        animator.SetTrigger("damaged");
        if (health == 0)
        {
            animator.SetTrigger("death");
            destroyenemy();
        }
    }
    private void destroyenemy()
    {
        
        Destroy(gameObject); 
    }

}
