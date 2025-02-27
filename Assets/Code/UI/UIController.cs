using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    public TextMeshProUGUI timerText;
    private float elapsedTime = 0f;
    private string difficulty = "Easy";
    private Color textColor = Color.green;

    public TextMeshProUGUI moneyText;
    private int money = 0;

    public RectTransform healthBarFill;
    private float maxHealthWidth;
    private int maxHealth;

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
}
