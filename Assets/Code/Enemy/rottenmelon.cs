using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rottenmelon : MonoBehaviour
{
    public int damage;
    public float destroyDelay = 3f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyDelay);
    }

   private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StarterAssets.ThirdPersonController playerHealth = other.GetComponent<StarterAssets.ThirdPersonController>();
            if(playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Player got hit by Rotten Melon: " + damage);
            }
            Destroy(gameObject);
        }
    }
}
