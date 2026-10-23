using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPanelUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text winCoinText;
    [SerializeField] private TMP_Text winTimeText;
    [SerializeField] private TMP_Text winBestTimeText;

    [Header("Stars Display")]
    [SerializeField] private GameObject[] winStars;

    [Header("Buttons")]
    [SerializeField] private Button winNextLevelButton;
    [SerializeField] private Button winRestartButton;
    [SerializeField] private Button winHomeButton;

    public void Initialize(Action onNextLevel, Action onRestart, Action onHome)
    {
        if (winNextLevelButton != null)
        {
            winNextLevelButton.onClick.RemoveAllListeners();
            winNextLevelButton.onClick.AddListener(() => onNextLevel?.Invoke());
        }

        if (winRestartButton != null)
        {
            winRestartButton.onClick.RemoveAllListeners();
            winRestartButton.onClick.AddListener(() => onRestart?.Invoke());
        }

        if (winHomeButton != null)
        {
            winHomeButton.onClick.RemoveAllListeners();
            winHomeButton.onClick.AddListener(() => onHome?.Invoke());
        }
    }

    public void Show(int coinsEarned, float levelTime, int starsEarned)
    {
        gameObject.SetActive(true);

        if (winCoinText != null)
        {
            winCoinText.text = $"+{coinsEarned}";
        }

        if (winTimeText != null)
        {
            int m = Mathf.FloorToInt(levelTime / 60f);
            int s = Mathf.FloorToInt(levelTime % 60f);
            winTimeText.text = $"{m:00}:{s:00}";
        }

        if (winBestTimeText != null)
        {
            float bestTime = DataManager.GetBestTime(DataManager.SelectedLevel);
            if (bestTime > 0.05f)
            {
                int bm = Mathf.FloorToInt(bestTime / 60f);
                int bs = Mathf.FloorToInt(bestTime % 60f);
                winBestTimeText.text = $"BEST: {bm:00}:{bs:00}";
            }
            else
            {
                winBestTimeText.text = "";
            }
        }

        if (winStars != null)
        {
            for (int i = 0; i < winStars.Length; i++)
            {
                if (winStars[i] != null)
                {
                    winStars[i].SetActive(i < starsEarned);
                }
            }
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
