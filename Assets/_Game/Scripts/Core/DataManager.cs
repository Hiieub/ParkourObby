using System;
using UnityEngine;

public static class DataManager
{
    private const string KEY_TOTAL_COINS = "TotalCoins";
    private const string KEY_SELECTED_LEVEL = "SelectedLevel";
    private const string KEY_MUSIC_VOLUME = "MusicVolume";
    private const string KEY_SFX_VOLUME = "SFXVolume";
    private const string KEY_STARS_PREFIX = "Stars_";

    private const string SKIN_PREF_KEY = "SelectedSkinKey";
    private const string DEFAULT_SKIN_ID = "5";

    #region Level Configuration & ScriptableObject Database
    private static LevelDatabaseSO activeLevelDatabase;

    public static void SetLevelDatabase(LevelDatabaseSO db)
    {
        activeLevelDatabase = db;
    }

    public static LevelDatabaseSO Database
    {
        get
        {
            if (activeLevelDatabase == null)
            {
                activeLevelDatabase = Resources.Load<LevelDatabaseSO>("LevelDatabase");
            }
            return activeLevelDatabase;
        }
    }

    public static string DefaultLevel => Database != null ? Database.DefaultLevelId : "Easy_1";

    public static string GetNextLevelId(string currentLevelId)
    {
        return Database != null ? Database.GetNextLevelId(currentLevelId) : currentLevelId;
    }

    public static string FormatLevelDisplayName(string levelId)
    {
        return Database != null ? Database.GetDisplayName(levelId) : (string.IsNullOrEmpty(levelId) ? "STAGE 1" : levelId.Replace("_", " ").ToUpper());
    }
    #endregion

    #region Coins
    public static event Action<int> OnTotalCoinsChanged;

    public static int TotalCoins
    {
        get => PlayerPrefs.GetInt(KEY_TOTAL_COINS, 0);
        set
        {
            PlayerPrefs.SetInt(KEY_TOTAL_COINS, value);
            PlayerPrefs.Save();
            OnTotalCoinsChanged?.Invoke(value);
        }
    }

    public static void AddCoins(int amount)
    {
        if (amount <= 0) return;
        TotalCoins += amount;
    }
    #endregion

    #region Selected Level
    public static string SelectedLevel
    {
        get => PlayerPrefs.GetString(KEY_SELECTED_LEVEL, DefaultLevel);
        set
        {
            PlayerPrefs.SetString(KEY_SELECTED_LEVEL, value);
            PlayerPrefs.Save();
        }
    }
    #endregion

    public static string SelectedSkin
    {
        get => PlayerPrefs.GetString(SKIN_PREF_KEY, DEFAULT_SKIN_ID);
        set
        {
            PlayerPrefs.SetString(SKIN_PREF_KEY, value);
            PlayerPrefs.Save();
        }
    }

    // Hệ thống Mua/Mở khóa Skin trong Shop
    public static bool IsSkinUnlocked(string skinId, int price)
    {
        if (price <= 0 || skinId == DEFAULT_SKIN_ID) return true;
        return PlayerPrefs.GetInt($"SkinUnlocked_{skinId}", 0) == 1;
    }

    public static void UnlockSkin(string skinId)
    {
        PlayerPrefs.SetInt($"SkinUnlocked_{skinId}", 1);
        PlayerPrefs.Save();
    }

    #region Audio Settings
    public static float MusicVolume
    {
        get => PlayerPrefs.GetFloat(KEY_MUSIC_VOLUME, 0.5f);
        set
        {
            PlayerPrefs.SetFloat(KEY_MUSIC_VOLUME, Mathf.Clamp01(value));
            PlayerPrefs.Save();
        }
    }

    public static float SFXVolume
    {
        get => PlayerPrefs.GetFloat(KEY_SFX_VOLUME, 0.8f);
        set
        {
            PlayerPrefs.SetFloat(KEY_SFX_VOLUME, Mathf.Clamp01(value));
            PlayerPrefs.Save();
        }
    }
    #endregion

    #region Level Stars
    public static int GetStars(string levelId)
    {
        if (string.IsNullOrEmpty(levelId)) return 0;
        return PlayerPrefs.GetInt(KEY_STARS_PREFIX + levelId, 0);
    }

    public static void SetStars(string levelId, int stars)
    {
        if (string.IsNullOrEmpty(levelId)) return;
        int currentBest = GetStars(levelId);
        if (stars > currentBest)
        {
            PlayerPrefs.SetInt(KEY_STARS_PREFIX + levelId, Mathf.Clamp(stars, 0, 3));
            PlayerPrefs.Save();
        }
    }
    #endregion

    #region Best Time / High Score Record
    private const string KEY_BEST_TIME_PREFIX = "BestTime_";

    public static float GetBestTime(string levelId)
    {
        if (string.IsNullOrEmpty(levelId)) return 0f;
        return PlayerPrefs.GetFloat(KEY_BEST_TIME_PREFIX + levelId, 0f);
    }

    public static bool TrySetBestTime(string levelId, float time)
    {
        if (string.IsNullOrEmpty(levelId) || time <= 0.05f) return false;

        float currentBest = GetBestTime(levelId);
        if (currentBest <= 0.05f || time < currentBest)
        {
            PlayerPrefs.SetFloat(KEY_BEST_TIME_PREFIX + levelId, time);
            PlayerPrefs.Save();
            return true;
        }

        return false;
    }
    #endregion
}
