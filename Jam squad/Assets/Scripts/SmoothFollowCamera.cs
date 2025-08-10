using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    [Header("�������� ���������")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -5f);

    [Header("�������������� ���������")]
    [SerializeField] private float maxSpeed = Mathf.Infinity; // ������ ����������� ��������
    [SerializeField] private bool useFixedUpdate = true; // ������������ FixedUpdate ��� ���������� ��������

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

        // ���������� ������ ��������� �������
        Vector3 targetMovement = target.position - lastTargetPosition;
        lastTargetPosition = target.position;

        // ������� ����������� � ������ ������� �������� ����
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