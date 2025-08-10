using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cell : MonoBehaviour 
{
    [SerializeField] CollectableHolder[] holders;

    [Header("��������� �������� �������")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutQuad;

    [Header("��������� �������� ��������")]
    [SerializeField] private GameObject newObj;
    [SerializeField] private Vector3 finalScale = Vector3.one; 
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease easeType2 = Ease.OutBack;

    private Tweener positionTweener;
    private int holderIndex = 0;
    private bool removIsStart = false;


    private IEnumerator RemoveOneBall()
    {
        yield return new WaitForSeconds(5f); // ��� 5 ������ ����� �������

        while (holderIndex >= 0)
        {
            // ��������� ������ � ������������� �����
            if (holderIndex >= holders.Length || holderIndex < 0)
            {
                Debug.LogError("Invalid holderIndex: " + holderIndex);
                break;
            }

            CollectableHolder currentHolder = holders[holderIndex];

            // ���������, ��� collectable ����������
            if (currentHolder.collectable != null && currentHolder.collectable.gameObject != null)
            {
                // ��������� ����������
                currentHolder.collectable.gameObject.transform.DOScale(Vector3.zero, duration)
                    .SetEase(easeType2)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        // ���������� ������ ����� ��������
                        Destroy(currentHolder.collectable.gameObject);
                        currentHolder.collectable = null;
                        holderIndex--;
                        if (holderIndex < 0)
                        {
                            holderIndex = 0;
                            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                        }
                    });
            }
            else
            {
                Debug.LogWarning("No collectable to remove at index " + holderIndex);
            }

            // ��������� ������
            
            

            // ��� ����� ��������� ���������
            yield return new WaitForSeconds(5f);
        }

        // ����� ������� ��������� (��� 0), ������������� �����
        Debug.Log("No more balls. Reloading scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void TryToUpgrade(GameObject[] collectableObjs)
    {
        
        if (holderIndex > 2) return;
        for (int i = 0; i < 3; i++)
        {
            print("sdnajikbnkj");
            //holders[holderIndex].collectable = collectableObjs[i];
            collectableObjs[i].transform.SetParent(holders[holderIndex].transform);
            positionTweener = collectableObjs[i].transform.DOLocalMove(Vector3.zero, moveDuration)
             .SetEase(easeType)
             .SetUpdate(true);
            
        }

        GameObject newObj_ = Instantiate(newObj);
        newObj_.transform.SetParent(holders[holderIndex].transform);
        newObj_.transform.localPosition = Vector3.zero;
        newObj_.transform.localScale = Vector3.zero;
        holders[holderIndex].collectable = newObj_;
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


        holderIndex++;
        if(holderIndex == 3 && !removIsStart)
        {
            removIsStart = true;
            holderIndex--;
            StartCoroutine(RemoveOneBall());
        }
    }


}
