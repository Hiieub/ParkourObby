using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM Clips")]
    [SerializeField] private AudioClip mainBgm;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip jumpSfx;
    [SerializeField] private AudioClip coinSfx;
    [SerializeField] private AudioClip checkpointSfx;
    [SerializeField] private AudioClip bounceSfx;
    [SerializeField] private AudioClip respawnSfx;
    [SerializeField] private AudioClip winSfx;
    [SerializeField] private AudioClip btnClickedSfx;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource.loop = true;
        sfxSource.loop = false;
    }

    private void Start()
    {
        if (mainBgm != null && !bgmSource.isPlaying)
        {
            PlayBGM(mainBgm);
        }
    }

    public void PlayBGM(AudioClip clip, float volume = 0.5f)
    {
        if (bgmSource == null || clip == null) return;
        bgmSource.clip = clip;
        bgmSource.volume = volume;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayJump() => PlaySFX(jumpSfx);
    public void PlayCoin() => PlaySFX(coinSfx);
    public void PlayCheckpoint() => PlaySFX(checkpointSfx);
    public void PlayBounce() => PlaySFX(bounceSfx);
    public void PlayRespawn() => PlaySFX(respawnSfx);
    public void PlayWin() => PlaySFX(winSfx);
    public void BtnClickedSfx() => PlaySFX(btnClickedSfx);

    public void SetMusicVolume(float volume)
    {
        if (bgmSource != null) bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume;
    }
}
