using UnityEngine;
using UnityEngine.EventSystems;
using static DG.Tweening.DOTween;
using DG.Tweening;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale Animation")]
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _clickScale = 0.95f;
    [SerializeField] private float _animationDuration = 0.2f;

    [Header("Wobble Settings")]
    [SerializeField] private float _wobbleDuration = 0.8f;
    [SerializeField] private float _wobbleAmount = 5f;

    [Header("Hover Sound")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _hoverClips;

    private Vector3 _originalScale;
    private Tween _currentScaleTween;

    private void Awake()
    {
        _originalScale = transform.localScale;

        // Гарантируем, что звук не играет сам по себе
        if (_audioSource != null)
            _audioSource.playOnAwake = false;
    }

    private void Start()
    {
        // ЗАПУСКАЕМ ПОКАЧИВАНИЕ И НИКОГДА НЕ ОСТАНАВЛИВАЕМ
        transform.DORotate(new Vector3(0, 0, _wobbleAmount), _wobbleDuration / 2f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetLink(gameObject); // Защита от утечек
    }

    private void OnDisable()
    {
        // Очищаем scale-анимацию при отключении
        _currentScaleTween?.Kill();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimateScale(_originalScale * _hoverScale);
        PlayRandomSound();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateScale(_originalScale);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AnimateScale(_originalScale * _clickScale);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AnimateScale(_originalScale * _hoverScale);
    }

    private void AnimateScale(Vector3 targetScale)
    {
        _currentScaleTween?.Kill();
        _currentScaleTween = transform.DOScale(targetScale, _animationDuration)
            .SetEase(Ease.OutBack);
    }

    private void PlayRandomSound()
    {
        if (_audioSource == null || _hoverClips == null || _hoverClips.Length == 0)
            return;

        int randomIndex = Random.Range(0, _hoverClips.Length);
        AudioClip clip = _hoverClips[randomIndex];
        _audioSource.PlayOneShot(clip);
    }
}