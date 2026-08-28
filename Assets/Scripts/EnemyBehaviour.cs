using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



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

    [Header("Settings")]
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float stopAtDistance = 0.5f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float losePlayerTime = 3f;

    private NavMeshAgent _agent;
    private Animator _animator;
    private EnemyState _state = EnemyState.Patrolling;
    private int _currentPatrolIndex;
    private bool _isWaiting;
    private float _timeSinceLostPlayer;

    private IEnumerator WaitAtPatrolPoint()
    {
        _isWaiting = true;
        _agent.isStopped = true;

        yield return new WaitForSeconds(patrolWaitTime);

        _agent.isStopped = false;
        GoToNextPatrolPoint();
        _isWaiting = false;
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }


    // Start is called before the first frame update
    void Start()
    {
        GoToNextPatrolPoint();
    }

    // Update is called once per frame
    void Update()
    {
        var distanceToPlayer = Vector3.Distance(player.position, transform.position);

        switch (_state)
        {
            case EnemyState.Patrolling:
                Patrol();
                if (distanceToPlayer <= detectionRange && CanSeePlayer())
                {
                    _state = EnemyState.Detecting;
                }
                break;

            case EnemyState.Detecting:
                FollowPlayer();
                if(!CanSeePlayer())
                {
                    _timeSinceLostPlayer += Time.deltaTime;
                    if(_timeSinceLostPlayer >= losePlayerTime)
                    {
                        _state = EnemyState.Patrolling;
                        GoToClosestPatrolPoint();
                    }
                    else
                    {
                        // if the enemy spots the player while following him
                        _timeSinceLostPlayer = 0f;
                    }
                    break;
                }
                break;
        }

    }

    void FollowPlayer()
    {

        _agent.SetDestination(player.position);
        LogHandler.Log($"Destination set to: {player}");
    }

    private void Patrol()
    {
        if (_isWaiting) return;
        if(!_agent.pathPending && _agent.remainingDistance <= stopAtDistance)
        {
            return;
        }
    }
    public void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        _agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
        _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPoints.Length;

        LogHandler.Log($"Making way towards Tree No: {_currentPatrolIndex}");
    }

    void GoToClosestPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        var closestIndex = 0;
        var closestDistance = float.MaxValue;

        for (var i = 0; i < patrolPoints.Length; i++)
        {
            var distance = Vector3.Distance(transform.position, patrolPoints[i].position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        _currentPatrolIndex = closestIndex;
        _agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
    }

    bool CanSeePlayer()
    {
        return IsFacingPlayer() && HasClearPathToPlayer();
    }

    bool IsFacingPlayer()
    {
        //calculate direction to the player.
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        var angle = Vector3.Angle(transform.forward, dirToPlayer);
        return angle <= viewAngle / 2f;
    }

    //check if there are any obstacles in between enemy and player
    bool HasClearPathToPlayer()
    {
        var dirToPlayer = player.position - transform.position;
        
        //shoot a raycast in the direction to the player to check if the path is clear
        if(Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, dirToPlayer.magnitude))
        {
            return hit.transform == player;
        }

        return true;
    }
}
