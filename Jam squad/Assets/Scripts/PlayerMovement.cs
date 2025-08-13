using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 10f;

    [Header("Visuals")]
    [SerializeField] private Transform playerModel; // ������ ������ ��������� (��������, ������ ��� 3D-���)
    [SerializeField] private Animator animator;

    private Rigidbody rb;
    private Vector3 movementInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (playerModel == null)
        {
            Debug.LogError("PlayerModel �� ��������! ������� � ����������.", this);
        }

        if (animator == null && playerModel != null)
        {
            animator = playerModel.GetComponent<Animator>();
        }
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movementInput = new Vector3(moveX, moveY, 0f).normalized;

        float movementMagnitude = movementInput.magnitude;

        // ������� ������ ������, ���� ���� ��������
        if (movementMagnitude > 0.1f && playerModel != null)
        {
            // ���� � ����������� �� ����������� ��������
            float targetAngle = Mathf.Atan2(movementInput.x, movementInput.y) * Mathf.Rad2Deg; // ����� = 0�
            playerModel.rotation = Quaternion.Lerp(
                playerModel.rotation,
                Quaternion.Euler(0f, 0f, -targetAngle), // �����, ����� ��������� ��������������
                turnSpeed * Time.deltaTime
            );
        }

        // �������� � ������
        if (animator != null)
        {
            animator.SetFloat("Speed", movementMagnitude);
        }
    }

    void FixedUpdate()
    {
        // ������� ���� ������ ������ (Rigidbody)
        rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
    }
}