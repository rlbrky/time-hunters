using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class CreatureSC : MonoBehaviour
{
    [Header("General Setup")]
    public NavMeshAgent agent;
    public Animator animator;
    [Header("Patrol Setup")]
    public List<Transform> patrolPoints;
    [Header("General Settings")] 
    public float walkSpeed, chaseSpeed, minIdleTime, maxIdleTime, catchDistance, minChaseTime, maxChaseTime;
    [Header("AI Vision Setup")] 
    public Vector3 raycastOffset;
    public float sightDistance;
    [Header("Area Patrol Points")]
    public List<Transform> area1_PatrolPoints;
    public List<Transform> area2_PatrolPoints;
    public List<Transform> area3_PatrolPoints;
    
    [HideInInspector] public Transform player;
    
    private Transform _currentDestination; //Randomly picked destination to go. If nothing happens.
    private Vector3 _destination; //Destination coordinates.
    private int _randomNu; //To determine a random destination.
    private int _patrolDecisionRndNu; //To determine if AI will stay on the spot or go somewhere randomly
    private float _idleRnd; //A random number between min and max idling times.
    private State _currentState;

    [Header("Player Hiding Control")] 
    [SerializeField] private float forgetHidingPointTimer; //Specifies the time AI forgets a point in hidingScore.
    [SerializeField] private float checkLocationThreshold; //If the closetScore is higher or equal to this number, monster will check the location that caused noise to surpass the threshold.
    //If player hides in closet this score goes up, with the specified time the AI will forget slowly.
    private int _playerHidInClosetScore;
    
    public enum State
    {
        Idling,
        Walking,
        Chasing,
    }

    public State GetCurrentState()
    {
        return _currentState;
    }

    private void Start()
    {
        _randomNu = Random.Range(0, patrolPoints.Count);
        _currentDestination = patrolPoints[_randomNu];
        _currentState = State.Walking;
    }

    private void Update()
    {
        if (player != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            RaycastHit hit;
            if (Physics.Raycast(transform.position + raycastOffset, directionToPlayer, out hit, sightDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    _currentState = State.Chasing;
                    StopCoroutine(IdlingCoroutine());
                    //TODO: Animations
                    StopCoroutine(ChaseCoroutine());
                    StartCoroutine(ChaseCoroutine());
                }
            }
        }
        else
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, sightDistance);
            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Player"))
                {
                    player = col.transform;
                    return;
                }
            }
        }
        
        switch (_currentState)
        {
            case State.Idling:
                _patrolDecisionRndNu = Random.Range(0, 2);
                switch (_patrolDecisionRndNu) //Randomly decide what to do.
                {
                    case 0: //Go to a new destination
                        _randomNu = Random.Range(0, patrolPoints.Count);
                        _currentDestination = patrolPoints[_randomNu];
                        _currentState = State.Walking;
                        break;
                    case 1: //Idle for a while.
                        //TODO:Set animations
                        agent.speed = 0;
                        StopCoroutine(IdlingCoroutine());
                        StartCoroutine(IdlingCoroutine());
                        break;
                }
                break;
                
            case State.Walking:
                _destination = _currentDestination.position;
                agent.SetDestination(_destination);
                agent.speed = walkSpeed;
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    _currentState = State.Idling;
                }
                break;
                
            case State.Chasing:
                _destination = player.position;
                agent.SetDestination(_destination);
                agent.speed = chaseSpeed;
                if (agent.remainingDistance <= catchDistance) //Monster can catch the player in range.
                {
                    //TODO: Kill player
                    _currentState = State.Idling;
                }
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, sightDistance);
    }

    public void StopChasing()
    {
        _currentState = State.Walking;
        StopCoroutine(ChaseCoroutine());
        _randomNu = Random.Range(0, patrolPoints.Count);
        _currentDestination = patrolPoints[_randomNu];
    }

    /// <summary>
    /// Adds to monster's memory that player hid in a hiding spot. If the player hides too much, the creature will check the last spot that surpassed the threshold.
    /// </summary>
    /// <param name="pointToApply">Noise value to apply</param>
    /// <param name="location">Position that player hid.</param>
    /// <param name="areaSpecifier">Area that this happened.</param>
    public void UpdatePlayerHidingScore(int pointToApply, Vector3 location, int areaSpecifier)
    {
        _playerHidInClosetScore += pointToApply;
        if (_playerHidInClosetScore > checkLocationThreshold)
        {
            switch (areaSpecifier)
            {
                case 0:
                    TeleportToClosestPointInArea(location, area1_PatrolPoints);
                    break;
                case 1:
                    TeleportToClosestPointInArea(location, area2_PatrolPoints);
                    break;
                case 2:
                    TeleportToClosestPointInArea(location, area3_PatrolPoints);
                    break;
            }
        }
    }

    /// <summary>
    /// Finds the closest location in the area specified, then checks the position given.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="areaPatrolPoints"></param>
    private void TeleportToClosestPointInArea(Vector3 position, List<Transform> areaPatrolPoints)
    {
        Transform pointToTeleport = CheckForClosestPoint(position, areaPatrolPoints);
        transform.position = pointToTeleport.position; //Teleport to the area.
    }

    private Transform CheckForClosestPoint(Vector3 pos, List<Transform> newPatrolPoints)
    {
        patrolPoints = newPatrolPoints;
        Transform closestPoint = null;
        foreach (Transform point in newPatrolPoints)
        {
            if (closestPoint == null)
                closestPoint = point; //Get first location as closest point.
            else
            {
                //If we have a point that is closer than the current one, select that point as the new closest point.
                if (Vector3.Distance(closestPoint.position, pos) > Vector3.Distance(point.position, pos))
                {
                    closestPoint = point;
                }
            }
        }

        return closestPoint;
    }
    
    private IEnumerator ChaseCoroutine()
    {
        float chaseTime = Random.Range(minChaseTime, maxChaseTime);
        yield return new WaitForSeconds(chaseTime);
        _currentState = State.Walking;
        _randomNu = Random.Range(0, patrolPoints.Count);
        _currentDestination = patrolPoints[_randomNu];
        //TODO:Set animations
    }

    private IEnumerator IdlingCoroutine()
    {
        _idleRnd = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(_idleRnd);
        _currentState = State.Walking;
        _randomNu = Random.Range(0, patrolPoints.Count);
        _currentDestination = patrolPoints[_randomNu];
        //TODO:Set animations
    }
}
