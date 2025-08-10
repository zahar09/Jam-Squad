using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -5f);

    [Header("Дополнительные настройки")]
    [SerializeField] private float maxSpeed = Mathf.Infinity; // Убрали ограничение скорости
    [SerializeField] private bool useFixedUpdate = true; // Использовать FixedUpdate для физических объектов

    private Vector3 velocity = Vector3.zero;
    private Vector3 lastTargetPosition;

    private void Start()
    {
        if (target != null)
        {
            lastTargetPosition = target.position;
            transform.position = target.position + offset;
        }
    }

    private void Update()
    {
        if (!useFixedUpdate)
        {
            FollowTarget();
        }
    }

    private void FixedUpdate()
    {
        if (useFixedUpdate)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        if (target == null) return;

        // Фильтрация резких изменений позиции
        Vector3 targetMovement = target.position - lastTargetPosition;
        lastTargetPosition = target.position;

        // Плавное перемещение с учетом текущей скорости цели
        Vector3 desiredPosition = target.position + offset + targetMovement * 0.1f;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothTime,
            maxSpeed,
            useFixedUpdate ? Time.fixedDeltaTime : Time.deltaTime
        );
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        lastTargetPosition = newTarget.position;
        velocity = Vector3.zero;
    }
}