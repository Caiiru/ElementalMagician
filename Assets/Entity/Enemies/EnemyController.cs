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
    private Vector2 movementDirection;
    public float movementSpeed;
    public bool canMove;
    [Header("JUMP")] [Range(1f, 3f)] 
    public float JumpForce = 1f;
    
    public bool isGrounded = false;

    [Header("Area")]
    [Range(1.5f,5f)]
    public float attackArea;
    [Header("Vision")]
    [Range(1.5f,5f)]
    public float fieldOfView;
    public bool isSeeingPlayer;
    public float cooldownToReturn = 3f;
    public float timeSinceLastPlayerView = 0f;

    public LayerMask wallMask;

    private void Awake()
    {
        //Initial State
        state = EnemyState.Idle;

        movementDirection = Vector2.left;

        //Components
        _rb2D = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }
    void Start()
    {
        attackArea = 1.5f;
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
                target = GameManager.GetInstance().GetPlayerEntity().transform;
                 Patrol();
                if (seeTheTarget(target.transform))
                {
                    changeState(EnemyState.Pursuit);
                }
                break;
            case EnemyState.Pursuit:
                if (seeTheTarget(target.transform))
                {
                        lastTargetPosition = target.position;
                        MoveToPosition(lastTargetPosition);
                        timeSinceLastPlayerView = Time.time; 
                }
                else
                {
                    MoveToPosition(lastTargetPosition);
                    if (Time.time - timeSinceLastPlayerView > cooldownToReturn)
                    {
                        changeState(EnemyState.Patrol);
                    }
                    else
                    {
                    }
                }
                break;


            default: 
                break;
        }
    }
    private void changeState(EnemyState state)
    {
         
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

    private RaycastHit2D[] results;
    void Update()
    {
        if(canMove)
            MoveToPosition(target.transform.position);

        //isGrounded = Physics2D.CircleCast(getPosition(), 0.2f, getPosition() + Vector2.down);
        isGrounded = Physics2D.Raycast(getPosition(), Vector2.down, 1f);
        Debug.DrawRay(getPosition(), Vector2.down, Color.yellow);
        //Physics2D.RaycastNonAlloc(getPosition(), Vector2.down,results);
        //isGrounded = hit.collider.transform.CompareTag("Floor");
         
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
        Debug.DrawLine(transform.position, targetPosition, Color.green);
        if (!isOnTarget(targetPosition))
        {
            Debug.Log("NOT IN TARGET YEEEEEEEEEEEEEEEEEEET");
            var _direction = (targetPosition - getPosition());  
            Debug.DrawLine(transform.position,  getPosition() + _direction, Color.magenta);
            // _rb2D.velocity = new Vector2(_direction.x * movementSpeed, _rb2D.velocity.y);
            //transform.position += new Vector3(_direction.x * movementSpeed * Time.deltaTime, transform.position.y,
                //transform.position.z);
            //transform.position += (new Vector3(_direction.x, getPosition().y, 0) * movementSpeed) * Time.deltaTime;
            //transform.position+= new Vector3(_direction.x * 1f * Time.deltaTime,transform.position.y);
            if (_direction.y >= 1 && isGrounded)
            {
                Jump();
            }
        }
        else
        {
            
        }
        
    }

    void Patrol()
    {
        if (Physics2D.Raycast(getPosition(), Vector2.left,1f, wallMask ) || Physics2D.Raycast(getPosition(), Vector2.right,1f, wallMask ))
        {
            ToggleDirection();
        }
        MoveToPosition(getPosition() + movementDirection);
    }

    private void ToggleDirection()
    {
        movementDirection *= -1;
    }

    private void Jump()
    {
        _rb2D.AddForce(Vector2.up * JumpForce,ForceMode2D.Impulse);
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