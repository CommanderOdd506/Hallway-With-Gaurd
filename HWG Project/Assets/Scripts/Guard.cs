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
    private Animator animator;
    public Transform neckPivot;
    private EnemyState currentState;
    public EnemyState startingState;
    public float viewDistance = 10f;
    public float searchBuffer = 60f;
    public float innerDetectionRadius = 3f;
    public float distractionDistance = 20f;
    public float distractionTimer = 15f;
    public float normalSpeed = 2f;
    public float aggroSpeed = 3.2f;
    public float rotateSpeed = 4f;
    public float headRotateSpeed = 6f;

    public LayerMask sightBlockLayers;
    public Transform eyePoint;


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
    private Quaternion defaultRotation;

    [Range(0, 180)]
    public float viewAngle = 90f;


    void Start()
    {
        _timeSinceLastSeen = searchBuffer + 0.01f;
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindObjectOfType<PlayerMovement>().transform;
        animator = GetComponentInChildren<Animator>();
        currentState = startingState;
        defaultRotation = neckPivot.localRotation;
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
            Vector3 dirToPlayer = (player.position - eyePoint.position).normalized;
            float distance = Vector3.Distance(eyePoint.position, player.position);

            RaycastHit hit;

            if (Physics.Raycast(eyePoint.position, dirToPlayer, out hit, distance))
            {
                if (hit.transform == player)
                {
                    _seesPlayer = true;
                    _aggroMemoryTimer = aggroMemoryDuration;
                    _timeSinceLastSeen = 0f;
                    lastSeenSpot = player.position;
                }
            }
        }
        else
        {
            if (_aggroMemoryTimer > 0f)
            {
                _aggroMemoryTimer -= Time.deltaTime;
                _seesPlayer = true;
                _isDistracted = false;

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
        Debug.Log("Distracted");
        float dis = Vector3.Distance(targetPoint.position, transform.position);
        if (dis <= distractionDistance && currentState != EnemyState.Aggro)
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
        UpdateAnimator();

        if (currentState == EnemyState.Aggro)
        {
            navMeshAgent.speed = aggroSpeed;
            RotateHead();
        }
        else
        {
            navMeshAgent.speed = normalSpeed;
            neckPivot.localRotation = Quaternion.Slerp(
            neckPivot.localRotation,
            defaultRotation,
            headRotateSpeed * Time.deltaTime
            );
        }

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

    float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }

    private void RotateHead()
    {
        Vector3 direction = player.position - neckPivot.position;
        Quaternion worldTargetRotation = Quaternion.LookRotation(direction);

        // Convert to local space
        Quaternion localTargetRotation = Quaternion.Inverse(neckPivot.parent.rotation) * worldTargetRotation;

        Vector3 localEuler = localTargetRotation.eulerAngles;

        localEuler.x = NormalizeAngle(localEuler.x);
        localEuler.y = NormalizeAngle(localEuler.y);

        localEuler.x = Mathf.Clamp(localEuler.x, -30f, 30f);
        localEuler.y = Mathf.Clamp(localEuler.y, -60f, 60f);

        // Rebuild rotation
        Quaternion clampedRotation = Quaternion.Euler(localEuler);

        neckPivot.localRotation = Quaternion.Slerp(neckPivot.localRotation,clampedRotation,headRotateSpeed * Time.deltaTime);
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

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= navMeshAgent.stoppingDistance + 0.2f)
        {
            navMeshAgent.updateRotation = false;

            Vector3 lookDirection = player.position - transform.position;
            lookDirection.y = 0f;

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime
                );
            }
        }
        else
        {
            navMeshAgent.updateRotation = true;
        }
    }

    private void UpdateAnimator()
    {
        float speedPercent = navMeshAgent.velocity.magnitude / aggroSpeed;
        animator.SetFloat("Speed", speedPercent);

        animator.SetBool("IsAggro", currentState == EnemyState.Aggro);
        animator.SetBool("IsSearching", currentState == EnemyState.Search);
        animator.SetBool("IsDistracted", currentState == EnemyState.Distracted);
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
