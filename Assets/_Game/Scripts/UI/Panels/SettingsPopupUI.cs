using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopupUI : MonoBehaviour
{
    private UIPopupAnimation popupAnim;

    [Header("Navigation Buttons")]
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnContinue;
    [SerializeField] private Button btnRetry;
    [SerializeField] private Button btnHome;

    [Header("Audio Toggles")]
    [SerializeField] private ToggleSwitch musicToggle;
    [SerializeField] private ToggleSwitch sfxToggle;

    private void Awake()
    {
        popupAnim = GetComponent<UIPopupAnimation>();

        if (btnClose != null) btnClose.onClick.AddListener(Hide);
    }

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

        SetupAudioToggles();
    }

    public void SetupAudio() => SetupAudioToggles();
    private void SetupAudioToggles()
    {
        if (musicToggle != null)
        {
            musicToggle.SetState(DataManager.IsMusicOn, animate: true);
            musicToggle.OnToggled += (isOn) =>
            {
                DataManager.IsMusicOn = isOn;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.SetMusicVolume(DataManager.MusicVolume);
                }
            };
        }

        if (sfxToggle != null)
        {
            sfxToggle.SetState(DataManager.IsSfxOn, animate: false);
            sfxToggle.OnToggled += (isOn) =>
            {
                DataManager.IsSfxOn = isOn;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.SetSFXVolume(DataManager.SFXVolume);
                }
            };
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(DataManager.MusicVolume);
            AudioManager.Instance.SetSFXVolume(DataManager.SFXVolume);
        }
    }

    public void Show()
    {
        if (popupAnim != null) popupAnim.Open();
        else gameObject.SetActive(true);

        if (musicToggle != null) musicToggle.SetState(DataManager.IsMusicOn, animate: true);
        if (sfxToggle != null) sfxToggle.SetState(DataManager.IsSfxOn, animate: false);
    }

    public void Hide()
    {
        if (popupAnim != null) popupAnim.Close();
        else gameObject.SetActive(false);
    }
}
