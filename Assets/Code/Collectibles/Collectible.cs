using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int value = 1; 
    public string collectibleType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.Instance.CollectCollectible(collectibleType);
            Destroy(gameObject); 
        }
    }
}
