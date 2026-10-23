using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkinData
{
    public string skinId;
    public string skinName;
    public int price;
    public GameObject skinPrefab;
    public Sprite icon;
}

[CreateAssetMenu(fileName = "SkinDatabase", menuName = "GameData/ Skin Database")]
public class SkinDatabaseSO : ScriptableObject
{
    public List<SkinData> skins = new List<SkinData>();

    public SkinData GetSkinById(string id)
    {
        if (string.IsNullOrEmpty(id) && skins.Count > 0)
        {
            return skins[0];
        }

        return skins.Find(s => s.skinId == id) ?? (skins.Count > 0 ? skins[0] : null);
    }
}
