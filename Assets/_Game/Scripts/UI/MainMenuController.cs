using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject skinsPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Main Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button skinsButton;
    [SerializeField] private Button settingsButton;

    [Header("Level Select - Tabs")]
    [SerializeField] private Button easyTabButton;
    [SerializeField] private Button normalTabButton;
    [SerializeField] private Image easyTabBg;
    [SerializeField] private Image normalTabBg;

    [Header("Level Cards Containers")]
    [SerializeField] private GameObject easyCardsContainer;
    [SerializeField] private GameObject normalCardsContainer;

    [Header("Navigation Buttons")]
    [SerializeField] private Button backFromLevelSelect;
    [SerializeField] private Button backFromSkins;
    [SerializeField] private Button closeSettingsButton;

    [Header("Currency Display")]
    [SerializeField] private TMP_Text coinText;

    [Header("Settings Controls")]
    [SerializeField] private ToggleSwitch musicToggle;
    [SerializeField] private ToggleSwitch sfxToggle;

    private bool isEasyTabActive = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance );
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        RegisterListeners();
        RefreshCurrency();
        InitSettings();
        ShowMainMenu();
    }

    private void RegisterListeners()
    {
        // Main Menu
        if (playButton != null) playButton.onClick.AddListener(OpenLevelSelect);
        if (skinsButton != null) skinsButton.onClick.AddListener(OpenSkins);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);

        // Navigation
        if (backFromLevelSelect != null) backFromLevelSelect.onClick.AddListener(ShowMainMenu);
        if (backFromSkins != null) backFromSkins.onClick.AddListener(ShowMainMenu);
        if (closeSettingsButton != null) closeSettingsButton.onClick.AddListener(CloseSettings);

        // Tabs
        if (easyTabButton != null) easyTabButton.onClick.AddListener(() => SwitchTab(true));
        if (normalTabButton != null) normalTabButton.onClick.AddListener(() => SwitchTab(false));

        // Settings
        if (musicToggle != null)
        {
            musicToggle.OnToggled += (isOn) =>
            {
                DataManager.IsMusicOn = isOn;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.SetMusicVolume(DataManager.MusicVolume);
                }
            };
        }
        if (sfxToggle != null)
        {
            sfxToggle.OnToggled += (isOn) =>
            {
                DataManager.IsSfxOn = isOn;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.SetSFXVolume(DataManager.SFXVolume);
                }
            };
        }
    }

    public void ShowMainMenu()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        if (skinsPanel != null) skinsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void OpenLevelSelect()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (skinsPanel != null) skinsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(true);

        var levelManager = levelSelectPanel.GetComponentInChildren<LevelSelectManager>();
        if (levelManager != null) levelManager.RefreshAllCards();

        SwitchTab(isEasyTabActive);
    }

    public void OpenSkins()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (skinsPanel != null) skinsPanel.SetActive(true);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void SwitchTab(bool isEasy)
    {
        isEasyTabActive = isEasy;

        if (easyCardsContainer != null) easyCardsContainer.SetActive(isEasy);
        if (normalCardsContainer != null) normalCardsContainer.SetActive(!isEasy);
    }

    public void LoadParkourLevel(string levelKey)
    {
        DataManager.SelectedLevel = levelKey;
        SceneManager.LoadScene("GameScene");
    }

    public void RefreshCurrency()
    {
        if (coinText != null)
        {
            coinText.text = DataManager.TotalCoins.ToString();
        }
    }

    private void InitSettings()
    {
        if (musicToggle != null) musicToggle.SetState(DataManager.IsMusicOn, animate: false);
        if (sfxToggle != null) sfxToggle.SetState(DataManager.IsSfxOn, animate: false);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(DataManager.MusicVolume);
            AudioManager.Instance.SetSFXVolume(DataManager.SFXVolume);
        }
    }
}
