using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

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
    [SerializeField] private GameObject pressAnyKeyText;

    [Header("Меню паузы")]
    [SerializeField] private GameObject settingsMenu; // Меню настроек (активируется на паузе)
    [SerializeField] private Image pauseOverlay;     // Затемнение фона (например, черный с alpha)
    [SerializeField] private float menuScaleDuration = 0.4f;
    [SerializeField] private Ease menuEase = Ease.OutBack;

    private bool objectsInitialized;
    private bool isPaused = false;
    private Vector3 _settingsInitialScale;

    private void Start()
    {
        InitializeObjects();

        // Настройка меню паузы
        if (settingsMenu != null)
        {
            _settingsInitialScale = settingsMenu.transform.localScale;
            settingsMenu.SetActive(false);
            settingsMenu.transform.localScale = Vector3.zero;
        }

        if (pauseOverlay != null)
        {
            pauseOverlay.color = new Color(pauseOverlay.color.r, pauseOverlay.color.g, pauseOverlay.color.b, 0);
            pauseOverlay.gameObject.SetActive(false);
        }
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

    private void Update()
    {
        // Проверяем нажатие Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused || settingsMenu == null) return;

        isPaused = true;

        // Включаем оверлей ДО анимации, но с нулевой альфой
        if (pauseOverlay != null)
        {
            pauseOverlay.gameObject.SetActive(true);
            pauseOverlay.color = new Color(pauseOverlay.color.r, pauseOverlay.color.g, pauseOverlay.color.b, 0); // сбрасываем альфу
            pauseOverlay.DOFade(0.7f, 0.3f)
                .SetUpdate(true); // 🔥 КЛЮЧЕВОЕ: работает на паузе
        }

        // Показываем меню
        settingsMenu.SetActive(true);
        settingsMenu.transform.localScale = Vector3.zero;
        settingsMenu.transform.DOScale(_settingsInitialScale, menuScaleDuration)
            .SetEase(menuEase)
            .SetUpdate(true); // уже было — хорошо

        // Ставим игру на паузу
        Time.timeScale = 0.1f;
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;

        // Анимация исчезновения меню
        settingsMenu.transform.DOScale(Vector3.zero, menuScaleDuration)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                settingsMenu.SetActive(false);

                // Анимация затухания оверлея
                if (pauseOverlay != null)
                {
                    pauseOverlay.DOFade(0f, 0.3f)
                        .SetUpdate(true) // 🔥 Работает на паузе
                        .OnComplete(() =>
                        {
                            pauseOverlay.gameObject.SetActive(false);
                        });
                }
            });

        // Снимаем паузу
        Time.timeScale = 1;
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
        foreach (var cell in cells)
        {
            if (cell == null) continue;
            Vector3 worldPosition = cell.position;
            Quaternion worldRotation = cell.rotation;
            cell.SetParent(null, false);
            cell.position = worldPosition;
            cell.rotation = worldRotation;
        }

        foreach (var container in hormonesContainers)
        {
            if (container == null) continue;
            Vector3 worldPosition = container.position;
            Quaternion worldRotation = container.rotation;
            container.SetParent(null, false);
            container.position = worldPosition;
            container.rotation = worldRotation;
        }

        foreach (var obj in additionalObjects)
        {
            if (obj == null) continue;
            Vector3 worldPosition = obj.position;
            Quaternion worldRotation = obj.rotation;
            obj.SetParent(null, false);
            obj.position = worldPosition;
            obj.rotation = worldRotation;
        }

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
        if (player != null)
            Destroy(player.gameObject);

        if (victoryCamera.TryGetComponent(out SmoothFollowCamera followCam))
            followCam.enabled = false;

        Sequence mainSequence = DOTween.Sequence();

        mainSequence.Append(victoryCamera.transform.DOMove(cameraWinTarget.position, cameraMoveTime));
        mainSequence.Join(victoryCamera.transform.DORotate(cameraWinTarget.eulerAngles, cameraMoveTime));

        mainSequence.AppendInterval(objectsAppearDelay);

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

        Sequence appearSequence = DOTween.Sequence();
        foreach (var obj in winObjects)
        {
            if (obj != null)
            {
                appearSequence.Join(obj.DOScale(obj.localScale, objectsAnimationTime).SetEase(scaleEase));
            }
        }
        mainSequence.Append(appearSequence);

        mainSequence.AppendCallback(() => {
            if (pressAnyKeyText != null)
                pressAnyKeyText.SetActive(true);

            mainSequence.Pause();
            StartCoroutine(WaitForPlayerInput(mainSequence));
        });

        Sequence disappearSequence = DOTween.Sequence();
        foreach (var obj in winObjects)
        {
            if (obj != null)
            {
                disappearSequence.Join(obj.DOScale(Vector3.zero, objectsAnimationTime).SetEase(scaleEase));
            }
        }
        mainSequence.Append(disappearSequence);

        mainSequence.AppendCallback(() => {
            foreach (var obj in winObjects)
            {
                if (obj != null)
                    obj.gameObject.SetActive(false);
            }

            if (pressAnyKeyText != null)
                pressAnyKeyText.SetActive(false);
        });

        if (fadeImage != null)
        {
            mainSequence.AppendCallback(() => fadeImage.gameObject.SetActive(true));
            mainSequence.Append(fadeImage.DOFade(1f, fadeTime));
        }

        mainSequence.AppendCallback(() => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });

        mainSequence.Play();
    }

    private IEnumerator WaitForPlayerInput(Sequence sequence)
    {
        while (!Input.anyKeyDown && Input.touchCount == 0)
        {
            yield return null;
        }

        yield return null;

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