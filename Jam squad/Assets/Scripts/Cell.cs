using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cell : MonoBehaviour 
{
    [SerializeField] CollectableHolder[] holders;

    [Header("Настройки анимации подбора")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutQuad;

    [Header("Настройка анимации обгрейда")]
    [SerializeField] private GameObject newObj;
    [SerializeField] private Vector3 finalScale = Vector3.one; 
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease easeType2 = Ease.OutBack;

    [Header("Настройки тряски")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeStrength = 0.2f;
    [SerializeField] private int vibrato = 10;
    [SerializeField] private float randomness = 90f;
    


    private Vector3 _originalScale;
    private Sequence _shakeSequence;

    private Tweener positionTweener;
    private int holderIndexToPut = 0;
    private int holderIndexToDestroy = 0;

    private bool removIsStart = false;

    private CellManager cellManager;

    private void Awake()
    {
        _originalScale = transform.localScale;

    }

    private void Start()
    {
        cellManager = FindAnyObjectByType<CellManager>();
    }

    private IEnumerator RemoveOneBall()
    {
        yield return new WaitForSeconds(5f); // Ждём 5 секунд перед началом

        while (holderIndexToDestroy >= 0)
        {
            // Проверяем индекс и существование слота
            if (holderIndexToDestroy >= holders.Length || holderIndexToDestroy < 0)
            {
                Debug.LogError("Invalid holderIndex: " + holderIndexToDestroy);
                break;
            }

            CollectableHolder currentHolder = holders[holderIndexToDestroy];

            // Проверяем, что collectable существует
            if (currentHolder.collectable != null && currentHolder.collectable.gameObject != null)
            {
                PlayShakeEffect();
                // Анимируем уменьшение
                currentHolder.collectable.gameObject.transform.DOScale(Vector3.zero, duration)
                    .SetEase(easeType2)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        // Уничтожаем объект после анимации
                        Destroy(currentHolder.collectable.gameObject);
                        currentHolder.collectable = null;
                        holderIndexToDestroy--;
                        holderIndexToPut--;
                        if (holderIndexToDestroy < 0)
                        {
                            holderIndexToDestroy = 0;
                            cellManager.LoosGame(gameObject);
                            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                        }
                    });
            }
            else
            {
                Debug.LogWarning("No collectable to remove at index " + holderIndexToDestroy);
            }

            // Уменьшаем индекс
            
            

            // Ждём перед следующим удалением
            yield return new WaitForSeconds(5f);
        }


        //Debug.Log("No more balls. Reloading scene...");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        cellManager.LoosGame(gameObject);

    }


    public void TryToUpgrade(GameObject[] collectableObjs)
    {
        
        if (holderIndexToPut > 2) return;
        for (int i = 0; i < 3; i++)
        {
            print("sdnajikbnkj");
            //holders[holderIndex].collectable = collectableObjs[i];
            collectableObjs[i].transform.SetParent(holders[holderIndexToPut].transform);
            positionTweener = collectableObjs[i].transform.DOLocalMove(Vector3.zero, moveDuration)
             .SetEase(easeType)
             .SetUpdate(true);
            
        }

        GameObject newObj_ = Instantiate(newObj);
        newObj_.transform.SetParent(holders[holderIndexToPut].transform);
        newObj_.transform.localPosition = Vector3.zero;
        newObj_.transform.localScale = Vector3.zero;
        holders[holderIndexToPut].collectable = newObj_;
        newObj_.transform.DOScale(finalScale, duration)
            .SetEase(easeType2)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                foreach (GameObject obj in collectableObjs)
                {
                    Destroy(obj);
                }
            });

        PlayShakeEffect();
        holderIndexToPut++;
        holderIndexToDestroy++;
        if(holderIndexToPut == 3 && !removIsStart)
        {
            //GetMemoryMessage();
            cellManager.GetMemoryMessage();
            removIsStart = true;
            holderIndexToDestroy = 2;
            StartCoroutine(RemoveOneBall());
        }
    }

    public void PlayShakeEffect()
    {
        _shakeSequence?.Kill();

        _shakeSequence = DOTween.Sequence();

        _shakeSequence.Append(transform.DOShakeScale(
                shakeDuration,
                strength: shakeStrength,
                vibrato: vibrato,
                randomness: randomness
            ).SetEase(Ease.OutQuad));

        _shakeSequence.Append(transform.DOScale(_originalScale, 0.1f));
    }

    private void OnDestroy()
    {
        _shakeSequence?.Kill();
    }


}
