using UnityEngine;
using UnityEngine.UI;

public class BtnClickedSfx : MonoBehaviour
{
    [Header("Custom Clips")]
    [SerializeField] private AudioClip customClickClip;
    //[SerializeField] private bool playHoverSound = false;

    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();

        if (btn != null)
        {
            btn.onClick.AddListener(PlayClickSound);
        }
        else
        {
            Debug.LogWarning($"[BtnClickedSfx] No Button component on {gameObject.name}", gameObject);
        }
    }

    private void PlayClickSound()
    {
        if (AudioManager.Instance == null) return;

        if (customClickClip != null)
        {
            AudioManager.Instance.PlaySFX(customClickClip);
        }
        else
        {
            AudioManager.Instance.BtnClickedSfx();
        }
    }

    private void OnDestroy()
    {
        if (btn != null)
        {
            btn.onClick.RemoveListener(PlayClickSound);
        }
    }
}
