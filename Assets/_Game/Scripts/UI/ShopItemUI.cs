using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private Button acctionBtn;
    [SerializeField] private Image btnImage;
    [SerializeField] private TMP_Text btnTxt;

    [Header("Button Colors")]
    [SerializeField] private Color equippedColor = new Color(0.32f, 0.32f, 0.32f);
    [SerializeField] private Color equipColor = new Color(0.28f, 0.52f, 0.85f);
    [SerializeField] private Color buyColor = new Color(0f, 0.9f, 0f);

    private SkinData skinData;
    private ShopManager shopManager;

    public void Setup(SkinData data, ShopManager manager)
    {
        skinData = data;
        shopManager = manager;

        if (nameTxt != null) nameTxt.text = data.skinName;
        if (iconImage != null && data.icon != null) iconImage.sprite = data.icon;

        acctionBtn.onClick.RemoveListener(OnBtnClicked);
        acctionBtn.onClick.AddListener(OnBtnClicked);

        RefreshUI();
    }

    public void RefreshUI()
    {
        bool isUnlocked = DataManager.IsSkinUnlocked(skinData.skinId, skinData.price);
        bool isEquipped = DataManager.SelectedSkin == skinData.skinId;

        if (isEquipped)
        {
            btnTxt.text = "EQUIPPED";
            btnImage.color = equippedColor;
            acctionBtn.interactable = false;
        }
        else if (isUnlocked)
        {
            btnTxt.text = "EQUIP";
            btnImage.color = equipColor;
            acctionBtn.interactable = true;
        }
        else
        {
            btnTxt.text = $"BUY {skinData.price}";
            btnImage.color = buyColor;
            acctionBtn.interactable = true;
        }
    }

    private void OnBtnClicked()
    {
        shopManager.OnSkinItemClicked(skinData);
    }
}
