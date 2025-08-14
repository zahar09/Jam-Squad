using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Education : MonoBehaviour
{
    [Header("Объекты")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject firsthormonesContainer;
    [SerializeField] private GameObject firstCell;
    [SerializeField] private GameObject secondhormonesContainer;
    [SerializeField] private GameObject thirdhormonesContainer;
    [SerializeField] private GameObject game;

    [Header("Аудио (голоса)")]
    [SerializeField] private AudioSource voice1Source;
    [SerializeField] private AudioSource voice2Source;
    [SerializeField] private AudioSource voice3Source;

    [Header("Задержки")]
    [SerializeField] private float delayPlayer = 1f;
    [SerializeField] private float delayAfterPlayer = 1.5f;
    [SerializeField] private float delayBetweenHormones = 0.3f;
    [SerializeField] private float delayAfterCell = 1.5f;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private AnimationCurve easeCurve = default;

    private void Start()
    {
        if (easeCurve.length == 0)
        {
            easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        DisableAllObjects();
        Invoke(nameof(ShowPlayer), delayPlayer);
    }

    private void DisableAllObjects()
    {
        if (player != null) player.SetActive(false);
        if (firstCell != null) firstCell.SetActive(false);
        if (firsthormonesContainer != null) firsthormonesContainer.SetActive(false);
        if (secondhormonesContainer != null) secondhormonesContainer.SetActive(false);
        if (thirdhormonesContainer != null) thirdhormonesContainer.SetActive(false);
    }

    private void ShowPlayer()
    {
        if (player == null) return;

        player.SetActive(true);
        player.transform.localScale = Vector3.zero;

        player.transform.DOScale(Vector3.one, animationDuration)
            .SetEase(easeCurve)
            .OnComplete(() =>
            {
                // ✅ Голос 1: "Это вы"
                PlayVoice(voice1Source);
                Invoke(nameof(ShowFirstHormones), delayAfterPlayer);
            });
    }

    private void ShowFirstHormones()
    {
        ShowHormones(firsthormonesContainer);
    }

    private void ShowHormones(GameObject hormonesContainer)
    {
        if (hormonesContainer == null) return;

        // Сохраняем мировые позицию и вращение
        Vector3 worldPosition = hormonesContainer.transform.position;
        Quaternion worldRotation = hormonesContainer.transform.rotation;

        hormonesContainer.transform.SetParent(null, false);
        hormonesContainer.transform.position = worldPosition;
        hormonesContainer.transform.rotation = worldRotation;

        hormonesContainer.SetActive(true);

        Transform[] hormoneTransforms = new Transform[hormonesContainer.transform.childCount];
        Vector3[] targetScales = new Vector3[hormonesContainer.transform.childCount];

        for (int i = 0; i < hormonesContainer.transform.childCount; i++)
        {
            Transform child = hormonesContainer.transform.GetChild(i);
            hormoneTransforms[i] = child;
            targetScales[i] = child.localScale;
            child.localScale = Vector3.zero;
            child.gameObject.SetActive(false);
        }


        StartCoroutine(AnimateHormonesSequentially(hormoneTransforms, targetScales, () =>
        {
            if (hormonesContainer == firsthormonesContainer)
                PlayVoice(voice2Source);
        }));
    }

    private IEnumerator AnimateHormonesSequentially(Transform[] hormones, Vector3[] targetScales, System.Action onCompleted = null)
    {
        for (int i = 0; i < hormones.Length; i++)
        {
            Transform hormone = hormones[i];
            if (hormone == null) continue;

            hormone.gameObject.SetActive(true);
            hormone.DOScale(targetScales[i], animationDuration).SetEase(easeCurve);

            yield return new WaitForSeconds(delayBetweenHormones);
        }

        // Все гормоны показаны → вызываем коллбэк
        onCompleted?.Invoke();
    }

    public void ActivateCell()
    {
        if (firstCell == null) return;

        Vector3 worldPosition = firstCell.transform.position;
        Quaternion worldRotation = firstCell.transform.rotation;
        Vector3 targetScale = firstCell.transform.localScale;

        firstCell.transform.SetParent(null, false);
        firstCell.transform.position = worldPosition;
        firstCell.transform.rotation = worldRotation;
        firstCell.SetActive(true);
        firstCell.transform.localScale = Vector3.zero;

        firstCell.transform.DOScale(targetScale, animationDuration)
            .SetEase(easeCurve)
            .OnComplete(() =>
            {
                Invoke(nameof(ShowNextHormoneGroups), delayAfterCell);
            });
    }

    private void ShowNextHormoneGroups()
    {
        ShowHormones(secondhormonesContainer);
        ShowHormones(thirdhormonesContainer);
    }

    public void StartGame()
    {
        PlayVoice(voice3Source);
        // Запускаем игру через короткую задержку, чтобы голос начал играть
        Invoke(nameof(LaunchGame), 2f);
    }

    private void LaunchGame()
    {
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ActivateCell(0);
        }
        else
        {
            Debug.LogError("GameManager не найден в сцене!");
        }
    }

    // Универсальный метод для проигрывания голоса
    private void PlayVoice(AudioSource source)
    {
        if (source != null && source.clip != null)
        {
            source.Play();
        }
        else if (source == null)
        {
            Debug.LogWarning("AudioSource не назначен.");
        }
        else
        {
            Debug.LogWarning("На AudioSource не назначен клип.");
        }
    }
}