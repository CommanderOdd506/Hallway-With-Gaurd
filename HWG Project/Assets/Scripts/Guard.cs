using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrol,
    Aggro
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

    private bool _seesPlayer;
    private float _timeSinceLastSeen;
    private Vector3 lastSeenSpot;

    [Range(0, 180)]
    public float viewAngle = 90f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

        if (distanceToPlayer > viewDistance)
        {
            _seesPlayer = false;
            _timeSinceLastSeen += Time.deltaTime;
            return;
        }

        directionToPlayer.Normalize();

        float dot = Vector3.Dot(transform.forward, directionToPlayer);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (angle < viewAngle * 0.5f)
        {
            _seesPlayer = true;
            _timeSinceLastSeen = 0;
            lastSeenSpot = player.position;
        }
        else
        {
            _timeSinceLastSeen+= Time.deltaTime;
            _seesPlayer = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckSight();
        if (_seesPlayer)
        {
            navMeshAgent.SetDestination(player.position);
        }
        else if (_timeSinceLastSeen < searchBuffer)
        {
            navMeshAgent.SetDestination(lastSeenSpot);
        }
        else
        {
            PatrolState();
        }


    }

    public void PatrolState()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            int index = Random.Range(0, PatrolPoints.Length);
            navMeshAgent.SetDestination(PatrolPoints[index].position);
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
