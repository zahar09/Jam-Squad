using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBall : MonoBehaviour
{
    [SerializeField] private Transform player;        
    [SerializeField] private float moveSpeed = 3f;    
    [SerializeField] private float detectionRange = 10f; 
    [SerializeField] private float rotationSpeed = 10f;  

    private Rigidbody rb;
    private bool isPlayerInRange;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
                Debug.LogError("Player not found! Assign it in inspector or use 'Player' tag.");
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            isPlayerInRange = true;
            Vector3 direction = (player.position - transform.position).normalized;
            direction.z = 0f; 

            
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

            
            Vector3 targetPosition = transform.position + direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);
        }
        else
        {
            isPlayerInRange = false;
        }
    }

    // Опционально: отрисовка зоны обнаружения
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isPlayerInRange ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
