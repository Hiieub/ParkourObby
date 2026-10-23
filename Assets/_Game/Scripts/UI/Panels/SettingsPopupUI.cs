using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopupUI : MonoBehaviour
{
    [Header("Navigation Buttons")]
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnContinue;
    [SerializeField] private Button btnRetry;
    [SerializeField] private Button btnHome;

    [Header("Audio Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public void Initialize(Action onCloseOrResume, Action onRetry, Action onHome)
    {
        if (btnClose != null)
        {
            btnClose.onClick.RemoveAllListeners();
            btnClose.onClick.AddListener(() => onCloseOrResume?.Invoke());
        }

        if (btnContinue != null)
        {
            btnContinue.onClick.RemoveAllListeners();
            btnContinue.onClick.AddListener(() => onCloseOrResume?.Invoke());
        }

        if (btnRetry != null)
        {
            btnRetry.onClick.RemoveAllListeners();
            btnRetry.onClick.AddListener(() => onRetry?.Invoke());
        }

        if (btnHome != null)
        {
            btnHome.onClick.RemoveAllListeners();
            btnHome.onClick.AddListener(() => onHome?.Invoke());
        }

        SetupAudioSliders();
    }

    private void SetupAudioSliders()
    {
        if (musicSlider != null)
        {
            musicSlider.value = DataManager.MusicVolume;
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = DataManager.SFXVolume;
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(DataManager.MusicVolume);
            AudioManager.Instance.SetSFXVolume(DataManager.SFXVolume);
        }
    }

    private void OnMusicChanged(float val)
    {
        DataManager.MusicVolume = val;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(val);
        }
    }

    private void OnSfxChanged(float val)
    {
        DataManager.SFXVolume = val;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(val);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if (musicSlider != null) musicSlider.value = DataManager.MusicVolume;
        if (sfxSlider != null) sfxSlider.value = DataManager.SFXVolume;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
