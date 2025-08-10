using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);
    [SerializeField] private float smoothSpeed = 0.15f; // ������ ��� ����� (� ��������), � �� ���������

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        // ���������� SmoothDamp � ���������� ���������� �������
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothSpeed // ��� �����, �� ������� ������ "�������" ���� (� ��������)
        );

        // ������� ������� (�����������, ������ ����������� LookAt)
        transform.rotation = Quaternion.LookRotation(
            target.position - transform.position,
            Vector3.up
        );
    }
}