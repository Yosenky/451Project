using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using StarterAssets;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI statsText;
    private float elapsedTime = 0f;
    private string difficulty = "Easy";
    private Color textColor = Color.green;

    public TextMeshProUGUI moneyText;
    public int money = 0;

    public RectTransform healthBarFill;
    private float maxHealthWidth;
    private int maxHealth;

    // Dictionary to store initial stat values
    private Dictionary<string, float> initialStats = new Dictionary<string, float>();

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
        maxHealthWidth = healthBarFill.rect.width;
        Cursor.lockState = CursorLockMode.Locked;

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

        UpdateStatsDisplay();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        moneyText.text = "$" + money.ToString();
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        maxHealthWidth = healthBarFill.rect.width;
        SetHealth(maxHealth);
    }

    public void SetHealth(int currentHealth)
    {
        if (maxHealth == 0 || maxHealthWidth == 0) return;

        float healthPercentage = Mathf.Clamp((float)currentHealth / maxHealth, 0f, 1f);
        healthBarFill.sizeDelta = new Vector2(maxHealthWidth * healthPercentage, healthBarFill.sizeDelta.y);
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

        statsText.text = "<b>Stats</b>\n\n" +
                         FormatStat("Max Health", player.MaxHealth) + "\n" +
                         FormatStat("Move Speed", player.MoveSpeed) + "\n" +
                         FormatStat("Sprint Multiplier", player.SprintMultiplier) + "\n" +
                         FormatStat("Damage", player.Damage) + "\n" +
                         FormatStat("Attack Speed", player.AttackSpeed) + "\n" +
                         FormatStat("Max Jumps", player.MaxJumps) + "\n" +
                         FormatStat("Health Regen", player.HealthRegenRate) + "\n" +
                         FormatStat("Jump Height", player.JumpHeight) + "\n";
    }

}