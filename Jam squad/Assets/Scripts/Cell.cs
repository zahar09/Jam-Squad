using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private CollectableHolder[] holders;

    [Header("Настройки анимации подбора")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutQuad;

    [Header("Настройка анимации апгрейда")]
    [SerializeField] private GameObject newObj;
    [SerializeField] private Vector3 finalScale = Vector3.one;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease easeType2 = Ease.OutBack;

    [Header("Настройки тряски")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeStrength = 0.2f;
    [SerializeField] private int vibrato = 10;
    [SerializeField] private float randomness = 90f;

    [Header("Sound Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _upgradeSounds;

    [Header("Light Settings (3D)")]
    [SerializeField] private Light _cellLight; // Основной источник света ячейки
    [SerializeField] private Color finalLightColor = Color.green; // Цвет после апгрейда — теперь в инспекторе!

    [Header("Color Animation Settings")]
    [SerializeField] private float colorChangeDuration = 0.5f;
    [SerializeField] private Ease colorEase = Ease.Linear;

    private Vector3 _originalScale;
    private Sequence _shakeSequence;
    private Tweener positionTweener;
    private Tween _colorTween; // Для анимации цвета света
    private int holderIndexToPut = 0;
    private int holderIndexToDestroy = 0;
    private bool removIsStart = false;
    private CellManager cellManager;

    private void Awake()
    {
        _originalScale = transform.localScale;

        if (_audioSource != null)
            _audioSource.playOnAwake = false;
    }

    private void Start()
    {
        cellManager = FindAnyObjectByType<CellManager>();
    }

    private IEnumerator RemoveOneBall()
    {
        yield return new WaitForSeconds(5f);

        while (holderIndexToDestroy >= 0)
        {
            if (holderIndexToDestroy >= holders.Length || holderIndexToDestroy < 0)
            {
                Debug.LogError("Invalid holderIndex: " + holderIndexToDestroy);
                break;
            }

            CollectableHolder currentHolder = holders[holderIndexToDestroy];

            if (currentHolder.collectable != null && currentHolder.collectable.gameObject != null)
            {
                PlayShakeEffect();

                currentHolder.collectable.gameObject.transform.DOScale(Vector3.zero, duration)
                    .SetEase(easeType2)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        Destroy(currentHolder.collectable.gameObject);
                        currentHolder.collectable = null;
                        holderIndexToDestroy--;
                        holderIndexToPut--;

                        if (holderIndexToDestroy < 0)
                        {
                            holderIndexToDestroy = 0;
                            cellManager.LoosGame(gameObject);
                        }
                    });
            }
            else
            {
                Debug.LogWarning("No collectable to remove at index " + holderIndexToDestroy);
            }

            yield return new WaitForSeconds(5f);
        }

        cellManager.LoosGame(gameObject);
    }

    public void TryToUpgrade(GameObject[] collectableObjs)
    {
        if (holderIndexToPut > 2) return;

        for (int i = 0; i < 3; i++)
        {
            collectableObjs[i].transform.SetParent(holders[holderIndexToPut].transform);
            positionTweener = collectableObjs[i].transform.DOLocalMove(Vector3.zero, moveDuration)
                .SetEase(easeType)
                .SetUpdate(true);
        }

        GameObject newObj_ = Instantiate(newObj);
        newObj_.transform.SetParent(holders[holderIndexToPut].transform);
        newObj_.transform.localPosition = Vector3.zero;
        newObj_.transform.localScale = Vector3.zero;
        holders[holderIndexToPut].collectable = newObj_;

        newObj_.transform.DOScale(finalScale, duration)
            .SetEase(easeType2)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                foreach (GameObject obj in collectableObjs)
                {
                    Destroy(obj);
                }
            });

        PlayShakeEffect();
        PlayRandomSound(_upgradeSounds);

        holderIndexToPut++;
        holderIndexToDestroy++;

        if (holderIndexToPut == 3 && !removIsStart)
        {
            ChangeLightColor(finalLightColor);
            cellManager.GetMemoryMessage(gameObject);
            removIsStart = true;
            holderIndexToDestroy = 2;
            StartCoroutine(RemoveOneBall());
        }
        
    }

    public void ChangeLightColor(Color targetColor)
    {
        _colorTween?.Kill(); // Останавливаем предыдущую анимацию

        if (_cellLight == null)
        {
            Debug.LogWarning("Свет (_cellLight) не назначен в инспекторе.");
            return;
        }

        _colorTween = DOTween.To(
                () => _cellLight.color,
                color => _cellLight.color = color,
                targetColor,
                colorChangeDuration
            )
            .SetEase(colorEase)
            .SetUpdate(true); // Работает при timeScale = 0
    }

    public void PlayShakeEffect()
    {
        _shakeSequence?.Kill();

        _shakeSequence = DOTween.Sequence();
        _shakeSequence.Append(transform.DOShakeScale(
                shakeDuration,
                strength: shakeStrength,
                vibrato: vibrato,
                randomness: randomness
            ).SetEase(Ease.OutQuad));

        _shakeSequence.Append(transform.DOScale(_originalScale, 0.1f));
    }

    public void PlayRandomSound(AudioClip[] clips)
    {
        if (_audioSource == null || clips == null || clips.Length == 0) return;

        int randomIndex = Random.Range(0, clips.Length);
        _audioSource.PlayOneShot(clips[randomIndex]);
    }

    private void OnDestroy()
    {
        _shakeSequence?.Kill();
        _colorTween?.Kill();
    }
}