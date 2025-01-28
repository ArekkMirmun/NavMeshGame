using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public enum EnemyState {
    Patrolling,
    Alert,
    Chasing,
    Attacking,
    AlertPatrolling
}

public class Enemy : MonoBehaviour
{
    private static readonly int Velocity = Animator.StringToHash("velocity");
    [SerializeField] private Transform path;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float alertPatrolSpeed = 3f;
    [SerializeField] private float chaseSpeed = 4.5f;
    [SerializeField] private Transform[] waypoints;

    [SerializeField] private GameObject player;
    [SerializeField] private float playerDetectionRange = 10f;
    [SerializeField] private float playerDetectionAngle = 30f;
    [SerializeField] private float playerDetectionInterval = 0.5f;

    [SerializeField] private Animator animator; // Reference to the Animator
    private NavMeshAgent _navMeshAgent;
    [SerializeField]private EnemyState currentState;

    private int _currentWaypointIndex;
    private bool _playerDetected;
    private bool _isAlerted;
    private const float AlertTimer = 5f; // Time in alert state after losing sight of the player
    private float _alertTimeLeft;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        ChangeState(EnemyState.Patrolling);
        StartCoroutine(PlayerDetection());
    }

    void Update()
    {
        // Update the Animator's velocity parameter based on the NavMeshAgent's velocity
        var currentSpeed = _navMeshAgent.velocity.magnitude;
        animator.SetFloat(Velocity, currentSpeed);

        // State machine logic
        switch (currentState)
        {
            case EnemyState.Patrolling:
                HandlePatrolling();
                break;
            case EnemyState.Alert:
                HandleAlert();
                break;
            case EnemyState.Chasing:
                HandleChasing();
                break;
            case EnemyState.Attacking:
                HandleAttacking();
                break;
            case EnemyState.AlertPatrolling:
                HandleAlertPatrolling();
                break;
        }
    }

    #region State Handling

    private void HandlePatrolling()
    {
        _navMeshAgent.speed = patrolSpeed;

        if (!_navMeshAgent.hasPath || _navMeshAgent.remainingDistance < 0.1f)
        {
            // Go to the next waypoint
            _navMeshAgent.SetDestination(waypoints[_currentWaypointIndex].position);
            _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void HandleAlert()
    {
        // Stay alert for a certain time
        _alertTimeLeft -= Time.deltaTime;

        if (_alertTimeLeft <= 0f)
        {
            ChangeState(_isAlerted ? EnemyState.AlertPatrolling : EnemyState.Patrolling);
        }
    }

    private void HandleChasing()
    {
        _navMeshAgent.speed = chaseSpeed;

        // Chase the player
        _navMeshAgent.SetDestination(player.transform.position);

        // Check distance to attack
        if (Vector3.Distance(transform.position, player.transform.position) < 1f)
        {
            ChangeState(EnemyState.Attacking);
        }
    }

    private void HandleAttacking()
    {
        
        print("Attack");
        // Reset back to chasing after attacking (for now)
        ChangeState(EnemyState.Chasing);
    }

    private void HandleAlertPatrolling()
    {
        _navMeshAgent.speed = alertPatrolSpeed;

        if (!_navMeshAgent.hasPath || _navMeshAgent.remainingDistance < 0.1f)
        {
            // Go to the next waypoint in alert mode
            _navMeshAgent.SetDestination(waypoints[_currentWaypointIndex].position);
            _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;

        // Reset NavMeshAgent for specific states if needed
        if (newState != EnemyState.Attacking)
        {
            _navMeshAgent.isStopped = false;
        }
    }

    
    #endregion
    
    #region Player Detection
    
    private IEnumerator PlayerDetection()
    {
        while (true)
        {
            Vector3 direction = player.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            if (Vector3.Distance(transform.position, player.transform.position) < playerDetectionRange && angle < playerDetectionAngle)
            {
                if (Physics.Raycast(transform.position, direction, out var hit))
                {
                    if (hit.collider.gameObject == player)
                    {
                        _playerDetected = true;
                        _isAlerted = true;
                        _alertTimeLeft = AlertTimer;

                        ChangeState(EnemyState.Chasing);
                    }
                }
            }
            else
            {
                _playerDetected = false;

                if (currentState == EnemyState.Chasing)
                {
                    ChangeState(EnemyState.Alert);
                }
            }

            yield return new WaitForSeconds(playerDetectionInterval);
        }
    }
    #endregion
    
    #region Kill Player
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    #endregion
}
