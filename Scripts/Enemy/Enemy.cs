using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int healthPoints = 20;

    [Header("Idle Data")]
    public float idleTime;
    public float aggresionRange;

    [Header("Mode Data")]
    public float moveSpeed;
    public float chaseSpeed;
    public float turnSpeed;
    private bool manualMovement;
    private bool manualRotation;

    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;

    public Transform player {  get; private set; }
    public Animator anim {  get; private set; }
    public NavMeshAgent agent { get; private set; }

    protected EnemyStateMachine stateMachine {  get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        //player = GameObject.Find("Player").GetComponent<Player>();
        player = FindObjectOfType<Player>().transform;
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }

    protected virtual void Update()
    {
        
    }

    public virtual void HitImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(HitImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator HitImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(0.1f);

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    public virtual void GetHit()
    {
        healthPoints--;
    }

    public void ActivateManualMovement(bool _manualMovement) => this.manualMovement = _manualMovement;
    public bool ManualMovementActive() => manualMovement;
    public bool ActivateManualRotation(bool _manualRotation) => this.manualRotation = _manualRotation; 
    public bool ManualRotationActive() => manualRotation;

    public bool PlayerInAggresionRange() => Vector3.Distance(transform.position, player.position) < aggresionRange;

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPoints[currentPatrolIndex].transform.position;

        currentPatrolIndex++;
        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }

    private void InitializePatrolPoints()
    {
        foreach (Transform t in patrolPoints)
        {
            t.parent = null;
        }
    }

    public Quaternion FaceTarget(Vector3 target)
    {
        Quaternion targetLocation = Quaternion.LookRotation(target - transform.position);
        Vector3 currentEulerAngles = transform.root.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetLocation.eulerAngles.y, turnSpeed * Time.deltaTime);

        return Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);
    }
}
