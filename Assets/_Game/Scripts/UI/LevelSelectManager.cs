using System.Collections.Generic;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Database & Spawning")]
    [SerializeField] private LevelDatabaseSO levelDatabase;
    [SerializeField] private LevelCardUI levelCardPrefab;

    [Header("Card containers")]
    [SerializeField] private Transform easyContent;
    [SerializeField] private Transform normalContent;

    private List<LevelCardUI> spawnedCards = new List<LevelCardUI>();
    private void Start()
    {
        GenerateLevelCards();
    }
    private void GenerateLevelCards()
    {
        CleanCardContainer(easyContent);
        CleanCardContainer(normalContent);
        spawnedCards.Clear();

        if (levelDatabase == null)
        {
            return;
        }

        if (levelCardPrefab == null)
        {
            return;
        }

        foreach (LevelConfigSO config in levelDatabase.Levels)
        {
            if (config == null) continue;

            Transform target = config.Difficulty == LevelDifficulty.Easy ? easyContent : normalContent;

            if (target == null)
            {
                Debug.LogWarning($"[LevelSelectManager] Chưa gán container cho {config.Difficulty}");
                continue;
            }

            LevelCardUI newCard = Instantiate(levelCardPrefab, target);
            newCard.Setup(config, this);
            spawnedCards.Add(newCard);
        }
    }

    private void CleanCardContainer(Transform container)
    {
        if (container == null) return;
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnLevelCardClicked(LevelConfigSO config)
    {
        if (MainMenuController.Instance != null)
        {
            MainMenuController.Instance.LoadParkourLevel(config.LevelId);
        }
    }

    public void RefreshAllCards()
    {
        foreach (var card in spawnedCards)
        {
            if (card != null) card.RefreshStars();
        }
    }
}
