using UnityEngine;
using UnityEngine.UI;

public class ShopPopup : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private ShopManager shopManager;
    private UIPopupAnimation popupAnim;

    private void Awake()
    {
        popupAnim = GetComponent<UIPopupAnimation>();

        if (backBtn != null) backBtn.onClick.AddListener(CloseShop);
    }

    public void OpenShop()
    {
        if (popupAnim != null) popupAnim.Open();
        else gameObject.SetActive(true);

        shopManager.UpdateCoinUI();
        shopManager.RefreshAllItems();
    }

    public void CloseShop()
    {
        if (popupAnim != null) popupAnim.Close();
        else gameObject.SetActive(false);

        if (MainMenuController.Instance != null)
        {
            MainMenuController.Instance.ShowMainMenu();
            MainMenuController.Instance.RefreshCurrency();
        }
    }
}
