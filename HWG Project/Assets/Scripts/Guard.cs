using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public enum EnemyState
{
    Patrol,
    Aggro,
    Search,
    Distracted,
    Stunned
}



public class Guard : MonoBehaviour
{

    // -----------------------------
    // Patrol Settings
    // -----------------------------

    public Transform[] PatrolPoints;
    public Transform[] newPatrolPoints;

    private int _currentPatrolIndex;
    private bool _inLastRoom;


    // -----------------------------
    // Core Components
    // -----------------------------

    private NavMeshAgent navMeshAgent;
    private Transform player;
    private Animator animator;

    public Transform neckPivot;
    public Transform eyePoint;


    // -----------------------------
    // State Machine
    // -----------------------------

    private EnemyState currentState;
    private EnemyState previousState;

    public EnemyState startingState;


    // -----------------------------
    // Vision Settings
    // -----------------------------

    public float viewDistance = 10f;
    public float viewAngle = 90f;

    public float innerDetectionRadius = 3f;
    public LayerMask sightBlockLayers;

    private bool _seesPlayer;


    // -----------------------------
    // Search Behavior
    // -----------------------------

    public float searchBuffer = 60f;

    private float _timeSinceLastSeen;
    private Vector3 lastSeenSpot;

    public float aggroMemoryDuration = 4f;
    private float _aggroMemoryTimer;


    // -----------------------------
    // Distraction System
    // -----------------------------

    public float distractionDistance = 20f;
    public float distractionTimer = 15f;

    private bool _isDistracted;
    private Vector3 distractionPoint;
    private float _distractionTimer;


    // -----------------------------
    // Movement Speeds
    // -----------------------------

    public float normalSpeed = 2f;
    public float aggroSpeed = 3.2f;

    public float finalAggroSpeed = 4.2f;
    public float finalWalkSpeed = 3f;

    public float rotateSpeed = 4f;
    public float headRotateSpeed = 6f;


    // -----------------------------
    // Stun System
    // -----------------------------

    public float stunDuration = 3f;

    private bool _isStunned;
    private float _stunTimer;


    // -----------------------------
    // Audio (Voice Lines)
    // -----------------------------

    public AudioSource voiceSource;

    public AudioClip[] spottedClips;
    public AudioClip[] searchClips;
    public AudioClip[] distractedClips;
    public AudioClip[] stunnedClips;

    public AudioSource stunSource;
    public AudioClip bonk;


    // -----------------------------
    // Goblin Head UI
    // -----------------------------

    public GameObject[] goblinHeads;
    public float goblinHeadDistance = 35f;

    private int _currentHeadIndex;
    private int _lastHeadIndex = -1;
    private bool _lastHeadVisible = false;


    // -----------------------------
    // Head Rotation
    // -----------------------------

    private Quaternion defaultRotation;



    // =========================================================
    // Initialization
    // =========================================================

    void Start()
    {
        _timeSinceLastSeen = searchBuffer + 0.01f;

        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindObjectOfType<PlayerMovement>().transform;
        animator = GetComponentInChildren<Animator>();

        currentState = startingState;

        defaultRotation = neckPivot.localRotation;
    }



    // =========================================================
    // Vision / Sight Detection
    // =========================================================

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
                inVision = true;
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



    // =========================================================
    // State Machine
    // =========================================================

    void SetState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        previousState = currentState;
        currentState = newState;

        OnStateEnter(newState);
    }


    void OnStateEnter(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Aggro:
                PlayVoiceLine("SpottedPlayer");
                break;

            case EnemyState.Search:
                PlayVoiceLine("Searching");
                break;

            case EnemyState.Distracted:
                PlayVoiceLine("WhatWasThat");
                break;

            case EnemyState.Stunned:
                PlayVoiceLine("Stunned");
                break;
        }
    }


    public void CheckState()
    {
        if (_isStunned)
        {
            SetState(EnemyState.Stunned);
            return;
        }

        if (_seesPlayer)
        {
            SetState(EnemyState.Aggro);
        }
        else if (_isDistracted)
        {
            SetState(EnemyState.Distracted);
        }
        else if (_timeSinceLastSeen < searchBuffer)
        {
            SetState(EnemyState.Search);
        }
        else
        {
            SetState(EnemyState.Patrol);
        }
    }



    // =========================================================
    // Audio
    // =========================================================

    void PlayVoiceLine(string type)
    {
        AudioClip[] clips = null;

        switch (type)
        {
            case "SpottedPlayer": clips = spottedClips; break;
            case "Searching": clips = searchClips; break;
            case "WhatWasThat": clips = distractedClips; break;
            case "Stunned": clips = stunnedClips; break;
        }

        if (clips == null || clips.Length == 0) return;

        int index = Random.Range(0, clips.Length); // fixed: full range now reachable

        voiceSource.Stop();                        // fixed: interrupt previous line
        voiceSource.PlayOneShot(clips[index]);
    }




    // =========================================================
    // External Events
    // =========================================================

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


    public void Stun()
    {
        _isStunned = true;
        _stunTimer = stunDuration;

        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;

        SetState(EnemyState.Stunned);
        
        
        stunSource.PlayOneShot(bonk, 1.8f);
        
    }



    // =========================================================
    // Update Loop
    // =========================================================

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

            case EnemyState.Stunned:
                StunnedState();
                break;

            default:
                PatrolState();
                break;
        }
    }



    // =========================================================
    // State Behaviors
    // =========================================================

    private void PatrolState()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (_inLastRoom)
            {
                _currentPatrolIndex = (_currentPatrolIndex + 1) % newPatrolPoints.Length;
                navMeshAgent.SetDestination(newPatrolPoints[_currentPatrolIndex].position);
            }
            else
            {
                _currentPatrolIndex = (_currentPatrolIndex + 1) % PatrolPoints.Length;
                navMeshAgent.SetDestination(PatrolPoints[_currentPatrolIndex].position);
            }
        }
    }


    private void SearchState()
    {
        navMeshAgent.SetDestination(lastSeenSpot);
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

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotateSpeed * Time.deltaTime
                );
            }
        }
        else
        {
            navMeshAgent.updateRotation = true;
        }
    }


    private void StunnedState()
    {
        _stunTimer -= Time.deltaTime;

        neckPivot.localRotation = defaultRotation;

        if (_stunTimer <= 0f)
        {
            _isStunned = false;

            navMeshAgent.isStopped = false;
            navMeshAgent.updateRotation = true;

            lastSeenSpot = player.position;
            _aggroMemoryTimer = aggroMemoryDuration;
            _timeSinceLastSeen = 0f;
            _seesPlayer = true;
        }
    }



    // =========================================================
    // Animation
    // =========================================================

    private void UpdateAnimator()
    {
        float speedPercent = navMeshAgent.velocity.magnitude / aggroSpeed;

        animator.SetFloat("Speed", speedPercent);
        animator.SetBool("IsStunned", currentState == EnemyState.Stunned);
        animator.SetBool("IsAggro", currentState == EnemyState.Aggro);
        animator.SetBool("IsSearching", currentState == EnemyState.Search);
        animator.SetBool("IsDistracted", currentState == EnemyState.Distracted);
    }



    // =========================================================
    // Head Rotation
    // =========================================================

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

        Quaternion localTargetRotation =
            Quaternion.Inverse(neckPivot.parent.rotation) * worldTargetRotation;

        Vector3 localEuler = localTargetRotation.eulerAngles;

        localEuler.x = NormalizeAngle(localEuler.x);
        localEuler.y = NormalizeAngle(localEuler.y);

        localEuler.x = Mathf.Clamp(localEuler.x, -30f, 30f);
        localEuler.y = Mathf.Clamp(localEuler.y, -60f, 60f);

        Quaternion clampedRotation = Quaternion.Euler(localEuler);

        neckPivot.localRotation = Quaternion.Slerp(
            neckPivot.localRotation,
            clampedRotation,
            headRotateSpeed * Time.deltaTime
        );
    }



    // =========================================================
    // Goblin Head UI
    // =========================================================

    private void UpdateGoblinHeads()
    {
        float dis = Vector3.Distance(player.position, transform.position);
        bool shouldShow = dis <= goblinHeadDistance;

        int headIndex = 0;
        switch (currentState)
        {
            case EnemyState.Aggro: headIndex = 2; break;
            case EnemyState.Distracted:
            case EnemyState.Search: headIndex = 1; break;
            case EnemyState.Patrol: headIndex = 0; break;
            case EnemyState.Stunned: headIndex = 3; break;
        }

        if (headIndex == _lastHeadIndex && shouldShow == _lastHeadVisible) return;

        _lastHeadIndex = headIndex;
        _lastHeadVisible = shouldShow;

        for (int i = 0; i < goblinHeads.Length; i++)
            goblinHeads[i].SetActive(false);

        if (shouldShow)
            goblinHeads[headIndex].SetActive(true);
    }


    // =========================================================
    // Room Switch
    // =========================================================

    public void SwitchRoom()
    {
        _inLastRoom = true;

        aggroSpeed = finalAggroSpeed;
        normalSpeed = finalWalkSpeed;

        if (currentState == EnemyState.Aggro)
            navMeshAgent.speed = aggroSpeed;
        else
            navMeshAgent.speed = normalSpeed;
    }



    // =========================================================
    // Gizmos
    // =========================================================

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