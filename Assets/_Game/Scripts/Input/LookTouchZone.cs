using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class LookTouchZone : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private CinemachineOrbitalFollow orbital;
    [SerializeField] private float sensitivity = 0.15f;
    [SerializeField] private bool invertY = false;

    private int activePointerId = int.MinValue;
    private float DpiScale => Screen.dpi > 0 ? 160f / Screen.dpi : 1f;

    public void OnPointerDown(PointerEventData e)
    {
        if (activePointerId != int.MinValue) return;
        activePointerId = e.pointerId;
    }

    public void OnDrag(PointerEventData e)
    {
        if (e.pointerId != activePointerId || orbital == null) return;

        Vector2 d = e.delta * sensitivity * DpiScale;

        var h = orbital.HorizontalAxis;
        h.Value = h.ClampValue(h.Value + d.x);
        orbital.HorizontalAxis = h;

        var v = orbital.VerticalAxis;
        v.Value = v.ClampValue(v.Value + (invertY ? d.y : -d.y));
        orbital.VerticalAxis = v;
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (e.pointerId == activePointerId)
            activePointerId = int.MinValue;
    }
}