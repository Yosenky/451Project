using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeOptionPanel : MonoBehaviour
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
        selectButton.onClick.AddListener(HandleSelect);
    }

    private void HandleSelect()
    {
        onSelect?.Invoke();
        selectButton.interactable = false;
    }

    private void OnDestroy()
    {
        selectButton.onClick.RemoveListener(HandleSelect);
    }
}
