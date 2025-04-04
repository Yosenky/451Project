using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public enum WeaponType 
        {
            Ball, 
            Boomerang, 
            Explosive
        }
public class WeaponSelectController : MonoBehaviour
{
    public static WeaponSelectController Instance;

    [Header("UI References")]
    public GameObject weaponSelectInterface;            // Overall panel for weapon selection
    public GameObject weaponOptionPanelPrefab;            // Prefab for each weapon option panel
    public Transform weaponOptionsContainer;              // Container with a Horizontal Layout Group
   

    [Header("Weapon Icons")]
    public Sprite ballIcon;
    public Sprite boomerangIcon;
    public Sprite explosiveIcon;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (weaponSelectInterface != null)
            weaponSelectInterface.SetActive(false);

        // Ensure the cursor is visible for selection.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Start()
    {
        ShowWeaponSelect();
    }

    public void ShowWeaponSelect()
    {
        // Pause gameplay.
        Time.timeScale = 0f;
        weaponSelectInterface.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Clear previous weapon options.
        foreach (Transform child in weaponOptionsContainer)
            Destroy(child.gameObject);


        // Create a list of available weapon types.
        List<WeaponType> weaponTypes = new List<WeaponType>()
        {
            WeaponType.Ball,
            WeaponType.Boomerang,
            WeaponType.Explosive
        };

        // Get container dimensions.
        RectTransform containerRect = weaponOptionsContainer.GetComponent<RectTransform>();
        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;

        // Instantiate a weapon option panel for each available weapon.
        foreach (WeaponType type in weaponTypes)
        {
            GameObject optionObj = Instantiate(weaponOptionPanelPrefab, weaponOptionsContainer);
            WeaponOptionPanel optionPanel = optionObj.GetComponent<WeaponOptionPanel>();

            // Set size: 30% of container's width and 70% of container's height.
            RectTransform optionRect = optionObj.GetComponent<RectTransform>();
            optionRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, containerWidth * 0.3f);
            optionRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, containerHeight * 0.7f);

            Sprite icon = null;
            string description = "";
            Color bgColor = Color.white;

            switch (type)
            {
                case WeaponType.Ball:
                    icon = ballIcon;
                    description = "Standard Shoot";
                    bgColor = Color.green;
                    break;
                case WeaponType.Boomerang:
                    icon = boomerangIcon;
                    description = "Boomerang Shoot";
                    bgColor = Color.yellow;
                    break;
                case WeaponType.Explosive:
                    icon = explosiveIcon;
                    description = "Explosive Shoot";
                    bgColor = Color.red;
                    break;
            }

            optionPanel.Setup(icon, description, () => OnWeaponSelected(type), bgColor);
        }
    }

    void OnWeaponSelected(WeaponType selectedType)
    {
        // Set the chosen weapon in the Weapon script.
        Weapon.Instance.SetWeaponType(selectedType);
        UIController.Instance.ActivateMainUI();
        // Hide the weapon select interface and resume gameplay.
        weaponSelectInterface.SetActive(false);
        Time.timeScale = 1f;
        // Delay cursor lock to avoid interrupting UI click
        StartCoroutine(LockCursorNextFrame());
    }

    IEnumerator LockCursorNextFrame()
    {
        yield return null; // wait one frame
        Debug.Log("LOCKING");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


}
