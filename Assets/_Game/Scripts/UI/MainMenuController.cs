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
    [SerializeField] private int coinTest;

    [Header("Settings Controls")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

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

        // Settings Sliders
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
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
            coinTest = DataManager.TotalCoins;
            //coinText.text = DataManager.TotalCoins.ToString();
            coinText.text = coinTest.ToString();
        }
    }

    private void InitSettings()
    {
        float musicVol = DataManager.MusicVolume;
        float sfxVol = DataManager.SFXVolume;

        if (musicSlider != null) musicSlider.value = musicVol;
        if (sfxSlider != null) sfxSlider.value = sfxVol;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(musicVol);
            AudioManager.Instance.SetSFXVolume(sfxVol);
        }
    }

    private void OnMusicVolumeChanged(float val)
    {
        DataManager.MusicVolume = val;
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusicVolume(val);
    }

    private void OnSfxVolumeChanged(float val)
    {
        DataManager.SFXVolume = val;
        if (AudioManager.Instance != null) AudioManager.Instance.SetSFXVolume(val);
    }
}
