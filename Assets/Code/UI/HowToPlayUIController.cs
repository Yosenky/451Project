using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HowToPlayUIController : MonoBehaviour
{
    [Header("UI References")]
    public Canvas canvas;

    [Header("Left Side (3 Texts)")]
    public TextMeshProUGUI[] leftTexts = new TextMeshProUGUI[3];

    [Header("Right Side (6 Texts + Button)")]
    public TextMeshProUGUI[] rightTexts = new TextMeshProUGUI[6];
    public Button continueButton;

    [Header("Font Settings")]
    public float minFontSize = 12f;
    public float maxFontSize = 36f;

    [Header("Button Settings")]
    public float buttonHeight = 60f;

    void Start()
    {
        if (!canvas)
        {
            Debug.LogError("Canvas is not assigned.");
            return;
        }

        // === Full Screen Horizontal Wrapper ===
        GameObject wrapper = CreateUI("HorizontalWrapper", canvas.transform);
        SetFullStretch(wrapper);
        HorizontalLayoutGroup hGroup = wrapper.AddComponent<HorizontalLayoutGroup>();
        hGroup.spacing = 20;
        hGroup.childControlWidth = true;
        hGroup.childControlHeight = true;
        hGroup.childForceExpandWidth = true;
        hGroup.childForceExpandHeight = true;

        // === LEFT COLUMN ===
        GameObject leftPanel = CreateUI("LeftPanel", wrapper.transform);
        SetFullStretch(leftPanel);
        AddLayoutElement(leftPanel, flexWidth: 3);

        GameObject leftStack = CreateUI("LeftStack", leftPanel.transform);
        SetFullStretch(leftStack);
        VerticalLayoutGroup leftLayout = AddVerticalLayoutGroup(leftStack);

        foreach (var tmp in leftTexts)
        {
            SetupTMPToFit(tmp, leftStack.transform, minSize: 10f, maxSize: 36f, flexibleHeight: 1f);
        }

        // === RIGHT COLUMN ===
        GameObject rightPanel = CreateUI("RightPanel", wrapper.transform);
        SetFullStretch(rightPanel);
        AddLayoutElement(rightPanel, flexWidth: 7);

        GameObject rightStack = CreateUI("RightStack", rightPanel.transform);
        SetFullStretch(rightStack);
        VerticalLayoutGroup rightLayout = AddVerticalLayoutGroup(rightStack);

        // Add 6 right texts
        foreach (var tmp in rightTexts)
        {
            SetupTMPToFit(tmp, rightStack.transform, minFontSize, maxFontSize, 1f);
        }

        // Add button at bottom
        SetupFixedHeightButton(continueButton, rightStack.transform, buttonHeight);
    }

    // === TMP Text Setup ===
    void SetupTMPToFit(TextMeshProUGUI tmp, Transform parent, float minSize, float maxSize, float flexibleHeight)
    {
        // Reparent
        tmp.transform.SetParent(parent, false);

        // Full hard reset of scale & position
        RectTransform rt = tmp.GetComponent<RectTransform>();
        tmp.transform.localScale = Vector3.one;
        rt.localPosition = Vector3.zero;
        rt.localRotation = Quaternion.identity;

        // Stretch full width, align top
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1);

        // Even spacing on both sides
        rt.offsetMin = new Vector2(20, 0);
        rt.offsetMax = new Vector2(-20, 0);
        rt.anchoredPosition = new Vector2(0, 0);

        // TMP Settings
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = minSize;
        tmp.fontSizeMax = maxSize;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Ellipsis;
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.margin = new Vector4(10, 5, 10, 5);

        // Layout
        LayoutElement le = tmp.GetComponent<LayoutElement>();
        if (le == null) le = tmp.gameObject.AddComponent<LayoutElement>();
        le.minHeight = 10f;
        le.flexibleHeight = flexibleHeight;
        le.preferredHeight = -1f;
        le.preferredWidth = -1f;
    }


    // === Button Setup ===
    void SetupFixedHeightButton(Button button, Transform parent, float height)
    {
        button.transform.SetParent(parent, false);

        RectTransform rt = button.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.offsetMin = new Vector2(20, 0);
        rt.offsetMax = new Vector2(-20, 0);
        rt.sizeDelta = new Vector2(0, height);

        LayoutElement le = button.GetComponent<LayoutElement>();
        if (le == null) le = button.gameObject.AddComponent<LayoutElement>();
        le.preferredHeight = height;
        le.flexibleHeight = 0;

        // Auto-size the text on the button
        TextMeshProUGUI tmp = button.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 10;
            tmp.fontSizeMax = 32; // Slightly smaller than max available space
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.overflowMode = TextOverflowModes.Ellipsis;
            tmp.margin = new Vector4(10, 5, 10, 5);

            RectTransform textRT = tmp.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = Vector2.zero;
            textRT.offsetMax = Vector2.zero;
            textRT.pivot = new Vector2(0.5f, 0.5f);
        }
    }

    // === Utility Methods ===
    GameObject CreateUI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    void SetFullStretch(GameObject go)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
    }

    void AddLayoutElement(GameObject go, float flexWidth)
    {
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.flexibleWidth = flexWidth;
    }

    VerticalLayoutGroup AddVerticalLayoutGroup(GameObject go)
    {
        VerticalLayoutGroup layout = go.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = true;
        layout.childForceExpandWidth = true;
        layout.spacing = 10;
        layout.padding = new RectOffset(20, 20, 20, 20);
        return layout;
    }
}
