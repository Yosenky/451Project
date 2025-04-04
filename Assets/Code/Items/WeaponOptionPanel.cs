using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponOptionPanel : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI descriptionText;
    public Button selectButton;
    public Image background;

    public delegate void OnSelectAction();
    private OnSelectAction onSelect;

    public void Setup(Sprite icon, string description, OnSelectAction action, Color backgroundColor)
    {
        iconImage.sprite = icon;
        descriptionText.text = description;
        onSelect = action;
        background.color = backgroundColor;

        // Attach the click handler to the Select Button ONLY
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(HandleSelect);

            // Ensure its Image allows raycasting
            var buttonImage = selectButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.raycastTarget = true;
            }
        }
        else
        {
            Debug.LogWarning("WeaponOptionPanel: Select Button is not assigned.");
        }

        // Disable raycasting on the background so it doesn't block interaction
        if (background != null)
        {
            background.raycastTarget = false;
        }

        // Set icon size to be consistent
        if (iconImage != null)
        {
            RectTransform iconRect = iconImage.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(80, 80); // Adjust these values as needed.
        }

        Debug.Log($"WeaponOptionPanel Setup: {description}, Raycast config complete.");
    }

    private void HandleSelect()
    {
        Debug.Log("Weapon selected: " + descriptionText.text);
        onSelect?.Invoke();
        selectButton.interactable = false;
    }

    private void OnDestroy()
    {
        if (selectButton != null)
        {
            selectButton.onClick.RemoveListener(HandleSelect);
        }
    }
}
