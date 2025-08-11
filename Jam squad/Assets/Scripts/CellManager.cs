using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CellManager : MonoBehaviour
{
    [Header("Ячейки памяти")]
    [SerializeField] private GameObject messageObj;
    [SerializeField] private TextMeshProUGUI messageUI;
    [SerializeField] private string[] messageTextes;

    [Header("Меню победы")]
    [SerializeField] private GameObject winMenu;

    [Header("Анимация поражения")]
    [SerializeField] private SmoothFollowCamera _camera;



    private List<string> _availableMessages = new List<string>();
    // Start is called before the first frame update
    private void Start()
    {
        _availableMessages = new List<string>(messageTextes);
    }

    public void GetMemoryMessage()
    {
        if (_availableMessages.Count == 0)
        {
            WinGame();
        }

        int randomIndex = Random.Range(0, _availableMessages.Count);
        messageUI.text = _availableMessages[randomIndex];
        _availableMessages.RemoveAt(randomIndex);
        messageObj.gameObject.SetActive(true);

    }

    private void WinGame()
    {
        winMenu.SetActive(true);

    }

    public void LoosGame(GameObject cell)
    {
        _camera.target = cell.transform;
        PlayerHolder player = FindAnyObjectByType<PlayerHolder>();
        Destroy(player.gameObject);
    }
}
