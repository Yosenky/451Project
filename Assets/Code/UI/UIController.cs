using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI statsText;
    private float elapsedTime = 0f;
    private string difficulty = "Easy";
    private Color textColor = Color.green;
    private float originalHealthBarWidth;


    public TextMeshProUGUI moneyText;
    public int money = 0;

    public RectTransform healthBarFill;
    private float maxHealthWidth;
    private int maxHealth;

    public RectTransform statsPanel;               // The full panel containing statsText + toggleText
    public TextMeshProUGUI toggleText;             // The "Press Tab to Hide/Expand" text
    public GameObject statsTextObject;             // The actual text box showing stats
    private Vector2 expandedSize = new Vector2(325, 400);
    private Vector2 collapsedSize = new Vector2(325, 50);
    private bool statsVisible = true;

    // collectible panels
    // public RectTransform collectiblePanel;
    // public TextMeshProUGUI collectibleText;
    // public GameObject collectibleIconPrefab;


    // Dictionary to store initial stat values
    private Dictionary<string, float> initialStats = new Dictionary<string, float>();
    public GameObject mainUICanvas;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        originalHealthBarWidth = healthBarFill.rect.width;
        maxHealthWidth = originalHealthBarWidth;
        //Cursor.lockState = CursorLockMode.Locked;

        // Store initial player stats
        if (ThirdPersonController.Instance != null)
        {
            ThirdPersonController player = ThirdPersonController.Instance;
            initialStats["Max Health"] = player.MaxHealth;
            initialStats["Move Speed"] = player.MoveSpeed;
            initialStats["Sprint Multiplier"] = player.SprintMultiplier;
            initialStats["Damage"] = player.Damage;
            initialStats["Attack Speed"] = player.AttackSpeed;
            initialStats["Max Jumps"] = player.MaxJumps;
            initialStats["Health Regen"] = player.HealthRegenRate;
            initialStats["Jump Height"] = player.JumpHeight;

        }

        expandedSize = new Vector2(325, 400);
        collapsedSize = new Vector2(325, 50);


    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        if (elapsedTime >= 180)
        {
            difficulty = "Hard";
            textColor = Color.red;
        }
        else if (elapsedTime >= 120)
        {
            difficulty = "Medium";
            textColor = new Color(1f, 0.5f, 0f);
        }
        else if (elapsedTime >= 60)
        {
            difficulty = "Easy";
            textColor = Color.green;
        }

        timerText.color = textColor;
        timerText.text = $"Time Survived: {minutes:00}:{seconds:00}   {difficulty}";

        if (statsVisible)
        {
            UpdateStatsDisplay();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleStatsPanel();
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
        moneyText.text = "$" + money.ToString();
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        SetHealth(maxHealth);  // don't touch width here!
    }

    public void SetHealth(int currentHealth)
    {
        if (maxHealth == 0 || originalHealthBarWidth == 0) return;

        float healthPercentage = Mathf.Clamp((float)currentHealth / maxHealth, 0f, 1f);
        healthBarFill.sizeDelta = new Vector2(originalHealthBarWidth * healthPercentage, healthBarFill.sizeDelta.y);
    }

    public void GameOver()
    {
        PlayerPrefs.SetInt("FinalMoney", money);
        PlayerPrefs.SetFloat("FinalTime", elapsedTime);
        SceneManager.LoadScene("GameOverScene"); // Ensure "GameOverScene" exists
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void UpdateStatsDisplay()
    {
        if (ThirdPersonController.Instance == null) return;

        ThirdPersonController player = ThirdPersonController.Instance;

        // Function to format stats with color change if upgraded
        string FormatStat(string statName, float value)
        {
            if (initialStats.ContainsKey(statName))
            {
                float initialValue = initialStats[statName];
                if (value > initialValue)
                {
                    // Special case for Sprint Multiplier
                    if (statName == "Sprint Multiplier")
                    {
                        return $"<color=#03fc41FF><b>{statName}: {value}x</b></color>"; // Neon green and bold if upgraded
                    }
                    return $"<color=#03fc41FF><b>{statName}: {value}</b></color>"; // Neon green and bold if upgraded
                }
            }
            // Default black and bold
            return statName == "Sprint Multiplier"
                ? $"<color=#000000><b>{statName}: {value}x</b></color>"
                : $"<color=#000000><b>{statName}: {value}</b></color>";
        }

        statsText.text = FormatStat("Max Health", player.MaxHealth) + "\n" +
                         FormatStat("Move Speed", player.MoveSpeed) + "\n" +
                         FormatStat("Sprint Multiplier", player.SprintMultiplier) + "\n" +
                         FormatStat("Damage", player.GetWeapon().damage) + "\n" +
                         FormatStat("Attack Speed", player.GetWeapon().attackSpeed) + "\n" +
                         FormatStat("Max Jumps", player.MaxJumps) + "\n" +
                         FormatStat("Health Regen", player.HealthRegenRate) + "\n" +
                         FormatStat("Jump Height", player.JumpHeight) + "\n";
    }

    private void ToggleStatsPanel()
    {
        statsVisible = !statsVisible;

        statsTextObject.SetActive(statsVisible);
        toggleText.text = statsVisible ? "[TAB] Shrink Stats" : "[TAB] Expand Stats";

        float newHeight = statsVisible ? 400f : 50f;
        statsPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
    }
    public void ActivateMainUI()
    {
        if (mainUICanvas != null)
        {
            mainUICanvas.SetActive(true);
        }
    }

    // to add a collectible icon to the UI
    public void AddCollectedItem(string id, Sprite icon = null)
    {
       // TODO: do it 
    }


}