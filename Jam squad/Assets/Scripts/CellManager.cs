using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class CellManager : MonoBehaviour
{
    [Header("������ ������")]
    [SerializeField] private GameObject messageObj;
    [SerializeField] private TextMeshProUGUI messageUI;
    [SerializeField] private string[] messageTextes;

    [Header("���� ������")]
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject _endCamera;

    [Header("�������� ���������")]
    [SerializeField] private SmoothFollowCamera _camera;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("���� ������ ������")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _soundClips; // ������ ������

    

    private List<string> _availableMessages = new List<string>();

    private void Start()
    {
        _availableMessages = new List<string>(messageTextes);

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(false);
        }

        if (_audioSource != null)
            _audioSource.playOnAwake = false;
    }

    // ��������� ����� ��� ��������������� ���������� �����
    public void PlayRandomSound()
    {
        if (_audioSource == null || _soundClips == null || _soundClips.Length == 0)
            return;

        int randomIndex = Random.Range(0, _soundClips.Length);
        _audioSource.PlayOneShot(_soundClips[randomIndex]);
    }

    public void GetMemoryMessage()
    {
        int randomIndex = Random.Range(0, _availableMessages.Count);
        messageUI.text = _availableMessages[randomIndex];
        _availableMessages.RemoveAt(randomIndex);
        messageObj.gameObject.SetActive(true);
        PlayRandomSound();

        if (_availableMessages.Count == 0)
        {
            WinGame();
            return;
        }
    }

    private void WinGame()
    {
        DOVirtual.DelayedCall(5f, () =>
        {
            winMenu.SetActive(true);
        });
    }

    public void LoosGame(GameObject cell)
    {
        _camera.target = cell.transform;
        PlayerHolder player = FindAnyObjectByType<PlayerHolder>();
        Destroy(player.gameObject);

        cell.transform.DOScale(0.5f, fadeDuration).SetEase(Ease.InOutQuad);

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