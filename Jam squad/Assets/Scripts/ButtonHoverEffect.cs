using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Animation")]
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _animationDuration = 0.2f;

    [Header("����� ��� ���������")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _hoverClips;

    private Vector3 _originalScale;
    private Tween _currentTween;

    private void Awake()
    {
        _originalScale = transform.localScale;

        // ��������� ����-���������������
        if (_audioSource != null)
            _audioSource.playOnAwake = false;
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

    private void AnimateScale(Vector3 targetScale)
    {
        _currentTween?.Kill();
        _currentTween = transform.DOScale(targetScale, _animationDuration)
            .SetEase(Ease.OutBack);
    }

    private void PlayRandomSound()
    {
        // ���� ��� AudioSource ��� ��� ������ � �������
        if (_audioSource == null || _hoverClips == null || _hoverClips.Length == 0)
            return;

        // �������� ��������� ����
        int randomIndex = Random.Range(0, _hoverClips.Length);
        AudioClip randomClip = _hoverClips[randomIndex];

        // ����������� ���
        _audioSource.PlayOneShot(randomClip);
    }
}