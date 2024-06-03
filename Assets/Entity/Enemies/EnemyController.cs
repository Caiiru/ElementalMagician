using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    public EnemyState state;

    private Rigidbody2D _rb2D;
    private Collider2D _collider;
    [Space]
    [Header("Movement")]
    public Transform target;
    public Vector2 lastTargetPosition;
    public float movementSpeed;
    public bool canMove;

    [Header("Area")]
    [Range(1.5f,5f)]
    public float attackArea;
    [Header("Vision")]
    public float fieldOfView;
    public bool isSeeingPlayer;
    public float cooldownToReturn = 3f;

    private void Awake()
    {
        //Initial State
        state = EnemyState.Idle;

        //Components
        _rb2D = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }
    void Start()
    {
        canMove = false;
        
    }
    private void CheckCurrentState()
    {
        switch (state)
        {
            case EnemyState.Idle:
                changeState(EnemyState.Patrol);
                break;
            case EnemyState.Patrol:
                target = GameManager.getInstance().getPlayerEntity().transform;
                if (seeTheTarget(target.transform))
                {
                    changeState(EnemyState.Pursuit);
                }
                break;
            case EnemyState.Pursuit:
                if (seeTheTarget(target.transform))
                {
                        lastTargetPosition = target.position;
                        MoveToPosition(target.position);
                }
                else
                {
                    MoveToPosition(lastTargetPosition);
                    var timeSinceLastPlayerView = Time.time;
                    if (Time.time - timeSinceLastPlayerView > cooldownToReturn)
                    {
                        Debug.Log("Return to patrol");
                        changeState(EnemyState.Patrol);
                    }
                }
                break;


            default:
                Debug.LogError("Enemey " + transform.name + " has no state");
                break;
        }
    }
    private void changeState(EnemyState state)
    {
        
        Debug.Log("Change state to " + state.ToString());
        this.state = state;
    }
    private bool seeTheTarget(Transform target)
    {
        if((new Vector2(target.position.x, target.position.y) - getPosition()).sqrMagnitude < fieldOfView*fieldOfView)
        {
            isSeeingPlayer = true;
            return true;
        }
        isSeeingPlayer = false;

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove)
            MoveToPosition(target.transform.position);
    }
    private void FixedUpdate()
    {
        CheckCurrentState();
    }
    public Vector2 getPosition()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    public void MoveToPosition(Vector2 targetPosition)
    {
        if (!isOnTarget(targetPosition))
        {
            var _direction = (targetPosition - getPosition()).normalized;
            Debug.DrawLine(getPosition(), new Vector2(getPosition().x +_direction.x, getPosition().y), Color.red);
            // _rb2D.velocity = new Vector2(_direction.x * movementSpeed, _rb2D.velocity.y);
            transform.position += (new Vector3(_direction.x, _direction.y, 0) * movementSpeed) * Time.deltaTime;
        }
        else
        {
            
        }
        
    }

    private bool isOnTarget(Vector2 target)
    {
        Vector2 offset = target - getPosition();
        float sqr = offset.sqrMagnitude;
        if (sqr < attackArea * attackArea)
        {
            Debug.Log("IS ON ATTACK AREA");
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(getPosition(), attackArea);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(getPosition(), fieldOfView);
    }


}





public enum EnemyState
{
    Idle,
    Patrol,
    Pursuit
}