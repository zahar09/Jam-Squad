using UnityEngine;
using DG.Tweening;

public class EButton : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private AnimationCurve easeIn = AnimationCurve.EaseInOut(0, 0, 1, 1); // ��� ���������
    [SerializeField] private AnimationCurve easeOut = AnimationCurve.EaseInOut(0, 0, 1, 1); // ��� ����������

    private Vector3 originalScale;
    private bool isAnimating = false;

    private void Awake()
    {
        // ��������� �������� �������
        originalScale = transform.localScale;

        // ����� ������������� � 0, ���� ������ ������� ��� ������
        if (gameObject.activeInHierarchy && !isAnimating)
        {
            transform.localScale = Vector3.zero;
        }
    }

    private void OnEnable()
    {
        // ������������� ���������� ��������
        transform.DOKill();

        // ������������� scale � 0 �� ������, ���� ������ ��� �������� ��� ��������
        transform.localScale = Vector3.zero;

        // ��������� � ������������� ��������
        transform.DOScale(originalScale, animationDuration)
            .SetEase(easeIn);
    }

    

    // ������� ���� �����, ����� ������ ��������� "��������� � ���������"
    public void DisableWithAnimation()
    {
        if (isAnimating) return;

        isAnimating = true;
        transform.DOKill();

        transform.DOScale(Vector3.zero, animationDuration)
            .SetEase(easeOut)
            .OnComplete(() =>
            {
                // ������ ����� �������� � ���������
                isAnimating = false;
                // �����: SetActive(false) ������� OnDisable(), �� ������ ��� ����� ��������
                // ����� �������� �������� � ������ ����� MonoBehaviour
                gameObject.SetActive(false);
            });
    }

    // �����������: ���� ������ �������� ����� Invoke ��� ������

    public void DisableObject()
    {
        DisableWithAnimation();
    }
}