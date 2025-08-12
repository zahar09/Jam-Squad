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

    [Header("��������� �������� �������")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutQuad;

    [Header("������ ������")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private float _strength = 0.3f;
    [SerializeField] private int _vibrato = 10;
    [SerializeField] private float _randomness = 90f;
    [SerializeField] private bool _fadeOut = true;

    [Header("���� �������")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _collectSounds;

    private Tweener positionTweener;
    private int countOfObj = 0;
    private string currentType;
    private Vector3 _originalPosition;

    private void Start()
    {
        _originalPosition = _cameraTransform.position;

        if (_audioSource != null)
            _audioSource.playOnAwake = false;
    }

    private void OnTriggerStay(Collider other)
    {
        CollectableObj collectableObj = other.GetComponent<CollectableObj>();
        if (collectableObj != null && Input.GetKey(KeyCode.E))
        {
            TryToCollect(collectableObj);
        }

        Cell cell = other.GetComponent<Cell>();
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

                PlayRandomCollectSound(); // ������������� ���� ��� �������
                Destroy(collectableObj);
                countOfObj++;
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