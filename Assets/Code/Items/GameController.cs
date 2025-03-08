using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using StarterAssets;

public enum UpgradeType { IncreaseMaxHealth, IncreaseSpeed, IncreaseSprintSpeed }

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public int chestPrice = 0; // set to 0 for testing upgrades
    public GameObject upgradeInterface;
    public GameObject upgradeOptionPanelPrefab;
    public Transform upgradeOptionsContainer; // Container with Horizontal Layout Group
    public GameObject cancelButtonPrefab;       // Prefab for the cancel button
    private Chest currentChest;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (upgradeInterface != null)
            upgradeInterface.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void InteractChest(Chest chest)
    {
        currentChest = chest;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SetupUpgradeOptions();
        if (upgradeInterface != null)
            upgradeInterface.SetActive(true);
    }

    void SetupUpgradeOptions()
    {
        // Clear the upgrade options container.
        foreach (Transform child in upgradeOptionsContainer)
            Destroy(child.gameObject);

        // Instantiate the cancel button as a child of the overall upgradeInterface,
        // so it’s not affected by the Horizontal Layout Group.
        if (cancelButtonPrefab != null)
        {
            GameObject cancelObj = Instantiate(cancelButtonPrefab, upgradeInterface.transform);
            Button cancelBtn = cancelObj.GetComponent<Button>();
            cancelBtn.onClick.AddListener(CancelUpgrade);

            // Position it at the top-right corner of the upgradeInterface.
            RectTransform cancelRect = cancelObj.GetComponent<RectTransform>();
            cancelRect.anchorMin = new Vector2(1, 1);
            cancelRect.anchorMax = new Vector2(1, 1);
            cancelRect.pivot = new Vector2(1, 1);
            cancelRect.anchoredPosition = new Vector2(-10, -10); // adjust padding as needed
        }

        // Get container dimensions.
        RectTransform containerRect = upgradeOptionsContainer.GetComponent<RectTransform>();
        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;

        // Define upgrade options.
        UpgradeType[] allUpgrades = { UpgradeType.IncreaseMaxHealth, UpgradeType.IncreaseSpeed, UpgradeType.IncreaseSprintSpeed };

        // Instantiate 3 upgrade option panels.
        foreach (UpgradeType selectedUpgrade in allUpgrades)
        {
            GameObject optionObj = Instantiate(upgradeOptionPanelPrefab, upgradeOptionsContainer);
            UpgradeOptionPanel optionPanel = optionObj.GetComponent<UpgradeOptionPanel>();

            // Set size: 30% of container's width and 70% of container's height.
            RectTransform optionRect = optionObj.GetComponent<RectTransform>();
            optionRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, containerWidth * 0.3f);
            optionRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, containerHeight * 0.7f);

            Sprite icon = null;
            string description = "";
            Color bgColor = Color.white;

            switch (selectedUpgrade)
            {
                case UpgradeType.IncreaseMaxHealth:
                    description = "Increase Max Health";
                    bgColor = Color.green;
                    break;
                case UpgradeType.IncreaseSpeed:
                    description = "Increase Speed";
                    bgColor = Color.blue;
                    break;
                case UpgradeType.IncreaseSprintSpeed:
                    description = "Increase Sprint Speed";
                    bgColor = Color.yellow;
                    break;
            }

            optionPanel.Setup(icon, description, () => UpgradeSelected(selectedUpgrade), bgColor);
        }
    }

    void UpgradeSelected(UpgradeType upgradeType)
    {
        if (UIController.Instance.money < chestPrice)
        {
            Debug.Log("Not enough money to upgrade!");
            return;
        }

        UIController.Instance.AddMoney(-chestPrice);
        //chestPrice += 10;

        switch (upgradeType)
        {
            case UpgradeType.IncreaseMaxHealth:
                ThirdPersonController.Instance.UpgradePlayer("maxhealth", 10);
                Debug.Log("Max Health increased!");
                break;
            case UpgradeType.IncreaseSpeed:
                ThirdPersonController.Instance.UpgradePlayer("movespeed", 1);
                Debug.Log("Speed increased!");
                break;
            case UpgradeType.IncreaseSprintSpeed:
                ThirdPersonController.Instance.UpgradePlayer("sprintspeed", .2f);
                Debug.Log("Sprint Speed increased!");
                break;
        }

        if (currentChest != null)
        {
            Destroy(currentChest.gameObject);
            currentChest = null;
        }
        if (upgradeInterface != null)
            upgradeInterface.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CancelUpgrade()
    {
        if (currentChest != null)
        {
            currentChest.CancelAnimation();
            currentChest = null;
        }
        if (upgradeInterface != null)
            upgradeInterface.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
