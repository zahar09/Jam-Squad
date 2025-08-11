using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyBall : MonoBehaviour
{
    [SerializeField] private Transform player;        
    [SerializeField] private float moveSpeed = 3f;    
    [SerializeField] private float rotationSpeed = 10f;  

    private Rigidbody rb;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player.gameObject) 
        { 
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

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
        Vector3 direction = (player.position - transform.position).normalized;
        direction.z = 0f; 
        //Quaternion targetRotation = Quaternion.LookRotation(direction);
        //transform.rotation = Quaternion.Lerp(
        //    transform.rotation,
        //    targetRotation,
        //    rotationSpeed * Time.fixedDeltaTime
        //);
        //
        transform.LookAt(direction);
        Vector3 targetPosition = transform.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);
    }
}
