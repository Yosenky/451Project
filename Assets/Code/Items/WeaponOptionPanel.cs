using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class WeaponOptionPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image iconImage;
    public TextMeshProUGUI descriptionText;
    public Button selectButton;
    public Image background;

    public delegate void OnSelectAction();
    private OnSelectAction onSelect;

    // Original background color (to reset on exit)
    private Color originalBgColor;

    public void Setup(Sprite icon, string description, OnSelectAction action, Color backgroundColor)
    {
        iconImage.sprite = icon;
        descriptionText.text = description;
        onSelect = action;
        background.color = backgroundColor;
        originalBgColor = backgroundColor;
        selectButton.onClick.AddListener(HandleSelect);

        // Optionally scale the icon down if needed.
        RectTransform iconRect = iconImage.GetComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(80, 80);
    }

    private void HandleSelect()
    {
        onSelect?.Invoke();
        selectButton.interactable = false;
    }

    // When the pointer enters the panel, change the background color or log a message.
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Hover detected on: " + descriptionText.text);
        // Change color to indicate hover.
        background.color = Color.Lerp(originalBgColor, Color.white, 0.5f);
    }

    // When the pointer exits, revert the background color.
    public void OnPointerExit(PointerEventData eventData)
    {
        background.color = originalBgColor;
    }

    private void OnDestroy()
    {
        selectButton.onClick.RemoveListener(HandleSelect);
    }
}
