using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Education : MonoBehaviour
{
    [Header("Объекты")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject hormonesContainer; // Дочерний объект игрока (содержит гормоны)
    [SerializeField] private GameObject firstCell;

    [Header("Задержки")]
    [SerializeField] private float delayPlayer = 1f;
    [SerializeField] private float delayAfterPlayer = 1.5f;
    [SerializeField] private float delayBetweenHormones = 0.3f;
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

        if (hormonesContainer != null)
        {
            // Отключаем контейнер, но не трогаем иерархию
            hormonesContainer.SetActive(false);
        }
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
                Invoke(nameof(ShowHormones), delayAfterPlayer);
            });
    }

    private void ShowHormones()
    {
        if (hormonesContainer == null) return;

        // 1. Сохраняем мировую позицию и вращение контейнера
        Vector3 worldPosition = hormonesContainer.transform.position;
        Quaternion worldRotation = hormonesContainer.transform.rotation;

        // 2. Отвязываем от родителя, сохраняя позицию в мире
        hormonesContainer.transform.SetParent(null, false);
        hormonesContainer.transform.position = worldPosition;
        hormonesContainer.transform.rotation = worldRotation;

        // 3. Включаем контейнер, чтобы дети стали активны
        hormonesContainer.SetActive(true);

        // 4. Анимируем каждого гормона (дочернего объекта) по очереди
        Transform[] hormoneTransforms = new Transform[hormonesContainer.transform.childCount];
        Vector3[] localScales = new Vector3[hormonesContainer.transform.childCount];

        // Сохраняем начальные масштабы (на случай, если у них разные)
        for (int i = 0; i < hormonesContainer.transform.childCount; i++)
        {
            Transform child = hormonesContainer.transform.GetChild(i);
            hormoneTransforms[i] = child;
            localScales[i] = child.localScale;

            // Устанавливаем scale в 0 для анимации
            child.localScale = Vector3.zero;
        }

        // Запускаем анимацию поочерёдно
        StartCoroutine(AnimateHormonesSequentially(hormoneTransforms, localScales));
    }

    private IEnumerator AnimateHormonesSequentially(Transform[] hormones, Vector3[] targetScales)
    {
        for (int i = 0; i < hormones.Length; i++)
        {
            Transform hormone = hormones[i];
            if (hormone == null) continue;
            
            hormone.gameObject.SetActive(true);
            // Анимируем появление
            hormone.DOScale(targetScales[i], animationDuration)
                   .SetEase(easeCurve);

            yield return new WaitForSeconds(delayBetweenHormones);
        }
    }

    public void ActivateCell()
    {
        if (firstCell == null) return;

        Vector3 worldPosition = firstCell.transform.position;
        Quaternion worldRotation = firstCell.transform.rotation;
        Vector3 tartgetScale = firstCell.transform.localScale;

        firstCell.transform.SetParent(null, false);
        firstCell.transform.position = worldPosition;
        firstCell.transform.rotation = worldRotation;
        firstCell.SetActive(true);
        firstCell.transform.localScale = Vector3.zero;
        firstCell.transform.DOScale(tartgetScale, animationDuration)
            .SetEase(easeCurve);
    }
}