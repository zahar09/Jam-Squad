using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private Transform[] cells;
    [SerializeField] private Transform[] hormonesContainers;
    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;
    [SerializeField] private ArrowPointer arrow;
    [SerializeField] private Transform[] additionalObjects;

    [Header("Настройки победы")]
    [SerializeField] private Camera victoryCamera;
    [SerializeField] private Transform cameraWinTarget;
    [SerializeField] private Transform[] winObjects;
    [SerializeField] private Image fadeImage;

    [Space]
    [SerializeField] private float cameraMoveTime = 2f;
    [SerializeField] private float objectsAppearDelay = 1f;
    [SerializeField] private float objectsAnimationTime = 1f;
    [SerializeField] private float objectsDisplayTime = 3f;
    [SerializeField] private float fadeTime = 1f;

    private bool objectsInitialized;

    private void Start()
    {
        InitializeObjects();
    }

    void InitializeObjects()
    {
        if (objectsInitialized) return;

        foreach (var obj in winObjects)
        {
            obj.gameObject.SetActive(false);
        }

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(false);
        }

        objectsInitialized = true;
    }

    public void ActivateCell(int cellIndex)
    {
        if (cellIndex < 0 || cellIndex >= cells.Length) return;

        if (!objectsInitialized)
        {
            InitializeObjects();
        }

        DetachAllObjects();
        var cell = cells[cellIndex];
        cell.gameObject.SetActive(true);

        Vector3 targetScale = cell.localScale; // Сохраняем оригинальный масштаб
        cell.localScale = Vector3.zero;
        cell.DOScale(targetScale, scaleDuration)
            .SetEase(scaleEase)
            .OnComplete(() => ReleaseHormones(cellIndex));

        if (arrow != null)
        {
            arrow.gameObject.SetActive(true);
            arrow.SetTarget(cell);
        }
    }

    void DetachAllObjects()
    {
        foreach (var cell in cells) cell.SetParent(null);
        foreach (var container in hormonesContainers) container.SetParent(null);
        foreach (var obj in additionalObjects) if (obj != null) obj.SetParent(null);
    }

    public void WinGame()
    {
        if (victoryCamera == null || cameraWinTarget == null) return;

        var seq = DOTween.Sequence();

        seq.Append(victoryCamera.transform.DOMove(cameraWinTarget.position, cameraMoveTime));
        seq.Join(victoryCamera.transform.DORotate(cameraWinTarget.eulerAngles, cameraMoveTime));

        seq.AppendInterval(objectsAppearDelay);

        foreach (var obj in winObjects)
        {
            obj.gameObject.SetActive(true);
            Vector3 targetScale = obj.localScale; // Оригинальный масштаб
            obj.localScale = Vector3.zero;
            seq.Join(obj.DOScale(targetScale, objectsAnimationTime).SetEase(scaleEase));
        }

        seq.AppendInterval(objectsDisplayTime);

        foreach (var obj in winObjects)
        {
            seq.Join(obj.DOScale(Vector3.zero, objectsAnimationTime).SetEase(scaleEase));
        }

        if (fadeImage != null)
        {
            seq.AppendCallback(() => {
                fadeImage.gameObject.SetActive(true);
                fadeImage.DOFade(1f, fadeTime);
            });
        }
    }

    void ReleaseHormones(int index)
    {
        if (index < 0 || index >= hormonesContainers.Length) return;

        var container = hormonesContainers[index];
        container.gameObject.SetActive(true);

        foreach (Transform hormone in container)
        {
            Vector3 targetScale = hormone.localScale; // Оригинальный масштаб
            hormone.localScale = Vector3.zero;
            hormone.gameObject.SetActive(true);
            hormone.DOScale(targetScale, scaleDuration).SetEase(scaleEase);
        }
    }
}