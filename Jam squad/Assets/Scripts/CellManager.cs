using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class CellManager : MonoBehaviour
{
    [Header("������ ������")]
    [SerializeField] private GameObject messageObj;        // ������ � UI (������������/����������)
    [SerializeField] private Image messageImage;          // �����: Image ������ TextMeshPro
    [SerializeField] private Sprite[] messageSprites;     // ������ �������� ������ �����

    [Header("�������� ���������")]
    [SerializeField] private SmoothFollowCamera _camera;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("���� ������ ������")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _soundClips;

    [Header("��������� ��������")]
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

        // ��������, ��� messageObj ���������� ��������
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

            // �������� ��������� ������
            int randomIndex = Random.Range(0, _availableSprites.Count);
            messageImage.sprite = _availableSprites[randomIndex];
            _availableSprites.RemoveAt(randomIndex);

            // ���������� ������ � ������������
            messageObj.SetActive(true);
            PlayRandomSound();

            // ��������� �������� � GameManager
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            gameManager.ActivateCell(3 - _availableSprites.Count);

            // ���� ��� ������� ������� � ������
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