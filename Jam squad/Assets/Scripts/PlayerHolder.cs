using DG.Tweening;
using UnityEngine;


[System.Serializable]
public class CollectableHolder
{
    public Transform transform;
    public GameObject collectable;
}



public class PlayerHolder : MonoBehaviour
{
    [SerializeField] CollectableHolder[] holders;

    [Header("Настройки анимации подбора")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutQuad;

    private Tweener positionTweener;
    private int countOfObj = 0;
    private string currentType;


    private void OnTriggerStay(Collider other)
    {
        CollectableObj collectableObj = other.GetComponent<CollectableObj>();
        if (collectableObj != null && Input.GetKey(KeyCode.E))
        {
            print("asdmolkasm");
            TryToCollect(collectableObj);
        }

        Cell cell = other.GetComponent<Cell>();
        if (cell != null && countOfObj == 3 && Input.GetKey(KeyCode.E)) 
        {
            GameObject[] objs = new GameObject[countOfObj];
            for(int i = 0; i < 3; i++) 
            { 
                objs[i] = holders[i].collectable;
                holders[i].collectable = null;
            }
            countOfObj = 0;
            currentType = null;
            cell.TryToUpgrade(objs);
            
        }
    }

    private void TryToCollect(CollectableObj collectableObj)
    {
        
        foreach (CollectableHolder holder in holders)
        {
            if (currentType == null)
            {
                currentType = collectableObj.type;
            }
            else if(collectableObj.type != currentType) 
            {
                break;
            }
            if (holder.collectable == null)
            {
                holder.collectable = collectableObj.gameObject;
                collectableObj.transform.SetParent(holder.transform);
                //collectableObj.transform.localPosition = Vector3.zero;

                positionTweener = collectableObj.transform.DOLocalMove(Vector3.zero, moveDuration)
           .SetEase(easeType)
           .SetUpdate(true);

                
                Destroy(collectableObj);
                countOfObj++;
                break;
            }
        }
        
    }

    

}
