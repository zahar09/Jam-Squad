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

    [Header("Задержки")]
    [SerializeField] private float delayPlayer = 1f;
    [SerializeField] private float delayAfterPlayer = 1.5f;
    [SerializeField] private float delayBetweenHormones = 0.3f;
    [SerializeField] private float delayAfterCell = 1.5f; // Задержка после появления клетки
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

        // Отвязываем от родителя, сохраняя мировые координаты
        hormonesContainer.transform.SetParent(null, false);
        hormonesContainer.transform.position = worldPosition;
        hormonesContainer.transform.rotation = worldRotation;

        // Активируем контейнер
        hormonesContainer.SetActive(true);

        // Подготовка к анимации
        Transform[] hormoneTransforms = new Transform[hormonesContainer.transform.childCount];
        Vector3[] targetScales = new Vector3[hormonesContainer.transform.childCount];

        for (int i = 0; i < hormonesContainer.transform.childCount; i++)
        {
            Transform child = hormonesContainer.transform.GetChild(i);
            hormoneTransforms[i] = child;
            targetScales[i] = child.localScale;
            child.localScale = Vector3.zero; // начальный масштаб
            child.gameObject.SetActive(false); // скрываем до анимации
        }

        // Запускаем поочерёдную анимацию гормонов
        StartCoroutine(AnimateHormonesSequentially(hormoneTransforms, targetScales));
    }

    private IEnumerator AnimateHormonesSequentially(Transform[] hormones, Vector3[] targetScales)
    {
        for (int i = 0; i < hormones.Length; i++)
        {
            Transform hormone = hormones[i];
            if (hormone == null) continue;

            hormone.gameObject.SetActive(true);
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
                // После анимации клетки — ждём и показываем две группы гормонов
                Invoke(nameof(ShowNextHormoneGroups), delayAfterCell);
            });
    }

    private void ShowNextHormoneGroups()
    {
        // Показываем вторую и третью группу гормонов ОДНОВРЕМЕННО
        ShowHormones(secondhormonesContainer);
        ShowHormones(thirdhormonesContainer);
    }

    public void StartGame()
    {
        Vector3 worldPosition = game.transform.position;
        Quaternion worldRotation = game.transform.rotation;
        Vector3 targetScale = game.transform.localScale;

        game.transform.SetParent(null, false);
        game.transform.position = worldPosition;
        game.transform.rotation = worldRotation;
        game.SetActive(true);
    }
}