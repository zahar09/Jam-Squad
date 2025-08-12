using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartMenu : MonoBehaviour, IPointerClickHandler
{
    [Header("Animation Settings")]
    [SerializeField] private Transform[] _objects;
    [SerializeField] private float _slideDuration = 0.5f;
    [SerializeField] private float _delayBetweenObjects = 0.3f;
    [SerializeField] private float _slideDistance = 500f;

    [Header("Звук при клике на кнопку")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _soundClips;

    private Vector3[] _targetLocalPositions;
    private bool _isAnimating;

    private void Awake()
    {
        CacheTargetLocalPositions();
        PrepareForAnimation();

        if (_audioSource != null)
            _audioSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        if (!_isAnimating)
        {
            PrepareForAnimation();
            AnimateEnter();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayRandomSound();
    }

    public void PlayRandomSound()
    {
        if (_audioSource == null || _soundClips == null || _soundClips.Length == 0)
            return;

        int randomIndex = Random.Range(0, _soundClips.Length);
        _audioSource.PlayOneShot(_soundClips[randomIndex]);
    }

    private void CacheTargetLocalPositions()
    {
        _targetLocalPositions = new Vector3[_objects.Length];
        for (int i = 0; i < _objects.Length; i++)
        {
            _targetLocalPositions[i] = _objects[i].localPosition;
        }
    }

    private void PrepareForAnimation()
    {
        for (int i = 0; i < _objects.Length; i++)
        {
            Vector3 startPos = _targetLocalPositions[i];
            startPos.x -= _slideDistance;
            _objects[i].localPosition = startPos;
            _objects[i].gameObject.SetActive(false);
        }
    }

    public void AnimateEnter()
    {
        _isAnimating = true;

        for (int i = 0; i < _objects.Length; i++)
        {
            int index = i;
            DOVirtual.DelayedCall(i * _delayBetweenObjects, () =>
            {
                _objects[index].gameObject.SetActive(true);
                _objects[index].DOLocalMoveX(_targetLocalPositions[index].x, _slideDuration)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() => {
                        if (index == _objects.Length - 1)
                            _isAnimating = false;
                    });
            });
        }
    }

    public void AnimateExit()
    {
        _isAnimating = true;

        for (int i = 0; i < _objects.Length; i++)
        {
            int index = i;
            DOVirtual.DelayedCall(i * _delayBetweenObjects, () =>
            {
                Vector3 endPos = _targetLocalPositions[index];
                endPos.x -= _slideDistance;

                _objects[index].DOLocalMoveX(endPos.x, _slideDuration)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => {
                        _objects[index].gameObject.SetActive(false);

                        if (index == _objects.Length - 1)
                            _isAnimating = false;
                    });
            });
        }
    }
}