using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance; // Make singleton

    public TextMeshProUGUI timerText;
    private float elapsedTime = 0f;
    private string difficulty = "Easy";
    private Color textColor = Color.green;
    public Slider healthSlider; 
    public TextMeshProUGUI moneyText;
    private int money = 0;

    void Awake()
    {
        // Singleton setup
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

    void Update()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        if (elapsedTime >= 180) // 3 minutes - turns to hard
        {
            difficulty = "Hard";
            textColor = Color.red;
        }
        else if (elapsedTime >= 120) // 2 minutes - turns to medium
        {
            difficulty = "Medium";
            textColor = new Color(1f, 0.5f, 0f); // Orange
        }
        else if (elapsedTime >= 60) // 1 minute - easy
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

    public void SetMaxHealth(int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void SetHealth(int currentHealth)
    {
        healthSlider.value = currentHealth;
    }

    
}
