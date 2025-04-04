using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine.AI; // Needed for NavMesh
using System.Collections;

public enum UpgradeType { IncreaseMaxHealth, IncreaseSpeed, IncreaseSprintSpeed, IncreaseDamage, IncreaseAttackSpeed, IncreaseMaxJumps, IncreaseHealthRegenRate, IncreaseJumpHeight,RareIncreaseMaxHealth, RareIncreaseSpeed, RareIncreaseSprintSpeed, RareIncreaseDamage, RareIncreaseAttackSpeed }

public class GameController : MonoBehaviour
{

    [Header("Chest Spawning")]
    public GameObject chestPrefab;
    public float chestSpawnInterval = 25f;
    public float chestSpawnRadius = 25f; // how far from center can chests spawn
    public Transform levelCenter; // use an empty GameObject as a spawn origin


    public static GameController Instance;
    public int chestPrice = 0; // set to 0 for testing upgrades
    public GameObject upgradeInterface;
    public GameObject upgradeOptionPanelPrefab;
    public Transform upgradeOptionsContainer; // Container with Horizontal Layout Group
    public GameObject cancelButtonPrefab;       // Prefab for the cancel button
    private Chest currentChest;
    public Sprite maxHealthUpgradeIcon;
    public Sprite damageUpgradeIcon;
    public Sprite moveSpeedUpgradeIcon;
    public Sprite sprintSpeedUpgradeIcon;
    public Sprite attackSpeedUpgradeIcon;
    public Sprite maxJumpsUpgradeIcon;
    public Sprite healthRegenUpgradeIcon;
    public Sprite jumpHeightUpgradeIcon;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (upgradeInterface != null)
            upgradeInterface.SetActive(false);

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void Start()
    {
        StartCoroutine(SpawnChestRoutine());
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
        // Clear existing upgrade options.
        foreach (Transform child in upgradeOptionsContainer)
            Destroy(child.gameObject);

        // Instantiate the cancel button as a child of the overall upgradeInterface,
        // so it isn’t affected by the Horizontal Layout Group.
        if (cancelButtonPrefab != null)
        {
            GameObject cancelObj = Instantiate(cancelButtonPrefab, upgradeInterface.transform);
            Button cancelBtn = cancelObj.GetComponent<Button>();
            cancelBtn.onClick.AddListener(CancelUpgrade);

            // Position at the top-right of the upgradeInterface.
            RectTransform cancelRect = cancelObj.GetComponent<RectTransform>();
            cancelRect.anchorMin = new Vector2(1, 1);
            cancelRect.anchorMax = new Vector2(1, 1);
            cancelRect.pivot = new Vector2(1, 1);
            cancelRect.anchoredPosition = new Vector2(-10, -10); // adjust as needed
        }

        // CHANGED: Prepare a list of all available upgrade types and shuffle it.
        UpgradeType[] allUpgrades = 
        { 
            UpgradeType.IncreaseMaxHealth, UpgradeType.IncreaseSpeed, UpgradeType.IncreaseSprintSpeed, 
            UpgradeType.IncreaseDamage, UpgradeType.IncreaseAttackSpeed, UpgradeType.IncreaseMaxJumps, 
            UpgradeType.IncreaseHealthRegenRate, UpgradeType.IncreaseJumpHeight,UpgradeType.RareIncreaseMaxHealth, UpgradeType.RareIncreaseSpeed, UpgradeType.RareIncreaseSprintSpeed, 
            UpgradeType.RareIncreaseDamage, UpgradeType.RareIncreaseAttackSpeed,
        };

        List<UpgradeType> availableUpgrades = new List<UpgradeType>(allUpgrades);
        List<UpgradeType> selectedUpgrades = new List<UpgradeType>();

        // Select 3 unique upgrades using weighted random selection.
        for (int i = 0; i < 3; i++)
        {
            float totalWeight = 0f;
            foreach (UpgradeType upgrade in availableUpgrades)
            {
                totalWeight += GetUpgradeWeight(upgrade);
            }

            float randomValue = Random.Range(0f, totalWeight);
            float accum = 0f;
            UpgradeType chosen = availableUpgrades[0];

            foreach (UpgradeType upgrade in availableUpgrades)
            {
                accum += GetUpgradeWeight(upgrade);
                if (randomValue <= accum)
                {
                    chosen = upgrade;
                    break;
                }
            }
            selectedUpgrades.Add(chosen);
            availableUpgrades.Remove(chosen);
        }

        // Get container dimensions.
        RectTransform containerRect = upgradeOptionsContainer.GetComponent<RectTransform>();
        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;

        for (int i = 0; i < 3; i++)
        {
            UpgradeType selectedUpgrade = selectedUpgrades[i];
            GameObject optionObj = Instantiate(upgradeOptionPanelPrefab, upgradeOptionsContainer);
            UpgradeOptionPanel optionPanel = optionObj.GetComponent<UpgradeOptionPanel>();

            // Set size: 30% of container's width and 70% of container's height.
            RectTransform optionRect = optionObj.GetComponent<RectTransform>();
            optionRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, containerWidth * 0.3f);
            optionRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, containerHeight * 0.7f);

            Sprite icon = null;
            string description = "";
            Color bgColor = Color.blue; // default color

            switch (selectedUpgrade)
            {
                case UpgradeType.IncreaseMaxHealth:
                    icon = maxHealthUpgradeIcon;
                    description = "Increase Max Health";
                    bgColor = Color.blue;
                    break;
                case UpgradeType.IncreaseSpeed:
                    icon = moveSpeedUpgradeIcon;
                    description = "Increase Speed";
                    bgColor = Color.blue;
                    break;
                case UpgradeType.IncreaseSprintSpeed:
                    icon = sprintSpeedUpgradeIcon;
                    description = "Increase Sprint Speed";
                    bgColor = Color.blue;
                    break;
                case UpgradeType.IncreaseDamage:
                    icon = damageUpgradeIcon;
                    description = "Increase Damage";
                    bgColor = Color.blue;
                    break;
                case UpgradeType.IncreaseAttackSpeed:
                    icon = attackSpeedUpgradeIcon;
                    description = "Increase Attack Speed";
                    bgColor = Color.blue;
                    break;
                case UpgradeType.IncreaseMaxJumps:
                    icon = maxJumpsUpgradeIcon;
                    description = "Increase Max Jumps";
                    bgColor = Color.blue;
                    break;
                case UpgradeType.IncreaseHealthRegenRate:
                    icon = healthRegenUpgradeIcon;
                    description = "Increase Health Regen";
                    bgColor = Color.blue;
                    break;
                case UpgradeType.IncreaseJumpHeight:
                    icon = jumpHeightUpgradeIcon;
                    description = "Increase Jump Height";
                    bgColor = Color.blue;
                    break;
                case UpgradeType.RareIncreaseMaxHealth:
                    icon = maxHealthUpgradeIcon;
                    description = "Increase Max Health Significantly";
                    bgColor = Color.yellow;
                    break;
                case UpgradeType.RareIncreaseSpeed:
                    icon = moveSpeedUpgradeIcon;
                    description = "Increase Speed Significantly";
                    bgColor = Color.yellow;
                    break;
                case UpgradeType.RareIncreaseSprintSpeed:
                    icon = sprintSpeedUpgradeIcon;
                    description = "Increase Sprint Speed Significantly";
                    bgColor = Color.yellow;
                    break;
                case UpgradeType.RareIncreaseDamage:
                    icon = damageUpgradeIcon;
                    description = "Increase Damage Significantly";
                    bgColor = Color.yellow;
                    break;
                case UpgradeType.RareIncreaseAttackSpeed:
                    icon = attackSpeedUpgradeIcon;
                    description = "Increase Attack Speed Significantly";
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
        chestPrice += 10;

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
            case UpgradeType.IncreaseDamage:
                ThirdPersonController.Instance.UpgradePlayer("damage", 2f);
                Debug.Log("Damage increased!");
                break;
            case UpgradeType.IncreaseAttackSpeed:
                ThirdPersonController.Instance.UpgradePlayer("attackspeed", 0.5f);
                Debug.Log("Attack Speed increased!");
                break;
            case UpgradeType.IncreaseMaxJumps:
                ThirdPersonController.Instance.UpgradePlayer("maxjumps", 1f);
                Debug.Log("Max Jumps increased!");
                break;
            case UpgradeType.IncreaseHealthRegenRate:
                ThirdPersonController.Instance.UpgradePlayer("healthregen", 0.5f);
                Debug.Log("Health Regen increased!");
                break;
            case UpgradeType.IncreaseJumpHeight:
                ThirdPersonController.Instance.UpgradePlayer("jumpheight", 0.5f);
                Debug.Log("Jump Height increased!");
                break;
            case UpgradeType.RareIncreaseMaxHealth:
                ThirdPersonController.Instance.UpgradePlayer("maxhealth", 20);
                Debug.Log("Rare Max Health increased!");
                break;
            case UpgradeType.RareIncreaseSpeed:
                ThirdPersonController.Instance.UpgradePlayer("movespeed", 1.5f);
                Debug.Log("Rare Speed increased!");
                break;
            case UpgradeType.RareIncreaseSprintSpeed:
                ThirdPersonController.Instance.UpgradePlayer("sprintspeed", .4f);
                Debug.Log("Rare Sprint Speed increased!");
                break;
            case UpgradeType.RareIncreaseDamage:
                ThirdPersonController.Instance.UpgradePlayer("damage", 3f);
                Debug.Log("Damage increased!");
                break;
            case UpgradeType.RareIncreaseAttackSpeed:
                ThirdPersonController.Instance.UpgradePlayer("attackspeed", 0.75f);
                Debug.Log("Attack Speed increased!");
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
        Debug.Log("Locking cursor due to upgrade");
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
        Debug.Log("Locking cursor due to canceling upgrade");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    IEnumerator SpawnChestRoutine()
    {
        while (true)
        {
            SpawnChestOnNavMesh();
            yield return new WaitForSecondsRealtime(chestSpawnInterval);

        }
    }

    void SpawnChestOnNavMesh()
    {
        if (chestPrefab == null || levelCenter == null) return;

        Vector3 randomPoint = levelCenter.position + Random.insideUnitSphere * chestSpawnRadius;
        randomPoint.y = levelCenter.position.y;

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            GameObject chest = Instantiate(chestPrefab, hit.position, Quaternion.identity);
            print("Spawned chest!");
            Collider chestCollider = chest.GetComponent<Collider>();
            if (chestCollider != null)
            {
                // Raise the chest so its bottom touches the ground
                float bottomY = chestCollider.bounds.min.y;
                float desiredY = hit.position.y;
                float offset = desiredY - bottomY;

                chest.transform.position += new Vector3(0, offset, 0);
            }
        }
    }

    private float GetUpgradeWeight(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.RareIncreaseMaxHealth:
                return 0.5f;
            case UpgradeType.RareIncreaseSpeed:
                return 0.5f;
            case UpgradeType.RareIncreaseSprintSpeed:
                return 0.5f;
            case UpgradeType.RareIncreaseDamage:
                return 0.5f;
            case UpgradeType.RareIncreaseAttackSpeed:
                return 0.5f;
            default:
                return 1.0f;  // Normal upgrade.
        }
    }
}
