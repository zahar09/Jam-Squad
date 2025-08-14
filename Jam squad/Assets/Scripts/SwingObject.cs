using UnityEngine;
using DG.Tweening;
using System.Collections;

public class SwingObject : MonoBehaviour
{
    [Header("Wobble Settings")]
    [SerializeField] private float _wobbleDuration = 0.8f;
    [SerializeField] private float _wobbleAmount = 5f;

    private bool startAnim = false;
    private void Start()
    {
        startAnim = true;
        transform.DORotate(new Vector3(0, 0, _wobbleAmount), _wobbleDuration / 2f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetLink(gameObject);
    }

    private void OnEnable()
    {
        if(startAnim) return;

        startAnim = true;
        transform.DORotate(new Vector3(0, 0, _wobbleAmount), _wobbleDuration / 2f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetLink(gameObject);
    }
}