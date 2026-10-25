using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance { get; private set; }

    [Header("Main Menu Screen")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private TMP_Text coinText;

    [Header("Main Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button skinsButton;
    [SerializeField] private Button settingsButton;

    [Header("Sub Popup")]
    [SerializeField] private LevelSelectPopup levelSelectPopup;
    [SerializeField] private ShopPopup shopPopup;
    [SerializeField] private SettingsPopupUI settingsPopup;

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
        RegisterButtons();
        RefreshCurrency();
        ShowMainMenu();

        if (settingsPopup != null)
        {
            settingsPopup.SetupAudio();
        }
    }

    private void RegisterButtons()
    {
        if (playButton != null) playButton.onClick.AddListener(() =>
        {
            if (mainPanel != null) mainPanel.SetActive(false);
            if (levelSelectPopup != null) levelSelectPopup.OpenLevelSelect();
        });

        if (skinsButton != null) skinsButton.onClick.AddListener(() =>
        {
            if (mainPanel != null) mainPanel.SetActive(false);
            if (shopPopup != null) shopPopup.OpenShop();
        });

        if (settingsButton != null) settingsButton.onClick.AddListener(() =>
        {
            if (settingsPopup != null) settingsPopup.Show();
        });
    }

    public void ShowMainMenu()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
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
}
