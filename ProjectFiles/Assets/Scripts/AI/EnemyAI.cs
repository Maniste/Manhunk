using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    protected EnemyBaseState currentState;
    private string currentStateName = "a";
    private EnemyPatrolState patrolState => new EnemyPatrolState(this);
    public EnemyPatrolState PatrolState { get { return patrolState; } }

    private EnemyChaseState chaseState => new EnemyChaseState(this);
    public EnemyChaseState ChaseState { get { return chaseState; } }

    private EnemyInvestigateState investigateState => new EnemyInvestigateState(this);
    public EnemyInvestigateState InvestigateState { get { return investigateState; } }

    private EnemySearch searchState => new EnemySearch(this);
    public EnemySearch SearchState { get { return searchState; } }

    private EnemyAttackState attackState => new EnemyAttackState(this);
    public EnemyAttackState AttackState { get { return attackState; } }


    //////////////////////////////////////////////////////////////////////
    //ENEMY STATS
    //////////////////////////////////////////////////////////////////////
    private bool _canSeePlayer = false;
    public bool CanSeePlayer { get { return _canSeePlayer; } }

    private int enemyAgitation = 0;
    public int EnemyAgitation { get { return enemyAgitation; }set { enemyAgitation = value; } }

    private int timesPlayerHasBeenSeen = 0;
    public int TimesPlayerHasBeenSeen { get { return timesPlayerHasBeenSeen; } set { timesPlayerHasBeenSeen = value; } }

    [SerializeField] private int attackDamage = 0;
    public int AttackDamage { get { return attackDamage; } }


    [SerializeField] private float wanderSpeed = 0f;
    public float WanderSpeed { get { return wanderSpeed; } }

    [SerializeField] private float searchSpeed = 0f;
    public float SearchSpeed { get { return searchSpeed; } }

    [SerializeField] private float runSpeed = 0f;
    public float RunSpeed { get { return runSpeed; } }

    [SerializeField] private float turnSpeed = 0f;
    public float TurnSpeed { get { return turnSpeed; } }

    [SerializeField] private float fieldOfView = 0f;

    [SerializeField] private float _detectionTime = 1f;
    public float DetectionTime { get { return _detectionTime; } }

    private float currentDetectionValue = 0f;
    public float CurrentDetectionValue { get { return currentDetectionValue; } }

    private float _investigateTime = 1f;
    public float InvestigateTime { get { return _investigateTime; } }

    private float _currentTime = 0f;


    private Vector3 _playerLastLocation = Vector3.zero;
    public Vector3 PlayerLastLocation
    {
        get { return _playerLastLocation; }
        set
        {
            _playerLastLocation = value;
        }
    }

    private Vector3 _noiseLocation = Vector3.zero;
    public Vector3 NoiseLocation { get { return _noiseLocation; } set { _noiseLocation = value; } }

    [SerializeField] private Transform _player = null;
    public Transform Player { get { return _player; } }

    [SerializeField] private LayerMask playerLayer = 6;
    [SerializeField] private LayerMask playerMask = 6;
    public LayerMask PlayerLayer { get { return playerLayer; } }


    [SerializeField] private SpriteRenderer detectionRenderer = null;
    [SerializeField] private Sprite[] detectionSprites; //Make sure 0 = ? and 1 = !

    [SerializeField] private MapPointsOfIntrests pointsOfIntrest = null;
    public MapPointsOfIntrests PointsOfIntrest{get{ return pointsOfIntrest; } }

    //0 = idel, 1 = walk, 2 = run
    [SerializeField] private AnimationClip[] _clipsArray = null;
    public AnimationClip[] ClipsArray { get { return _clipsArray; } }

    private Animation _animator = null;
    public Animation _Animator { get { return _animator; } }

    [SerializeField] private BoxCollider _attackHitBox = null;
    public BoxCollider AttackHitBox { get {return _attackHitBox; } }

    private PlayerStealth _pStealth = null;

    private NavMeshAgent _agent = null;
    public NavMeshAgent Agent { get { return _agent; } }


    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _pStealth = _player.GetComponent<PlayerStealth>();
        _animator = transform.GetChild(0).transform.GetComponent<Animation>();
    }

    private void Start()
    {
        SetState(patrolState);
        detectionRenderer.color = SetDetectionColor(currentDetectionValue);
    }

    public void SetState(EnemyBaseState state)
    {
        Debug.Log("Change State");
        currentStateName = state.ToString();
        currentState = state;
        currentState.StateStart();
    }

    public void MoveToPlayer()
    {
        _agent.SetDestination(_player.position);
    }

    public void MoveToPlayersLastKnownLocation()
    {
        _agent.SetDestination(_playerLastLocation);
    }

    public void EnemyInsideNoiseRad(Vector3 noisePos)
    {
        _playerLastLocation = noisePos;

        //If already chasing the player just ignore
        if (currentState.GetType() == typeof(EnemyChaseState))
            return;

        if (GetCurrentAgitationLevel() == 0)
        {
            Debug.Log("hmmm whats that noise?");
            SetState(investigateState);
        }
        else if (GetCurrentAgitationLevel() == 1)
        {
            Debug.Log("where the hell are you?!");
            SetState(searchState);
        }
        else if (GetCurrentAgitationLevel() == 2)
        {
            Debug.Log("I know you are there!");
            SetState(chaseState);
        }
    }

    public float DistanceBetweenObjets(Vector3 A, Vector3 B)
    {
        float root = (Mathf.Pow(A.x - B.x, 2) + Mathf.Pow(A.y - B.y, 2) + Mathf.Pow(A.z - B.z, 2));
        return Mathf.Sqrt(root);
    }

    public bool DoesntHaveLineOfSight()
    {
        bool state = Physics.Linecast(transform.position, _player.position, playerMask);
        return state;
    }

    public bool IsInTheFieldOfView()
    {
    
        Vector3 forwardDir = transform.forward;
        Vector3 playerDor = (_player.position - transform.position).normalized;
        float dot = Vector3.Dot(forwardDir, playerDor);

        if (dot > fieldOfView)
            return true;
        else
            return false;
    }

    public bool IsEnemyAtPoint(Vector3 pos, Vector3 targetPos)
    {
        float dist = DistanceBetweenObjets(pos, targetPos);

        if (dist < 0.5f)
            return true;
        else
            return false;
    }

    private bool IsTooClose()
    {
        float dist = Mathf.Sqrt((transform.position.x - _player.position.x) * (transform.position.x - _player.position.x)
            + (transform.position.y - _player.position.y) * (transform.position.y - _player.position.y)
            + (transform.position.z - _player.position.z) * (transform.position.z - _player.position.z));

        if (dist < 3f)
            return true;
        else
            return false;
    }

    private bool CheckDetectionValue()
    {
        if (currentDetectionValue >= _detectionTime)
        {
            if (detectionRenderer.sprite != detectionSprites[1])
                detectionRenderer.sprite = detectionSprites[1];

            DetectionRing.OnIsBeingDetected(this);
            return true;
        }
        else
        {
            currentDetectionValue += Time.deltaTime;

            DetectionRing.OnIsBeingDetected(this);
            return false;
        }
    }

    private int GetCurrentAgitationLevel()
    {
        if (timesPlayerHasBeenSeen <= 1)
            return 0;
        else if (timesPlayerHasBeenSeen <= 3)
            return 1;
        else
            return 2;
    }

    private float LowerDetectionValue()
    {
        float newValue = currentDetectionValue -= Time.deltaTime;
        if (newValue < 0f)
            newValue = 0f;

        DetectionRing.OnIsBeingDetected(this);
        return newValue;
    }

    private Color SetDetectionColor(float value)
    {
        float minValue = (_detectionTime / 100);
        float currentValue = minValue * (value * _detectionTime);
        float test = Mathf.InverseLerp(0, _detectionTime, value);

        return new Color(test, 1f, 1f, test);
    }

    private Vector3 GetClosetPointInsideOfNavMesh(Vector3 point, float maxDistance = 5f)
    {
        NavMeshHit hit;
        bool checkIfFound = NavMesh.SamplePosition(point, out hit, maxDistance, NavMesh.AllAreas);

        if (checkIfFound)
        {
            Vector3 hitPoint = hit.position;

            NavMeshPath path = new NavMeshPath();
            _agent.CalculatePath(hitPoint, path);

            if (path.status == NavMeshPathStatus.PathComplete)
                return hitPoint;
            else
            {
                Debug.Log("Path not completable");
                return transform.position;
            }
        }
        else
        {
            Debug.Log("couldnt find point");
            //If worst case, dont move
            return transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {

        //If player is seen outside of shadows
        if (IsInTheFieldOfView() && !DoesntHaveLineOfSight() && !_pStealth.IsPlayerHidden)
        {
            detectionRenderer.color = SetDetectionColor(currentDetectionValue);
            if (CheckDetectionValue())
            {
                _canSeePlayer = true;
            }
        }
        // Make sure player can be seen if they are too close even when hidding
        else if (IsInTheFieldOfView() && !DoesntHaveLineOfSight() && IsTooClose())
        {
            _canSeePlayer = true;
        }
        //Player not seen
        else
        {
            if (currentDetectionValue > 0)
            {
                if (detectionRenderer.sprite != detectionSprites[0])
                    detectionRenderer.sprite = detectionSprites[0];

                detectionRenderer.color = SetDetectionColor(currentDetectionValue);
                currentDetectionValue = LowerDetectionValue();
            }

            _canSeePlayer = false;
        }


        currentState.StateUpdate();
    }

    private void OnDrawGizmos()
    {
        if (AttackHitBox != null)
        {
            Matrix4x4 rotation = Matrix4x4.TRS(
                AttackHitBox.transform.position,
                AttackHitBox.transform.rotation,
                AttackHitBox.transform.lossyScale);
            Gizmos.matrix = rotation;

            Gizmos.color = Color.red;
            Gizmos.DrawCube(AttackHitBox.center, AttackHitBox.size);
        }
    }

}
