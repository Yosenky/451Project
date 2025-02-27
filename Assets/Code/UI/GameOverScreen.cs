using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI finalStatsText;

    void Start()
    {
        // Retrieve final stats
        int money = PlayerPrefs.GetInt("FinalMoney", 0);
        float timeSurvived = PlayerPrefs.GetFloat("FinalTime", 0);

        int minutes = Mathf.FloorToInt(timeSurvived / 60);
        int seconds = Mathf.FloorToInt(timeSurvived % 60);

        finalStatsText.text = $"You survived: {minutes:00}:{seconds:00}\nMoney earned: ${money}";

        Cursor.lockState = CursorLockMode.None;
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("MainScene");
    }
}
