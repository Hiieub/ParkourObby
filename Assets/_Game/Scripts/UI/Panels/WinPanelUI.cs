using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPanelUI : MonoBehaviour
{
    private UIPopupAnimation popupAnim;

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

    private void Awake()
    {
        popupAnim = GetComponent<UIPopupAnimation>();
    }

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
        if (popupAnim != null) popupAnim.Open();
        else gameObject.SetActive(true);

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
                winBestTimeText.text = $"{bm:00}:{bs:00}";
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
                if (winStars[i] == null) continue;

                winStars[i].transform.DOKill();

                bool earned = i < starsEarned;
                winStars[i].SetActive(earned);

                if (earned)
                {
                    winStars[i].transform.localScale = Vector3.zero;

                    winStars[i].transform.DOScale(1f, 0.35f).SetEase(Ease.OutBack).SetDelay(0.2f + i * 0.15f)
                        .SetUpdate(true)
                        .SetLink(winStars[i], LinkBehaviour.KillOnDisable)
                        .OnStart(() => AudioManager.Instance?.PlayCoin());
                }
            }
        }
    }

    public void Hide()
    {
        if (popupAnim != null) popupAnim.Close();
        else gameObject.SetActive(false);
    }
}
