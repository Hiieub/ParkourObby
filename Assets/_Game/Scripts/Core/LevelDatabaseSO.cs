using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "ParkourObby/Level Database", order = 2)]
public class LevelDatabaseSO : ScriptableObject
{
    [Header("Levels Progression List")]
    [SerializeField] private List<LevelConfigSO> levels = new List<LevelConfigSO>();

    public IReadOnlyList<LevelConfigSO> Levels => levels;

    public string DefaultLevelId => (levels != null && levels.Count > 0 && levels[0] != null) 
        ? levels[0].LevelId 
        : "Easy_1";

    public LevelConfigSO GetLevelById(string levelId)
    {
        if (levels == null) return null;
        return levels.Find(l => l != null && string.Equals(l.LevelId, levelId, StringComparison.OrdinalIgnoreCase));
    }

    public string GetNextLevelId(string currentLevelId)
    {
        if (levels == null || levels.Count == 0) return currentLevelId;

        int index = levels.FindIndex(l => l != null && string.Equals(l.LevelId, currentLevelId, StringComparison.OrdinalIgnoreCase));
        if (index >= 0 && index < levels.Count - 1)
        {
            var nextLevel = levels[index + 1];
            if (nextLevel != null && !string.IsNullOrEmpty(nextLevel.LevelId))
            {
                return nextLevel.LevelId;
            }
        }

        return (levels[0] != null) ? levels[0].LevelId : "Easy_1";
    }

    public string GetDisplayName(string levelId)
    {
        var config = GetLevelById(levelId);
        if (config != null)
        {
            return config.DisplayName;
        }

        return string.IsNullOrEmpty(levelId) ? "STAGE 1" : levelId.Replace("_", " ").ToUpper();
    }
}
