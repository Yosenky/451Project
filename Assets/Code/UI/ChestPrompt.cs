using StarterAssets;
using UnityEngine;
using TMPro;

public class ChestPrompt : MonoBehaviour
{
    private GameObject promptCanvas;
    private Chest chest;

    void Awake()
    {
        Transform parent = transform.parent;
        if (parent != null)
        {
            var canvas = parent.Find("Canvas");
            if (canvas != null)
            {
                promptCanvas = canvas.gameObject; 
            }

            chest = parent.GetComponent<Chest>();
            if (chest == null)
            {
                Debug.LogWarning("ChestPrompt: Chest component not found on parent.");
            }
        }

        if (promptCanvas == null)
            Debug.LogWarning("ChestPrompt: Could not find Canvas under Chest.");

        if (promptCanvas != null)
            promptCanvas.SetActive(false); // Hide at start
    }

    void Start()
    {
        if (promptCanvas != null)
            promptCanvas.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (promptCanvas != null)
            {
                promptCanvas.SetActive(true);

                TextMeshProUGUI promptText = promptCanvas.GetComponentInChildren<TextMeshProUGUI>();
                if (promptText != null)
                {
                    int currentPrice = GameController.Instance.chestPrice;
                    int playerMoney = UIController.Instance.money;

                    string moneyColor = (playerMoney >= currentPrice) ? "#FFFFFF" : "#FF0000";

                    promptText.text = $"<b>[E] Open Chest (<color={moneyColor}>${currentPrice}</color>)</b>";
                    promptText.color = Color.white;
                }
            }

            if (chest != null)
                ThirdPersonController.Instance.SetInteractableChest(chest);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (promptCanvas != null)
                promptCanvas.SetActive(false);

            ThirdPersonController.Instance.ClearInteractableChest();
        }
    }

    public void ForceUpdatePrompt()
    {
        if (promptCanvas != null && promptCanvas.activeSelf)
        {
            UpdateChestPromptText();
        }
    }

    void UpdateChestPromptText()
    {
        TextMeshProUGUI promptText = promptCanvas.GetComponentInChildren<TextMeshProUGUI>();
        if (promptText != null)
        {
            int currentPrice = GameController.Instance.chestPrice;
            int playerMoney = UIController.Instance.money;

            string moneyColor = (playerMoney >= currentPrice) ? "#FFFFFF" : "#FF0000";

            promptText.text = $"<b>[E] Open Chest (<color={moneyColor}>${currentPrice}</color>)</b>";
            promptText.color = Color.white;
        }
    }
}
