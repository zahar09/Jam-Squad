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

    [Header("Звук подбора")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _collectSounds;

    [Header("Обучение")]
    [SerializeField] private GameObject eButton;

    private Tweener positionTweener;
    private int countOfObj = 0;
    private string currentType;
    private Vector3 _originalPosition;

    private bool isItEducation = true;

    private void Start()
    {
        _originalPosition = _cameraTransform.position;

        if (_audioSource != null)
            _audioSource.playOnAwake = false;
    }

    private void OnTriggerStay(Collider other)
    {
        CollectableObj collectableObj = other.GetComponent<CollectableObj>();
        if (collectableObj != null && isItEducation) 
        {
            eButton.gameObject.SetActive(true);
        }

        if (collectableObj != null && Input.GetKey(KeyCode.E))
        {
            TryToCollect(collectableObj);
        }

        Cell cell = other.GetComponent<Cell>();
        if (cell != null && isItEducation)
        {
            eButton.gameObject.SetActive(true);
        }

        if (cell != null && countOfObj == 3 && Input.GetKey(KeyCode.E))
        {
            GameObject[] objs = new GameObject[countOfObj];
            for (int i = 0; i < 3; i++)
            {
                objs[i] = holders[i].collectable;
                holders[i].collectable = null;
            }
            countOfObj = 0;
            currentType = null;
            cell.TryToUpgrade(objs);
            isItEducation = false;
            eButton.GetComponent<EButton>().DisableObject();

        }
    }


    private void OnTriggerExit(Collider other)
    {
        CollectableObj collectableObj = other.GetComponent<CollectableObj>();
        if (collectableObj != null && isItEducation) 
        {
            eButton.GetComponent<EButton>().DisableObject();
        }

        Cell cell = other.GetComponent<Cell>();
        if (cell != null && isItEducation)
        {
            eButton.GetComponent<EButton>().DisableObject();
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
            else if (collectableObj.type != currentType)
            {
                break;
            }
            if (holder.collectable == null)
            {
                holder.collectable = collectableObj.gameObject;
                collectableObj.transform.SetParent(holder.transform);

                positionTweener = collectableObj.transform.DOLocalMove(Vector3.zero, moveDuration)
                    .SetEase(easeType)
                    .SetUpdate(true);

                PlayRandomCollectSound(); // Воспроизводим звук при подборе
                Destroy(collectableObj);
                eButton.GetComponent<EButton>().DisableObject();
                countOfObj++;
                if (countOfObj == 3)
                {
                    OnCollectThreeHormones();
                }

                break;
            }
        }
    }

    public void PlayRandomCollectSound()
    {
        if (_audioSource == null || _collectSounds == null || _collectSounds.Length == 0)
            return;

        int randomIndex = Random.Range(0, _collectSounds.Length);
        _audioSource.PlayOneShot(_collectSounds[randomIndex]);
    }

    private void OnCollectThreeHormones()
    {
        if (!isItEducation)
        {
            return;
        }
        Education education = FindAnyObjectByType<Education>();
        if (education != null) 
        { 
            education.ActivateCell();
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