using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private Transform[] _objects;
    [SerializeField] private float _slideDuration = 0.5f;
    [SerializeField] private float _delayBetweenObjects = 0.3f;
    [SerializeField] private float _slideDistance = 500f;

    private Vector3[] _targetLocalPositions;
    private bool _isAnimating;

    private void Awake()
    {
        CacheTargetLocalPositions();
        PrepareForAnimation();
    }

    private void OnEnable()
    {
        if (!_isAnimating)
        {
            PrepareForAnimation();
            AnimateEnter();
        }
    }

    private void CacheTargetLocalPositions()
    {
        _targetLocalPositions = new Vector3[_objects.Length];
        for (int i = 0; i < _objects.Length; i++)
        {
            _targetLocalPositions[i] = _objects[i].localPosition;
        }
    }

    private void PrepareForAnimation()
    {
        for (int i = 0; i < _objects.Length; i++)
        {
            Vector3 startPos = _targetLocalPositions[i];
            startPos.x -= _slideDistance;
            _objects[i].localPosition = startPos;
            _objects[i].gameObject.SetActive(false);
        }
    }

    public void AnimateEnter()
    {
        _isAnimating = true;

        for (int i = 0; i < _objects.Length; i++)
        {
            int index = i;
            DOVirtual.DelayedCall(i * _delayBetweenObjects, () =>
            {
                _objects[index].gameObject.SetActive(true);
                _objects[index].DOLocalMoveX(_targetLocalPositions[index].x, _slideDuration)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() => {
                        if (index == _objects.Length - 1)
                            _isAnimating = false;
                    });
            });
        }
    }

    public void AnimateExit()
    {
        _isAnimating = true;

        for (int i = 0; i < _objects.Length; i++)
        {
            int index = i;
            DOVirtual.DelayedCall(i * _delayBetweenObjects, () =>
            {
                Vector3 endPos = _targetLocalPositions[index];
                endPos.x -= _slideDistance;

                _objects[index].DOLocalMoveX(endPos.x, _slideDuration)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => {
                        _objects[index].gameObject.SetActive(false);

                        if (index == _objects.Length - 1)
                            _isAnimating = false;
                    });
            });
        }
    }
}
