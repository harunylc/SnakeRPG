using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var existing = FindObjectOfType<UIManager>();
                if (existing != null)
                {
                    _instance = existing;
                }
                else
                {
                    GameObject go = new GameObject("UIManager_AutoCreated");
                    _instance = go.AddComponent<UIManager>();
                }
            }
            return _instance;
        }
    }

    public static void EnsureInstance()
    {
        var i = Instance;
    }

    [Header("UI References")]
    [SerializeField] private Slider xpSlider;
    [SerializeField] private Text levelText;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        EnsureUI();
    }

    private void Start()
    {
        // Subscribe to events
        if (LevelExperienceManager.Instance != null)
        {
            LevelExperienceManager.Instance.OnXPGained += UpdateXP;
            LevelExperienceManager.Instance.OnLevelUp += UpdateLevel;
        }
    }

    private void OnDestroy()
    {
        if (LevelExperienceManager.ExistingInstance != null)
        {
            LevelExperienceManager.ExistingInstance.OnXPGained -= UpdateXP;
            LevelExperienceManager.ExistingInstance.OnLevelUp -= UpdateLevel;
        }
    }

    private void UpdateXP(float progress)
    {
        if (xpSlider != null)
        {
            xpSlider.value = progress;
        }
    }

    private void UpdateLevel(int level)
    {
        if (levelText != null)
        {
            levelText.text = $"Lvl {level}";
        }
    }

    private void EnsureUI()
    {
        if (xpSlider == null || levelText == null)
        {
            // 1. Check for Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            // 2. Create Slider (Top Center)
            if (xpSlider == null)
            {
                GameObject sliderGO = new GameObject("XP_Slider");
                sliderGO.transform.SetParent(canvas.transform, false);
                
                // Background
                Image bg = sliderGO.AddComponent<Image>();
                bg.color = Color.gray;
                
                RectTransform sliderRT = sliderGO.GetComponent<RectTransform>();
                sliderRT.anchorMin = new Vector2(0.5f, 1f); // Top Center
                sliderRT.anchorMax = new Vector2(0.5f, 1f);
                sliderRT.pivot = new Vector2(0.5f, 1f);
                sliderRT.anchoredPosition = new Vector2(0, -20);
                sliderRT.sizeDelta = new Vector2(400, 20);

                // Fill Area (Child)
                GameObject fillArea = new GameObject("Fill Area");
                fillArea.transform.SetParent(sliderGO.transform, false);
                RectTransform fillAreaRT = fillArea.AddComponent<RectTransform>();
                fillAreaRT.anchorMin = Vector2.zero;
                fillAreaRT.anchorMax = Vector2.one;
                fillAreaRT.sizeDelta = Vector2.zero;

                // Fill (Child of Area)
                GameObject fill = new GameObject("Fill");
                fill.transform.SetParent(fillArea.transform, false);
                Image fillImage = fill.AddComponent<Image>();
                fillImage.color = Color.yellow;
                
                RectTransform fillRT = fill.GetComponent<RectTransform>();
                fillRT.anchorMin = Vector2.zero;
                fillRT.anchorMax = Vector2.one;
                fillRT.sizeDelta = Vector2.zero;

                // Setup Slider Component
                xpSlider = sliderGO.AddComponent<Slider>();
                xpSlider.targetGraphic = bg;
                xpSlider.fillRect = fillRT;
                xpSlider.direction = Slider.Direction.LeftToRight;
                xpSlider.minValue = 0;
                xpSlider.maxValue = 1;
                xpSlider.value = 0;
            }

            // 3. Create Text (Top Center, below slider)
            if (levelText == null)
            {
                GameObject textGO = new GameObject("Level_Text");
                textGO.transform.SetParent(canvas.transform, false);

                levelText = textGO.AddComponent<Text>();
                levelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                levelText.color = Color.white;
                levelText.alignment = TextAnchor.MiddleCenter;
                levelText.text = "Lvl 1";
                levelText.fontSize = 24;

                RectTransform textRT = textGO.GetComponent<RectTransform>();
                textRT.anchorMin = new Vector2(0.5f, 1f);
                textRT.anchorMax = new Vector2(0.5f, 1f);
                textRT.pivot = new Vector2(0.5f, 1f);
                textRT.anchoredPosition = new Vector2(0, -50);
                textRT.sizeDelta = new Vector2(200, 30);
            }
        }
    }
}
