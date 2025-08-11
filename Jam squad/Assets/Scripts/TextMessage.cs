using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMessage : MonoBehaviour
{
    [SerializeField] private float _scaleDuration = 0.5f;
    [SerializeField] private float _delayBeforeScaleOut = 2f;

    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        transform.DOScale(_originalScale, _scaleDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(_delayBeforeScaleOut, () =>
                {
                    transform.DOScale(Vector3.zero, _scaleDuration)
                        .SetEase(Ease.InBack)
                        .OnComplete(() => gameObject.SetActive(false));
                });
            });
    }

    private void OnDisable()
    {
        transform.localScale = Vector3.zero;
    }
}
