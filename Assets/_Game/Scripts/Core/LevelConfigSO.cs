using UnityEngine;

public enum LevelDifficulty { Easy, Normal }

[CreateAssetMenu(fileName = "LevelConfig", menuName = "ParkourObby/Level Config", order = 1)]
public class LevelConfigSO : ScriptableObject
{
    [Header("Level Information")]
    [SerializeField] private string levelId = "Easy_1";
    [SerializeField] private string displayName = "EASY 1";
    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private Sprite thumbnail;

    [SerializeField] private LevelDifficulty difficulty = LevelDifficulty.Easy;

    [Header("Star Thresholds")]
    [SerializeField] private int maxDeathsFor3Stars = 2;
    [SerializeField] private float maxTimeFor3Stars = 60f;
    [SerializeField] private int maxDeathsFor2Stars = 5;
    [SerializeField] private float maxTimeFor2Stars = 120f;

    public string LevelId => levelId;
    public string DisplayName => string.IsNullOrEmpty(displayName) ? levelId.Replace("_", " ").ToUpper() : displayName;
    public GameObject LevelPrefab => levelPrefab;
    public Sprite Thumbnail => thumbnail;

    public LevelDifficulty Difficulty => difficulty;

    public void Initialize(string id, string name, GameObject prefab, Sprite thumb = null)
    {
        levelId = id;
        displayName = name;
        levelPrefab = prefab;
        thumbnail = thumb;
    }

    public int CalculateStars(int deaths, float clearTime)
    {
        if (deaths > maxDeathsFor2Stars || clearTime > maxTimeFor2Stars)
        {
            return 1;
        }
        if (deaths > maxDeathsFor3Stars || clearTime > maxTimeFor3Stars)
        {
            return 2;
        }
        return 3;
    }
}
