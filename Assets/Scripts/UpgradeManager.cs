using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private static UpgradeManager _instance;
    public static UpgradeManager Instance 
    { 
        get
        {
            if (_instance == null)
            {
                var existing = FindObjectOfType<UpgradeManager>();
                if (existing != null)
                {
                    _instance = existing;
                }
                else
                {
                    GameObject go = new GameObject("UpgradeManager_AutoCreated");
                    _instance = go.AddComponent<UpgradeManager>();
                }
            }
            return _instance;
        } 
    }

    [SerializeField] private SnakeBodyManager snakeBodyManager;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    public void ShowUpgradeOptions()
    {
        // Per User Request: "choose random archer or default"
        // No UI, No Pause. Just auto-pick.
        
        int randomChoice = Random.Range(0, 2); // 0 or 1
        SelectUpgrade(randomChoice);
    }

    public void SelectUpgrade(int index)
    {
        // Apply Effect
        if (snakeBodyManager == null)
        {
            snakeBodyManager = FindObjectOfType<SnakeBodyManager>();
        }

        if (snakeBodyManager != null)
        {
            if (index == 0)
            {
                snakeBodyManager.AddSegment(SegmentType.Normal);
            }
            else
            {
                snakeBodyManager.AddSegment(SegmentType.Archer);
            }
        }
    }
}
