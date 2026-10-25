using DG.Tweening;
using System;
using UnityEngine;

public class UIPopupAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform container;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation Setting")]
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private Ease openEase = Ease.OutBack;
    [SerializeField] private Ease closeEase = Ease.InBack;

    public void Open(Action onComplete = null)
    {
        gameObject.SetActive(true);

        if (container != null)
        {
            container.DOKill();
            container.localScale = Vector3.zero;
            container.DOScale(1f, duration).SetEase(openEase).SetUpdate(true);
        }

        if (canvasGroup != null)
        {
            canvasGroup.DOKill();
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, duration * 0.8f).SetUpdate(true).OnComplete(() => onComplete?.Invoke());
        }
        else
        {
            container?.DOScale(1f, duration).SetEase(openEase).SetUpdate(true).OnComplete(() => onComplete?.Invoke());
        }
    }

    public void Close(Action onComplete = null)
    {
        if (container != null)
        {
            container.DOKill();
            container.DOScale(0f, duration * 0.7f).SetEase(closeEase).SetUpdate(true);
        }

        if (canvasGroup != null)
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(0f, duration * 0.7f).SetUpdate(true).OnComplete(() =>
            {
                gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }
        else
        {
            container?.DOScale(0f, duration * 0.7f).SetEase(closeEase).SetUpdate(true).OnComplete(() =>
            {
                gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }
    }
}
