using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public int chestPrice = 100;

    public GameObject upgradeInterface;

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
        //currentChest.Interact();

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (upgradeInterface != null)
            upgradeInterface.SetActive(true);
    }

    public void UpgradeSelected()
    {
        if (UIController.Instance.money < chestPrice)
        {
            Debug.Log("Not enough money to upgrade!");
            CancelUpgrade();
            return;
        }

        UIController.Instance.AddMoney(-chestPrice);

        chestPrice += 10;

        // Process the upgrade purchase (apply benefits, etc.)


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

        if (upgradeInterface != null){
            upgradeInterface.SetActive(false);
        }              

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
