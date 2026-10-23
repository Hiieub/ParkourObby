using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panel Components")]
    [SerializeField] private HUDPanel hudPanel;
    [SerializeField] private SettingsPopupUI settingsPopup;
    [SerializeField] private WinPanelUI winPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (hudPanel != null)
        {
            hudPanel.Initialize(onOpenSetting: OpenSettingMenu);
            hudPanel.UpdateStageTitle(DataManager.FormatLevelDisplayName(DataManager.SelectedLevel));
            hudPanel.UpdateCoins(0);
        }

        if (settingsPopup != null)
        {
            settingsPopup.Initialize(
                onCloseOrResume: CloseSettingMenu,
                onRetry: RestartCurrentLevel,
                onHome: GoToMainMenu
            );
            settingsPopup.Hide();
        }

        if (winPanel != null)
        {
            winPanel.Initialize(
                onNextLevel: GoToNextLevel,
                onRestart: RestartCurrentLevel,
                onHome: GoToMainMenu
            );
            winPanel.Hide();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            GameManager.Instance.OnProgressUpdated += HandleProgressUpdated;
            GameManager.Instance.OnTimerUpdated += HandleTimerUpdated;
            GameManager.Instance.OnCoinChanged += HandleCoinChanged;
            GameManager.Instance.OnLevelWon += HandleLevelWon;

            HandleProgressUpdated(GameManager.Instance.ProgressRatio, GameManager.Instance.CurrentStage, GameManager.Instance.TotalStages);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            GameManager.Instance.OnProgressUpdated -= HandleProgressUpdated;
            GameManager.Instance.OnTimerUpdated -= HandleTimerUpdated;
            GameManager.Instance.OnCoinChanged -= HandleCoinChanged;
            GameManager.Instance.OnLevelWon -= HandleLevelWon;
        }
    }

    #region State Machine Listener
    private void HandleGameStateChanged(GameState oldState, GameState newState)
    {
        switch (newState)
        {
            case GameState.Playing:
                if (settingsPopup != null) settingsPopup.Hide();
                if (winPanel != null) winPanel.Hide();
                if (hudPanel != null) hudPanel.SetVisible(true);
                break;

            case GameState.Paused:
                if (settingsPopup != null) settingsPopup.Show();
                break;

            case GameState.Victory:
                if (settingsPopup != null) settingsPopup.Hide();
                break;
        }
    }
    #endregion

    #region Game Events Handlers
    private void HandleProgressUpdated(float ratio, int stage, int totalStages)
    {
        if (hudPanel != null)
        {
            hudPanel.UpdateProgress(ratio, stage, totalStages);
        }
    }

    private void HandleTimerUpdated(float time)
    {
        if (hudPanel != null)
        {
            hudPanel.UpdateTimer(time);
        }
    }

    private void HandleCoinChanged(int totalCoins)
    {
        if (hudPanel != null)
        {
            hudPanel.UpdateCoins(totalCoins);
        }
    }

    private void HandleLevelWon(float time, int coins, int stars)
    {
        if (winPanel != null)
        {
            winPanel.Show(coins, time, stars);
        }
    }
    #endregion

    #region Navigation & Actions
    public void OpenSettingMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseGame();
        }
        else if (settingsPopup != null)
        {
            settingsPopup.Show();
            Time.timeScale = 0f;
        }
    }

    public void CloseSettingMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
        else if (settingsPopup != null)
        {
            settingsPopup.Hide();
            Time.timeScale = 1f;
        }
    }

    public void RestartCurrentLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToNextLevel()
    {
        Time.timeScale = 1f;

        string nextLevel = DataManager.GetNextLevelId(DataManager.SelectedLevel);
        DataManager.SelectedLevel = nextLevel;

        SceneManager.LoadScene("GameScene");
    }
    #endregion
}
