using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum EnemyState
{
    Patrolling,
    Detecting,
    Attack
}

public class EnemyBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask obstacleMask; // Add layer mask to ignore non-blocking colliders

    [Header("Enemy Settings")]
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float minPatrolWaitTime = 0.2f;
    [SerializeField] private float stopAtDistance = 0.5f;
    [SerializeField] private float maxLostPlayerTime = 15.0f;
    [SerializeField] private float losePlayerTime = 3f;
    [SerializeField] private float maxDetectionRange = 30.0f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float viewAngle = 90f;

    // I need to multiply this float but I forgot which part is responsible for it's movement.
    [SerializeField] private float enemyMoveSpeed = 5f;


    [Header("Events")]
    public UnityEvent onPlayerSight;
    public UnityEvent onPlayerLoss;


    private NavMeshAgent _agent;
    private EnemyState _state = EnemyState.Patrolling;
    private int _currentPatrolIndex;
    private bool _isWaiting;
    private float _timeSinceLostPlayer;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = enemyMoveSpeed;
    }

    private void Start()
    {
        GoToNextPatrolPoint();
    }

    private void Update()
    {
        // Skip all AI logic during pause
        if (GameManager.Instance.IsPaused)
            return;

        if (player == null) return;

        switch (_state)
        {
            case EnemyState.Patrolling:
                Patrol();
                if (CanSeePlayer())
                {
                    LogHandler.Log("Player spotted!");
                    ChangeState(EnemyState.Detecting);
                }
                break;

            case EnemyState.Detecting:
                Detecting();
                break;
        }
    }

    private void ChangeState(EnemyState newState)
    {
        _state = newState;

        if (_state == EnemyState.Patrolling)
        {
            _agent.isStopped = false;
            GoToClosestPatrolPoint();
        }
    }

    private void Patrol()
    {
        if (_isWaiting) return;

        // Check if destination is reached
        if (!_agent.pathPending && _agent.remainingDistance <= stopAtDistance)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        _isWaiting = true;
        _agent.isStopped = true;

        yield return new WaitForSeconds(patrolWaitTime);

        _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPoints.Length;
        GoToNextPatrolPoint();

        _agent.isStopped = false;
        _isWaiting = false;
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        _agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
    }

    private void GoToClosestPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        _currentPatrolIndex = closestIndex;
        GoToNextPatrolPoint();
    }

    private void Detecting()
    {
        // Don't detect while paused
        if (GameManager.Instance.IsPaused)
            return;

        // When the player is detected, head to towards thier position
        _agent.SetDestination(player.position);

        //Invoke the "player sighted event"
        onPlayerSight.Invoke();

        LogHandler.Log("PLAYER SPOTTED! I SEE YOU");

        if (CanSeePlayer())
        {
            _timeSinceLostPlayer = 0f; // Reset search timer while player is in sight
        }
        else
        {
            // when we've lost sight, invoke the 'player lost' event

            _timeSinceLostPlayer += Time.deltaTime;
            onPlayerLoss.Invoke();
            LogHandler.Log("I LOST THE PLAYER!");

            if (_timeSinceLostPlayer >= losePlayerTime)
            {
                ChangeState(EnemyState.Patrolling);
                
            }
        }
    }

    private bool CanSeePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange) return false;

        return IsFacingPlayer() && HasClearPathToPlayer(distanceToPlayer);
    }

    private bool IsFacingPlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        return angle <= viewAngle / 2f;
    }

    private bool HasClearPathToPlayer(float distanceToPlayer)
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, distanceToPlayer, ~obstacleMask))
        {
            return hit.transform == player;
        }

        return false;
    }

    // Visual Gizmos for easy Scene Debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 fovLine1 = Quaternion.AngleAxis(viewAngle / 2, Vector3.up) * transform.forward * detectionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-viewAngle / 2, Vector3.up) * transform.forward * detectionRange;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);
    }

    // Listens to OnItemCollect event
    public void MultiplyEnemyAggression(float agroMultiplier)
    {
        // Decreases wait time (enemy spends less time idling)
        patrolWaitTime = Mathf.Max(minPatrolWaitTime, patrolWaitTime * agroMultiplier);
        LogHandler.Log($"Enemy detection decreased to {patrolWaitTime}");

        // Increases pursuit persistence (enemy hunts longer before giving up)
        losePlayerTime = Mathf.Min(maxLostPlayerTime, losePlayerTime / agroMultiplier);
        LogHandler.Log($"Enemy pursuit persistence increase to {losePlayerTime}");

        // Expands sight radius (enemy spots player from further away)
        detectionRange = Mathf.Min(maxDetectionRange, detectionRange / agroMultiplier);
        LogHandler.Log($"Enemy detection range increased to {detectionRange}");

        enemyMoveSpeed = enemyMoveSpeed / agroMultiplier;
    }

}
