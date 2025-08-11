using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _animationDuration = 0.2f;

    private Vector3 _originalScale;
    private Tween _currentTween;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimateScale(_originalScale * _hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateScale(_originalScale);
    }

    private void AnimateScale(Vector3 targetScale)
    {
        _currentTween?.Kill();
        _currentTween = transform.DOScale(targetScale, _animationDuration)
            .SetEase(Ease.OutBack);
    }
}