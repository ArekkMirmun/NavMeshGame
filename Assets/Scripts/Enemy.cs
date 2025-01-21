using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform path;
    [SerializeField] private int childrenIndex;
    [SerializeField] private Vector3 min;
    [SerializeField] private Vector3 max;

    [SerializeField] private GameObject player;
    [SerializeField] private float playerDetectionRange = 10f;
    [SerializeField] private float playerDetectionAngle = 30f;
    [SerializeField] private float playerDetectionInterval = 1f;

    [SerializeField] private Animator animator; // Reference to the Animator
    private NavMeshAgent navMeshAgent;         // Reference to the NavMeshAgent
    private bool playerDetected = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        StartCoroutine(PlayerDetection());
    }

    private void Update()
    {
        // Update the Animator's velocity parameter based on the NavMeshAgent's velocity
        float currentSpeed = navMeshAgent.velocity.magnitude;
        animator.SetFloat("velocity", currentSpeed);
    }

    private IEnumerator PlayerDetection()
    {
        while (true)
        {
            // Get direction and angle between player and enemy
            Vector3 direction = player.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);
            RaycastHit hit;

            // If player is within range and angle
            if (Vector3.Distance(transform.position, player.transform.position) < playerDetectionRange && angle < playerDetectionAngle)
            {
                // If raycast hits player
                if (Physics.Raycast(transform.position, direction, out hit))
                {
                    if (hit.collider.gameObject == player)
                    {
                        navMeshAgent.SetDestination(player.transform.position);
                        playerDetected = true;
                        //look at player
                        transform.LookAt(player.transform);
                    }
                    else
                    {
                        playerDetected = false;
                    }
                }
            }
            else
            {
                playerDetected = false;
            }

            // Wait for the next detection cycle
            yield return new WaitForSeconds(playerDetectionInterval);
        }
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
    private void OnDrawGizmos()
    {
        //Show in game
        Debug.DrawRay(transform.position, transform.forward * playerDetectionRange, playerDetected ? Color.red : Color.green);
        Debug.DrawRay(transform.position, Quaternion.Euler(0, playerDetectionAngle, 0) * transform.forward * playerDetectionRange, playerDetected ? Color.red : Color.green);
        Debug.DrawRay(transform.position, Quaternion.Euler(0, -playerDetectionAngle, 0) * transform.forward * playerDetectionRange, playerDetected ? Color.red : Color.green);
        
    }
}
