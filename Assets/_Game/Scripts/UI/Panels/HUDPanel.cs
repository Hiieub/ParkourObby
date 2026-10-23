using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDPanel : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text stageText;

    [Header("Progress Bar")]
    [SerializeField] private Image progressFillImage;
    [SerializeField] private TMP_Text progressText;

    [Header("Buttons")]
    [SerializeField] private Button btnSetting;

    private float targetFillRatio = 0f;
    private bool isInitialized = false;

    private void Update()
    {
        if (progressFillImage != null && isInitialized)
        {
            if (Mathf.Abs(progressFillImage.fillAmount - targetFillRatio) > 0.001f)
            {
                progressFillImage.fillAmount = Mathf.Lerp(progressFillImage.fillAmount, targetFillRatio, Time.deltaTime * 6f);
            }
        }
    }

    public void Initialize(Action onOpenSetting)
    {
        if (btnSetting != null)
        {
            btnSetting.onClick.RemoveAllListeners();
            btnSetting.onClick.AddListener(() => onOpenSetting?.Invoke());
        }

        if (progressFillImage != null)
        {
            progressFillImage.type = Image.Type.Filled;
            progressFillImage.fillMethod = Image.FillMethod.Horizontal;
            progressFillImage.fillAmount = 0f;
        }

        targetFillRatio = 0f;
        isInitialized = true;
    }

    public void UpdateCoins(int totalCoins)
    {
        if (coinText != null)
        {
            coinText.text = totalCoins.ToString();
        }
    }

    public void UpdateTimer(float seconds)
    {
        if (timerText != null)
        {
            int m = Mathf.FloorToInt(seconds / 60f);
            int s = Mathf.FloorToInt(seconds % 60f);
            timerText.text = $"{m:00}:{s:00}";
        }
    }

    public void UpdateStageTitle(string stageTitle)
    {
        if (stageText != null)
        {
            stageText.text = stageTitle;
        }
    }

    public void UpdateProgress(float progressRatio, int currentStage, int totalStages)
    {
        targetFillRatio = Mathf.Clamp01(progressRatio);

        if (progressText != null)
        {
            progressText.text = $"{currentStage} / {totalStages}";
        }
    }

    public void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
}
