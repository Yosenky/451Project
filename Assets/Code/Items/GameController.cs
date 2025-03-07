using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum UpgradeType { IncreaseHealth, IncreaseDamage, IncreaseSpeed }

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public int chestPrice = 100;
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

        // Get all upgrade types and shuffle them.
        UpgradeType[] allUpgrades = (UpgradeType[])System.Enum.GetValues(typeof(UpgradeType));
        List<UpgradeType> upgradeList = new List<UpgradeType>(allUpgrades);
        for (int i = 0; i < upgradeList.Count; i++)
        {
            UpgradeType temp = upgradeList[i];
            int randomIndex = Random.Range(i, upgradeList.Count);
            upgradeList[i] = upgradeList[randomIndex];
            upgradeList[randomIndex] = temp;
        }

        // Instantiate 3 upgrade option panels.
        for (int i = 0; i < 3; i++)
        {
            UpgradeType selectedUpgrade = upgradeList[i];
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
                case UpgradeType.IncreaseHealth:
                    description = "Increase Health";
                    bgColor = Color.green;
                    break;
                case UpgradeType.IncreaseDamage:
                    description = "Increase Damage";
                    bgColor = Color.red;
                    break;
                case UpgradeType.IncreaseSpeed:
                    description = "Increase Speed";
                    bgColor = Color.blue;
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
        chestPrice += 10;

        switch (upgradeType)
        {
            case UpgradeType.IncreaseHealth:
                Debug.Log("Health upgraded!");
                break;
            case UpgradeType.IncreaseDamage:
                Debug.Log("Damage upgraded!");
                break;
            case UpgradeType.IncreaseSpeed:
                Debug.Log("Speed upgraded!");
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
