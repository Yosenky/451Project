using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI finalStatsText;
    public Transform collectedItemsContainer;
    public GameObject collectedItemUIPrefab; 


    void Start()
    {
        // Retrieve final stats
        int money = PlayerPrefs.GetInt("FinalMoney", 0);
        float timeSurvived = PlayerPrefs.GetFloat("FinalTime", 0);

        int minutes = Mathf.FloorToInt(timeSurvived / 60);
        int seconds = Mathf.FloorToInt(timeSurvived % 60);

        finalStatsText.text = $"You survived: {minutes:00}:{seconds:00}\nMoney earned: ${money}";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // show collected collectibles 
        foreach (string collectibleName in GameController.Instance.collectedCollectibles)
        {
            GameObject iconObj = Instantiate(collectedItemUIPrefab, collectedItemsContainer);

            if (GameController.Instance.collectibleIcons.TryGetValue(collectibleName, out Sprite icon))
            {
                Image image = iconObj.GetComponentInChildren<Image>();
                if (image != null)
                {
                    image.sprite = icon;
                    image.preserveAspect = true;
                }
            }
        }
    }

    public void PlayAgain()
    {
        GameController.Instance.collectedCollectibles.Clear(); // clear 
        SceneManager.LoadScene("MainScene");
    }
}
