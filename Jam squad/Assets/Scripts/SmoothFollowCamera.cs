using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);
    [SerializeField] private float smoothSpeed = 0.15f; // Теперь это время (в секундах), а не множитель

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        // Используем SmoothDamp с правильным параметром времени
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothSpeed // Это время, за которое камера "догонит" цель (в секундах)
        );

        // Плавный поворот (опционально, вместо мгновенного LookAt)
        transform.rotation = Quaternion.LookRotation(
            target.position - transform.position,
            Vector3.up
        );
    }
}