using DG.Tweening;
using UnityEngine;
using System.Collections;
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
    [SerializeField] private float fadeTime = 1f;

    [Header("Подсказка (опционально)")]
    [SerializeField] private GameObject pressAnyKeyText; // Например: Text или Image

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
            obj?.gameObject.SetActive(false);
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

        Vector3 targetScale = cell.localScale;
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
        // Открепление клеток
        foreach (var cell in cells)
        {
            if (cell == null) continue;
            Vector3 worldPosition = cell.position;
            Quaternion worldRotation = cell.rotation;
            cell.SetParent(null, false);
            cell.position = worldPosition;
            cell.rotation = worldRotation;
        }

        // Открепление контейнеров гормонов
        foreach (var container in hormonesContainers)
        {
            if (container == null) continue;
            Vector3 worldPosition = container.position;
            Quaternion worldRotation = container.rotation;
            container.SetParent(null, false);
            container.position = worldPosition;
            container.rotation = worldRotation;
        }

        // Открепление дополнительных объектов
        foreach (var obj in additionalObjects)
        {
            if (obj == null) continue;
            Vector3 worldPosition = obj.position;
            Quaternion worldRotation = obj.rotation;
            obj.SetParent(null, false);
            obj.position = worldPosition;
            obj.rotation = worldRotation;
        }

        // Открепление объектов победы
        foreach (var winObj in winObjects)
        {
            if (winObj == null) continue;
            Vector3 worldPosition = winObj.position;
            Quaternion worldRotation = winObj.rotation;
            winObj.SetParent(null, false);
            winObj.position = worldPosition;
            winObj.rotation = worldRotation;
        }
    }

    public void WinGame()
    {
        if (victoryCamera == null || cameraWinTarget == null) return;

        Player player = FindAnyObjectByType<Player>();
        Destroy(player.gameObject);

        // Отключаем следящую камеру
        if (victoryCamera.TryGetComponent(out SmoothFollowCamera followCam))
            followCam.enabled = false;

        // Главная последовательность
        Sequence mainSequence = DOTween.Sequence();

        // 1. Анимация камеры
        mainSequence.Append(victoryCamera.transform.DOMove(cameraWinTarget.position, cameraMoveTime));
        mainSequence.Join(victoryCamera.transform.DORotate(cameraWinTarget.eulerAngles, cameraMoveTime));

        // 2. Задержка перед показом объектов
        mainSequence.AppendInterval(objectsAppearDelay);

        // 3. Включаем объекты победы
        mainSequence.AppendCallback(() => {
            foreach (var obj in winObjects)
            {
                if (obj != null)
                {
                    obj.gameObject.SetActive(true);
                    obj.localScale = Vector3.zero;
                }
            }
        });

        // 4. Анимация появления
        Sequence appearSequence = DOTween.Sequence();
        foreach (var obj in winObjects)
        {
            if (obj != null)
            {
                appearSequence.Join(obj.DOScale(obj.localScale, objectsAnimationTime).SetEase(scaleEase));
            }
        }
        mainSequence.Append(appearSequence);

        // 5. Показываем подсказку и ждём ввода
        mainSequence.AppendCallback(() => {
            if (pressAnyKeyText != null)
                pressAnyKeyText.SetActive(true);

            mainSequence.Pause(); // Останавливаем последовательность

            StartCoroutine(WaitForPlayerInput(mainSequence));
        });

        // 6. Анимация исчезновения
        Sequence disappearSequence = DOTween.Sequence();
        foreach (var obj in winObjects)
        {
            if (obj != null)
            {
                disappearSequence.Join(obj.DOScale(Vector3.zero, objectsAnimationTime).SetEase(scaleEase));
            }
        }
        mainSequence.Append(disappearSequence);

        // 7. Выключаем объекты
        mainSequence.AppendCallback(() => {
            foreach (var obj in winObjects)
            {
                if (obj != null)
                    obj.gameObject.SetActive(false);
            }

            if (pressAnyKeyText != null)
                pressAnyKeyText.SetActive(false);
        });

        // 8. Затемнение экрана
        if (fadeImage != null)
        {
            mainSequence.AppendCallback(() => fadeImage.gameObject.SetActive(true));
            mainSequence.Append(fadeImage.DOFade(1f, fadeTime));
        }

        // Запускаем последовательность
        mainSequence.Play();
    }

    private IEnumerator WaitForPlayerInput(Sequence sequence)
    {
        // Ждём нажатия клавиши, мыши или касания
        while (!Input.anyKeyDown && Input.touchCount == 0)
        {
            yield return null;
        }

        // На всякий случай ждём один кадр, чтобы избежать дублирования
        yield return null;

        // Продолжаем анимацию
        sequence.Play();
    }

    void ReleaseHormones(int index)
    {
        if (index < 0 || index >= hormonesContainers.Length) return;

        var container = hormonesContainers[index];
        container.gameObject.SetActive(true);

        foreach (Transform hormone in container)
        {
            if (hormone == null) continue;

            Vector3 targetScale = hormone.localScale;
            hormone.localScale = Vector3.zero;
            hormone.gameObject.SetActive(true);
            hormone.DOScale(targetScale, scaleDuration).SetEase(scaleEase);
        }
    }
}