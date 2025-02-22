using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rottenmelon : MonoBehaviour
{
    public int damage = 10; // Damage dealt to player
    public float destroyDelay = 3f;

    void Start()
    {
        Destroy(gameObject, destroyDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StarterAssets.ThirdPersonController player = other.GetComponent<StarterAssets.ThirdPersonController>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("Player got hit by Rotten Melon! Damage: " + damage);
            }
            Destroy(gameObject); // Destroy the enemy after dealing damage
        }
    }
}
