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

    [Header("Тряска камеры")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private float _strength = 0.3f;
    [SerializeField] private int _vibrato = 10;
    [SerializeField] private float _randomness = 90f;
    [SerializeField] private bool _fadeOut = true;

    private Tweener positionTweener;
    private int countOfObj = 0;
    private string currentType;
    private Vector3 _originalPosition;

    private void Start()
    {
        _originalPosition = _cameraTransform.position;
    }

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
                //ShakeCamera();


                Destroy(collectableObj);
                countOfObj++;
                break;
            }
        }
        
    }

    //public void ShakeCamera()
    //{
    //    _cameraTransform.DOShakePosition(
    //        duration: _duration,
    //        strength: _strength,
    //        vibrato: _vibrato,
    //        randomness: _randomness,
    //        snapping: false,
    //        fadeOut: _fadeOut
    //    ).OnComplete(() => {
    //        _cameraTransform.localPosition = _originalPosition;
    //    });
    //}



}
