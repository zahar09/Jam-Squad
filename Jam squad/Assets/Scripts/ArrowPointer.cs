using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] private float rotationSpeed = 5f;

    private void Update()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        direction.z = 0;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}