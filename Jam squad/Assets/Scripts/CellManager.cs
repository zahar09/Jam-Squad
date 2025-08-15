using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class CellManager : MonoBehaviour
{
    [Header("Ячейки памяти")]
    [SerializeField] private GameObject messageObj;        // Объект с UI (показывается/скрывается)
    [SerializeField] private Image messageImage;          // Новое: Image вместо TextMeshPro
    [SerializeField] private Sprite[] messageSprites;     // Массив спрайтов вместо строк

    [Header("Анимация поражения")]
    [SerializeField] private SmoothFollowCamera _camera;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Звук ячейки памяти")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _soundClips;

    [Header("Настройки обучения")]
    [SerializeField] private float educationDelayBeforeShrink;

    private bool isEducation = true;
    private List<Sprite> _availableSprites = new List<Sprite>();

    private void Start()
    {
        _availableSprites = new List<Sprite>(messageSprites);

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(false);
        }

        if (_audioSource != null)
            _audioSource.playOnAwake = false;

        // Убедимся, что messageObj изначально выключен
        if (messageObj != null)
            messageObj.SetActive(false);
    }

    public void PlayRandomSound()
    {
        if (_audioSource == null || _soundClips == null || _soundClips.Length == 0)
            return;

        int randomIndex = Random.Range(0, _soundClips.Length);
        _audioSource.PlayOneShot(_soundClips[randomIndex]);
    }

    public void GetMemoryMessage(GameObject cell)
    {
        if (isEducation)
        {
            isEducation = false;
            cell.GetComponent<Cell>().DestroyCell(educationDelayBeforeShrink);
        }
        else
        {
            if (_availableSprites.Count == 0) return;

            // Выбираем случайный спрайт
            int randomIndex = Random.Range(0, _availableSprites.Count);
            messageImage.sprite = _availableSprites[randomIndex];
            _availableSprites.RemoveAt(randomIndex);

            // Показываем объект с изображением
            messageObj.SetActive(true);
            PlayRandomSound();

            // Обновляем прогресс в GameManager
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            gameManager.ActivateCell(3 - _availableSprites.Count);

            // Если все спрайты собраны — победа
            if (_availableSprites.Count == 0)
            {
                WinGame();
            }
        }
    }

    private void WinGame()
    {
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        gameManager.WinGame();
    }

    public void LoosGame(GameObject cell)
    {
        _camera.target = cell.transform;
        PlayerHolder player = FindAnyObjectByType<PlayerHolder>();
        if (player != null)
            Destroy(player.gameObject);

        cell.GetComponent<Cell>().DestroyCell(0f);
        StartCoroutine(FadeAndLose());
    }

    private IEnumerator FadeAndLose()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                });
        }

        yield return new WaitForSeconds(fadeDuration);
    }
}