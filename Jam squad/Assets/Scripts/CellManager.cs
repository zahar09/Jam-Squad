using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CellManager : MonoBehaviour
{
    [Header("Ячейки памяти")]
    [SerializeField] private GameObject messageObj;
    [SerializeField] private TextMeshProUGUI messageUI;
    [SerializeField] private string[] messageTextes;
    

    [Header("Меню победы")]
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject _endCamera;

    [Header("Анимация поражения")]
    [SerializeField] private SmoothFollowCamera _camera;
    [SerializeField] private Image fadeImage; // Чёрный Image на весь экран
    [SerializeField] private float fadeDuration = 1.5f;
    

    private List<string> _availableMessages = new List<string>();

    private void Start()
    {
        _availableMessages = new List<string>(messageTextes);

        // Начальная настройка затемнения
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(false);
        }
    }

    public void GetMemoryMessage()
    {
        

        int randomIndex = Random.Range(0, _availableMessages.Count);
        messageUI.text = _availableMessages[randomIndex];
        _availableMessages.RemoveAt(randomIndex);
        messageObj.gameObject.SetActive(true);

        if (_availableMessages.Count == 0)
        {
            WinGame();
            return;
        }
    }

    private void WinGame()
    {
        //_endCamera.Priority = 15;
        //_endCamera.gameObject.SetActive(true);
        
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

        // Здесь можно добавить другие действия после затемнения
        // Например, показать меню поражения
    }
}