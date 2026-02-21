using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrol,
    Aggro,
    Search,
    Distracted
}
public class Guard : MonoBehaviour
{
    public Transform[] PatrolPoints;
    private NavMeshAgent navMeshAgent;
    private Transform player;
    private EnemyState currentState;
    public EnemyState startingState;
    public float viewDistance = 10f;
    public float searchBuffer = 60f;
    public float innerDetectionRadius = 3f;
    public float distractionDistance = 20f;
    public float distractionTimer = 15f;

    public GameObject[] goblinHeads;
    public float goblinHeadDistance = 35f;

    private bool _seesPlayer;
    private bool _isDistracted;
    private float _timeSinceLastSeen;
    private Vector3 lastSeenSpot;
    private Vector3 distractionPoint;
    private float _distractionTimer;
    private int _currentPatrolIndex;
    private int _currentHeadIndex;
    private float _aggroMemoryTimer;
    public float aggroMemoryDuration = 4f;

    [Range(0, 180)]
    public float viewAngle = 90f;


    void Start()
    {
        _timeSinceLastSeen = searchBuffer + 0.01f;
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindObjectOfType<PlayerMovement>().transform;
        currentState = startingState;
    }

    void CheckSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        if (distanceToPlayer <= innerDetectionRadius)
        {
            _seesPlayer = true;
            _aggroMemoryTimer = aggroMemoryDuration;
            _timeSinceLastSeen = 0f;
            lastSeenSpot = player.position;
            return;
        }
        bool inVision = false;

        if (distanceToPlayer <= viewDistance)
        {
            directionToPlayer.Normalize();
            float dot = Vector3.Dot(transform.forward, directionToPlayer);

            if (dot > Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad))
            {
                inVision = true;
            }
        }

        if (inVision)
        {
            _seesPlayer = true;
            _aggroMemoryTimer = aggroMemoryDuration;
            _timeSinceLastSeen = 0f;
            lastSeenSpot = player.position;
        }
        else
        {
            if (_aggroMemoryTimer > 0f)
            {
                _aggroMemoryTimer -= Time.deltaTime;
                _seesPlayer = true;

                // Keep updating last position while in memory chase
                lastSeenSpot = player.position;
                _timeSinceLastSeen = 0f;
            }
            else
            {
                _seesPlayer = false;
                _timeSinceLastSeen += Time.deltaTime;
            }
        }
    }

    public void TryDistraction(Transform targetPoint)
    {
        float dis = Vector3.Distance(targetPoint.position, transform.position);
        if (dis <= distractionDistance)
        {
            distractionPoint = targetPoint.position;
            _isDistracted = true;
        }
    }

    private void UpdateGoblinHeads()
    {
        float dis = Vector3.Distance(player.position, transform.position);

        int headIndex = 0;

        switch (currentState)
        {
            case EnemyState.Aggro:
                headIndex = 2;
                break;

            case EnemyState.Distracted:
            case EnemyState.Search:
                headIndex = 1;
                break;

            case EnemyState.Patrol:
                headIndex = 0;
                break;
        }

        for (int i = 0; i < goblinHeads.Length; i++)
            goblinHeads[i].SetActive(false);

        if (dis <= goblinHeadDistance)
            goblinHeads[headIndex].SetActive(true);
    }
    public void CheckState()
    {
        if (_seesPlayer)
        {
            currentState = EnemyState.Aggro;
        }
        else if (_isDistracted)
        {
            currentState = EnemyState.Distracted;
        }
        else if (_timeSinceLastSeen < searchBuffer)
        {
            currentState = EnemyState.Search;
        }
        else
        {
            currentState = EnemyState.Patrol;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckSight();
        CheckState();
        UpdateGoblinHeads(); 

        switch (currentState)
        {
            case EnemyState.Aggro:
                AggroState();
                break;
            case EnemyState.Search:
                SearchState();
                break;
            case EnemyState.Patrol:
                PatrolState();
                break;
            case EnemyState.Distracted:
                DistractedState();
                break;
            default:
                PatrolState();
                break;
        }
    }
    private void DistractedState()
    {
        navMeshAgent.SetDestination(distractionPoint);

        _distractionTimer += Time.deltaTime;

        if (_distractionTimer >= distractionTimer)
        {
            _isDistracted = false;
            _distractionTimer = 0f;
        }
    }

    private void SearchState()
    {
        navMeshAgent.SetDestination(lastSeenSpot);
    }

    private void AggroState()
    {
        navMeshAgent.SetDestination(player.position);
    }

    private void PatrolState()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            _currentPatrolIndex = (_currentPatrolIndex + 1) % PatrolPoints.Length;
            navMeshAgent.SetDestination(PatrolPoints[_currentPatrolIndex].position);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 forward = transform.forward;

        Quaternion leftRayRotation = Quaternion.Euler(0, -viewAngle * 0.5f, 0);
        Quaternion rightRayRotation = Quaternion.Euler(0, viewAngle * 0.5f, 0);

        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, leftRayDirection * viewDistance);
        Gizmos.DrawRay(transform.position, rightRayDirection * viewDistance);
    }
}
