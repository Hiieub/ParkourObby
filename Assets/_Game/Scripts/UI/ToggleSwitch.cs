using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform handle;
    [SerializeField] private Image background;
    [SerializeField] private Button clickBtn;

    [Header("Settings")]
    [SerializeField] private float offPosX = -35f;
    [SerializeField] private float onPosX = 35f;
    [SerializeField] private Color offColor = new Color(0.45f, 0.45f, 0.45f);
    [SerializeField] private Color onColor = new Color(0.18f, 0.8f, 0.35f);
    [SerializeField] private float duration = 0.2f;

    public bool IsOn { get; private set; }
    public event Action <bool> OnToggled;

    private void Awake()
    {
        if (clickBtn == null) clickBtn = GetComponent<Button>();
        if (clickBtn != null) clickBtn.onClick.AddListener(Toggle);
    }

    public void SetState(bool isOn, bool animate = false)
    {
        IsOn = isOn;
        float targetX = isOn ? onPosX : offPosX;
        Color targetColor = isOn ? onColor : offColor;

        if (handle != null)
        {
            handle.DOKill();
            if (animate) handle.DOAnchorPosX(targetX, duration).SetEase(Ease.OutQuad).SetUpdate(true);
            else handle.anchoredPosition = new Vector2(targetX, handle.anchoredPosition.y);
        }

        if (background != null)
        {
            background.DOKill();
            if (animate) background.DOColor(targetColor, duration).SetUpdate(true);
            else background.color = targetColor;
        }
    }

    private void Toggle()
    {
        SetState(!IsOn, animate: true);
        OnToggled?.Invoke(IsOn);
    }
}
