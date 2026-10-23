using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Database & Spawning")]
    [SerializeField] private SkinDatabaseSO skinDatabase;
    [SerializeField] private Transform contentParent;
    [SerializeField] private ShopItemUI skinItemPrefab;

    [Header("Optional UI")]
    [SerializeField] private TMP_Text coinsTxt;

    private List<ShopItemUI> spawnedItems = new List<ShopItemUI>();

    private void Start()
    {
        GenerateShopItems();
        UpdateCoinUI();
    }

    private void GenerateShopItems()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        spawnedItems.Clear();

        foreach (SkinData data in skinDatabase.skins)
        {
            ShopItemUI newItem = Instantiate(skinItemPrefab, contentParent);
            newItem.Setup(data, this);
            spawnedItems.Add(newItem);
        }
    }

    public void OnSkinItemClicked(SkinData skin)
    {
        bool isUnlocked = DataManager.IsSkinUnlocked(skin.skinId, skin.price);

        if (isUnlocked)
        {
            DataManager.SelectedSkin = skin.skinId;
        }
        else
        {
            if (DataManager.TotalCoins >= skin.price)
            {
                DataManager.TotalCoins -= skin.price;
                DataManager.UnlockSkin(skin.skinId);
                DataManager.SelectedSkin = skin.skinId;
                UpdateCoinUI();
            }else
            {
                Debug.Log("[ShopManager] ko đủ tiền mua!");
                return;
            }
        }

        RefreshAllItems();
    }

    private void RefreshAllItems()
    {
        foreach (var item in spawnedItems)
        {
            item.RefreshUI();
        }
    }

    private void UpdateCoinUI()
    {
        if (coinsTxt != null)
        {
            coinsTxt.text = DataManager.TotalCoins.ToString();
        }
    }
}
