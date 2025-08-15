using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartMenu : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Transform[] _objects; // основные объекты меню (кнопки)
    [SerializeField] private float _slideDuration = 0.5f;
    [SerializeField] private float _delayBetweenObjects = 0.3f;
    [SerializeField] private float _slideDistance = 500f;

    [Header("Credits Window")]
    [SerializeField] private GameObject creditsWindow; // панель "Об авторах"

    [Header("Settings Window")]
    [SerializeField] private GameObject settingsWindow; // панель "Настройки"

    [Header("Анимации")]
    [SerializeField] private float creditsScaleDuration = 0.4f;
    [SerializeField] private Ease creditsEase = Ease.OutBack;

    [Header("Звук клика на кнопку")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _soundClips;

    [Header("Global Light")]
    [SerializeField] private Light _globalLight;
    [SerializeField] private Color targetColor;
    [SerializeField] private float duration;

    private Vector3[] _targetLocalPositions;
    private Vector3 _creditsInitialScale;
    private Vector3 _settingsInitialScale;
    private bool _isAnimating;

    private void Awake()
    {
        CacheTargetLocalPositions();
        PrepareForAnimation();

        if (_audioSource != null)
            _audioSource.playOnAwake = false;

        // Сохраняем оригинальный scale окон
        if (creditsWindow != null)
        {
            _creditsInitialScale = creditsWindow.transform.localScale;
            creditsWindow.SetActive(false);
            creditsWindow.transform.localScale = Vector3.zero;
        }
        else
        {
            Debug.LogWarning("creditsWindow не назначен в инспекторе.");
        }

        if (settingsWindow != null)
        {
            _settingsInitialScale = settingsWindow.transform.localScale;
            settingsWindow.SetActive(false);
            settingsWindow.transform.localScale = Vector3.zero;
        }
        else
        {
            Debug.LogWarning("settingsWindow не назначен в инспекторе.");
        }
    }

    private void OnEnable()
    {
        if (!_isAnimating)
        {
            PrepareForAnimation();
            AnimateEnter();
        }
    }

    public void PlayRandomSound()
    {
        if (_audioSource == null || _soundClips == null || _soundClips.Length == 0)
            return;

        int randomIndex = Random.Range(0, _soundClips.Length);
        _audioSource.PlayOneShot(_soundClips[randomIndex]);
    }

    public void ChangeLightColorOverTime()
    {
        if (_globalLight == null)
        {
            Debug.LogWarning("Global Light reference is null.");
            return;
        }

        DOTween.To(
            () => _globalLight.color,
            color => _globalLight.color = color,
            targetColor,
            duration
        ).SetEase(Ease.Linear);
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
                    .OnComplete(() =>
                    {
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
                    .OnComplete(() =>
                    {
                        _objects[index].gameObject.SetActive(false);

                        if (index == _objects.Length - 1)
                            _isAnimating = false;
                    });
            });
        }
    }

    // ✅ ОТКРЫТИЕ ОКНА "ОБ АВТОРАХ"
    public void OpenCredits()
    {
        if (creditsWindow == null) return;

        PlayRandomSound();

        AnimateExit();

        DOVirtual.DelayedCall(_objects.Length * _delayBetweenObjects + _slideDuration, () =>
        {
            creditsWindow.SetActive(true);
            creditsWindow.transform.localScale = Vector3.zero;
            creditsWindow.transform.DOScale(_creditsInitialScale, creditsScaleDuration)
                .SetEase(creditsEase);
        });
    }

    // ✅ ЗАКРЫТИЕ ОКНА "ОБ АВТОРАХ"
    public void CloseCredits()
    {
        if (creditsWindow == null) return;

        PlayRandomSound();

        creditsWindow.transform.DOScale(Vector3.zero, creditsScaleDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                creditsWindow.SetActive(false);
                PrepareForAnimation();
                AnimateEnter();
            });
    }

    // ✅ ОТКРЫТИЕ ОКНА НАСТРОЕК
    public void OpenSettings()
    {
        if (settingsWindow == null) return;

        PlayRandomSound();

        AnimateExit();

        DOVirtual.DelayedCall(_objects.Length * _delayBetweenObjects + _slideDuration, () =>
        {
            settingsWindow.SetActive(true);
            settingsWindow.transform.localScale = Vector3.zero;
            settingsWindow.transform.DOScale(_settingsInitialScale, creditsScaleDuration)
                .SetEase(creditsEase);
        });
    }

    // ✅ ЗАКРЫТИЕ ОКНА НАСТРОЕК
    public void CloseSettings()
    {
        if (settingsWindow == null) return;

        PlayRandomSound();

        settingsWindow.transform.DOScale(Vector3.zero, creditsScaleDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                settingsWindow.SetActive(false);
                PrepareForAnimation();
                AnimateEnter();
            });
    }
}