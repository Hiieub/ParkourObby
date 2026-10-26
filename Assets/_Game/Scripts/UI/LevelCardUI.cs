using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCardUI : MonoBehaviour
{
    private LevelConfigSO levelConfig;
    private LevelSelectManager levelSelectManager;

    [SerializeField] private TMP_Text levelNameTxt;
    [SerializeField] private Image thumbnailImage;
    [SerializeField] private Image[] starImages;
    [SerializeField] private Sprite starFilled;
    [SerializeField] private Sprite starEmpty;

    [SerializeField] private Button playButton;

    public void Setup(LevelConfigSO config, LevelSelectManager manager)
    {
        levelConfig = config;
        levelSelectManager = manager;

        if (levelNameTxt != null)
            levelNameTxt.text = config.DisplayName;

        if (thumbnailImage != null && config.Thumbnail != null)
            thumbnailImage.sprite = config.Thumbnail;
        
        playButton.onClick.RemoveListener(OnPlayClicked);
        playButton.onClick.AddListener(OnPlayClicked);

        RefreshStars();
    }

    public void RefreshStars()
    {
        if (levelConfig == null || starImages == null || starImages.Length == 0) return;
        int starsEarned = DataManager.GetStars(levelConfig.LevelId);
        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] == null) continue;
            if (i < starsEarned && starFilled != null)
            {
                starImages[i].sprite = starFilled;
                starImages[i].color = Color.white;
            }
            else
            {
                if (starEmpty != null) starImages[i].sprite = starEmpty;
                starImages[i].color = new Color(0.6f, 0.6f, 0.6f, 0.7f);
            }
        }
    }

    private void OnPlayClicked()
    {
        if (levelConfig == null) return;

        levelSelectManager.OnLevelCardClicked(levelConfig);
    }
    private void OnDestroy()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
        }
    }
}
