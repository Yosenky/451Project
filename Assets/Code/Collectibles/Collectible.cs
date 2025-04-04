using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int value = 1; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //TODO: does nothing right now
            Destroy(gameObject); 
        }
    }
}
