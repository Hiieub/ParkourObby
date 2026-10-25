using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPopup : MonoBehaviour
{
    [Header("Tabs")]
    [SerializeField] private Button easyTabBtn;
    [SerializeField] private Button noramlTabBtn;

    [Header("Containers")]
    [SerializeField] private GameObject easyCardsContainer;
    [SerializeField] private GameObject normalCardsContainer;

    [SerializeField] private Button backBtn;

    private bool isEasyTabActive = true;

    private UIPopupAnimation popupAnim;

    private void Awake()
    {
        popupAnim = GetComponent<UIPopupAnimation>();

        if (easyTabBtn != null) easyTabBtn.onClick.AddListener(() => SwitchTab(true));
        if (noramlTabBtn != null) noramlTabBtn.onClick.AddListener(() => SwitchTab(false));
        if (backBtn != null) backBtn.onClick.AddListener(CloseLevelSelect);
    }

    public void OpenLevelSelect()
    {
        if (popupAnim != null) popupAnim.Open();
        else gameObject.SetActive(true);

        var levelManager = GetComponentInChildren<LevelSelectManager>();
        if (levelManager != null) levelManager.RefreshAllCards();

        SwitchTab(isEasyTabActive);
    }

    public void CloseLevelSelect()
    {
        if (popupAnim != null) popupAnim.Close();
        else gameObject.SetActive(false);

        if (MainMenuController.Instance != null)
        {
            MainMenuController.Instance.ShowMainMenu();
        }
    }

    private void SwitchTab(bool isEasy)
    {
        isEasyTabActive = isEasy;

        if (easyCardsContainer != null) easyCardsContainer.SetActive(isEasy);
        if (normalCardsContainer != null) normalCardsContainer.SetActive(!isEasy);
    }
}
