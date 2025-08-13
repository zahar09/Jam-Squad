using UnityEngine;
using DG.Tweening;

public class EButton : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private AnimationCurve easeIn = AnimationCurve.EaseInOut(0, 0, 1, 1); // При включении
    [SerializeField] private AnimationCurve easeOut = AnimationCurve.EaseInOut(0, 0, 1, 1); // При выключении

    private Vector3 originalScale;
    private bool isAnimating = false;

    private void Awake()
    {
        // Сохраняем исходный масштаб
        originalScale = transform.localScale;

        // Сразу устанавливаем в 0, если объект активен при старте
        if (gameObject.activeInHierarchy && !isAnimating)
        {
            transform.localScale = Vector3.zero;
        }
    }

    private void OnEnable()
    {
        // Останавливаем предыдущую анимацию
        transform.DOKill();

        // Устанавливаем scale в 0 на случай, если объект был отключён без анимации
        transform.localScale = Vector3.zero;

        // Анимируем к оригинальному масштабу
        transform.DOScale(originalScale, animationDuration)
            .SetEase(easeIn);
    }

    

    // Вызывай этот метод, когда хочешь корректно "выключить с анимацией"
    public void DisableWithAnimation()
    {
        if (isAnimating) return;

        isAnimating = true;
        transform.DOKill();

        transform.DOScale(Vector3.zero, animationDuration)
            .SetEase(easeOut)
            .OnComplete(() =>
            {
                // Только после анимации — отключаем
                isAnimating = false;
                // ВАЖНО: SetActive(false) вызовет OnDisable(), но объект уже будет выключен
                // Чтобы избежать рекурсии — делаем через MonoBehaviour
                gameObject.SetActive(false);
            });
    }

    // Опционально: если хочешь вызывать через Invoke или кнопку

    public void DisableObject()
    {
        DisableWithAnimation();
    }
}