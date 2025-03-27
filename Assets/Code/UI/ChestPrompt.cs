using StarterAssets;
using UnityEngine;

public class ChestPrompt : MonoBehaviour
{
    private GameObject promptUI;
    private Chest chest;

    void Awake()
    {
        // Look up the parent to find the Canvas/OpenChestText
        Transform parent = transform.parent;
        if (parent != null)
        {
            var canvas = parent.Find("Canvas");
            if (canvas != null)
            {
                var textObj = canvas.Find("OpenChestText");
                if (textObj != null)
                    promptUI = textObj.gameObject;
            }

            // Get the Chest component from the parent
            chest = parent.GetComponent<Chest>();
            if (chest == null)
            {
                Debug.LogWarning("ChestPrompt: Chest component not found on parent.");
            }
        }

        if (promptUI == null)
            Debug.LogWarning("ChestPrompt: Could not find OpenChestText under Canvas.");
    }

    void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (promptUI != null)
                promptUI.SetActive(true);

            if (chest != null)
                ThirdPersonController.Instance.SetInteractableChest(chest);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (promptUI != null)
                promptUI.SetActive(false);

            ThirdPersonController.Instance.ClearInteractableChest();
        }
    }
}
