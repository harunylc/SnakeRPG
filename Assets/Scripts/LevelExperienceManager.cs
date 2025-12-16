using System;
using UnityEngine;

public class LevelExperienceManager : MonoBehaviour
{
    private static LevelExperienceManager _instance;
    public static LevelExperienceManager Instance 
    { 
        get
        {
            if (_instance == null)
            {
                var existing = FindObjectOfType<LevelExperienceManager>();
                if (existing != null)
                {
                    _instance = existing;
                }
                else
                {
                    GameObject go = new GameObject("LevelExperienceManager_AutoCreated");
                    _instance = go.AddComponent<LevelExperienceManager>();
                }
            }
            return _instance;
        } 
    }

    public static LevelExperienceManager ExistingInstance => _instance;

    [Header("Level Settings")]
    [SerializeField] private AnimationCurve xpCurve = AnimationCurve.Linear(1, 10, 50, 500);
    
    public event Action<int> OnLevelUp;
    public event Action<float> OnXPGained; // Change to float (0-1 progress)

    private int _currentLevel = 1;
    private int _currentXP = 0;
    private int _requiredXP;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        CalculateNextLevelXP();
    }

    public void AddXP(int amount)
    {
        _currentXP += amount;

        if (_currentXP >= _requiredXP)
        {
            LevelUp();
        }

        // Notify UI (progress normalized)
        OnXPGained?.Invoke((float)_currentXP / _requiredXP);
    }

    private void LevelUp()
    {
        _currentXP -= _requiredXP;
        _currentLevel++;
        CalculateNextLevelXP();

        OnLevelUp?.Invoke(_currentLevel);
        UpgradeManager.Instance.ShowUpgradeOptions();
    }

    private void CalculateNextLevelXP()
    {
        // Simple curve evaluation or formula
        // _requiredXP = Mathf.RoundToInt(xpCurve.Evaluate(_currentLevel));
        
        // Per User Request: Kill 10 Enemies ~ 10 XP (assuming 1 XP per gem)
        // Flat curve or Linear? "kill 10 enemy spawn another segment" implying every 10 kills.
        _requiredXP = 10;
    }
}
