using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum EnemyState
{
    Patrolling,
    Detecting,
}

public class EnemyBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask obstacleMask; // Add layer mask to ignore non-blocking colliders
    [SerializeField] private PlayerHUD playerHUD;

    [Header("Enemy Settings")]
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float minPatrolWaitTime = 0.2f;
    [SerializeField] private float stopAtDistance = 0.5f;
    [SerializeField] private float maxLostPlayerTime = 15.0f;
    [SerializeField] private float losePlayerTime = 3f;
    [SerializeField] private float maxDetectionRange = 30.0f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float enemyMoveSpeed = 5f;

    [Header("Player Detection Settings")]
    [SerializeField] private float playerDetectionRate = 0.01f;
    [SerializeField] private float playerDetectionDischargeRate = 0.01f; // new: tune separately from fill rate
    [SerializeField] private float circleRateXY = 0.0140f;
    [SerializeField] private float currentCircleXY = 16f;
    [SerializeField] private float minCircleXY = 2f;
    [SerializeField] private float maxCircleXY = 16f;


    private float maxPlayerDetection = 1f;
    private float currentPlayerDetection;
    private bool isDetectionDischarging;

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

    //TODO: This method looks messy and poorly documented, ORGANISE IT!
    private Vector3 _lastKnownPlayerPosition;

    /// <summary>
    /// Runs every frame while the enemy is in the Detecting state.
    /// Fills or drains the detection meter, chases the player, and
    /// decides whether to trigger game over or give up and patrol.
    /// </summary>
    private void Detecting()
    {
        if (GameManager.Instance.IsPaused)
            return;

        bool canSeePlayer = CanSeePlayer();

        UpdateDetectionMeter(canSeePlayer);

        if (HasDetectedPlayer())
        {
            LogHandler.Log($"Detection reached {maxPlayerDetection}, Game Over!");
            GameManager.Instance.TriggerGameOver();
            return;
        }

        ChaseTarget(canSeePlayer);
        UpdateLostPlayerTimer(canSeePlayer);
    }

    /// <summary>Fills the meter while the player is visible, drains it otherwise.</summary>
    private void UpdateDetectionMeter(bool canSeePlayer)
    {
        float rate = canSeePlayer ? playerDetectionRate : -playerDetectionDischargeRate;
        float circlerate = canSeePlayer ? circleRateXY : -playerDetectionDischargeRate;

        currentPlayerDetection = Mathf.Clamp(
            currentPlayerDetection + (rate)  * Time.deltaTime,
            0f,
            maxPlayerDetection);

        isDetectionDischarging = !canSeePlayer && currentPlayerDetection > 0f && !canSeePlayer && currentCircleXY > 2f;

        // update the User Interface
        playerHUD.UpdateDetectionUI(currentPlayerDetection, maxPlayerDetection, minCircleXY, currentCircleXY, maxCircleXY);
    }

    private bool HasDetectedPlayer() => currentPlayerDetection >= maxPlayerDetection;

    /// <summary>Heads to the player while visible, otherwise to where they were last seen.</summary>
    private void ChaseTarget(bool canSeePlayer)
    {
        if (canSeePlayer)
            _lastKnownPlayerPosition = player.position;

        _agent.SetDestination(_lastKnownPlayerPosition);
    }

    /// <summary>Counts how long the player has been out of sight, then returns to patrol.</summary>
    private void UpdateLostPlayerTimer(bool canSeePlayer)
    {
        if (canSeePlayer)
        {
            _timeSinceLostPlayer = 0f;
            return;
        }

        _timeSinceLostPlayer += Time.deltaTime;

        if (_timeSinceLostPlayer >= losePlayerTime)
            ChangeState(EnemyState.Patrolling);
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
        // Null protection for transform or any component dependencies
        if (transform == null) return;

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

        enemyMoveSpeed = (enemyMoveSpeed / agroMultiplier);
    }

}
