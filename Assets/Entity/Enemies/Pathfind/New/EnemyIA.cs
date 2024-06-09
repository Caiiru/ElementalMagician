using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class EnemyIA : MonoBehaviour
{
    public Transform target;
    
    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.9f;
    public float jumpModifier = 0.345f;
    public float jumpCheckOffset = 0.1f;
    public float activateDistance = 15f;
    [SerializeField]private bool isGrounded;

    public LayerMask groundLayer;
    [Header("Cutom Behavior")] 
    
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;
    public bool canFly = false;

    [Header("Entity and Attack")] 
    public float attackRange = 1.4f;
    
    
    
    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;


    private Rigidbody2D _rigidbody2D;
    private Seeker seeker;
    void Start()
    {
        seeker = GetComponent<Seeker>();
        _rigidbody2D = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath",0f,0.5f);
    }

    private void UpdatePath()
    {
        if(seeker.IsDone() && followEnabled && TargetInDistance())
            seeker.StartPath(_rigidbody2D.position, target.position, OnPathComplete);
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }
        
        
        /*
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            if(reachedEndOfPath!=false)
                reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - _rigidbody2D.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        _rigidbody2D.AddForce(force);
        
        float distance = Vector2.Distance(_rigidbody2D.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
        */
    }

    private bool TargetInDistance()
    {
        if(_rigidbody2D!=null)
            return Vector2.Distance(_rigidbody2D.position, target.transform.position) < activateDistance;


        return false;
    }

    private void PathFollow()
    {
        if (path == null)
            return;
        if (currentWaypoint >= path.vectorPath.Count)
            return;

        isGrounded = Physics2D.Raycast(transform.position, -Vector3.up,
            GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset, groundLayer);
        
        Debug.DrawRay(transform.position, -Vector3.up * (GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset), Color.magenta);

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - _rigidbody2D.position).normalized;
        Vector2 force = new Vector2(direction.x,0) * speed * Time.deltaTime;

        if (jumpEnabled && isGrounded)
            if (direction.y > jumpNodeHeightRequirement)
                _rigidbody2D.AddForce(Vector2.up * speed * jumpModifier);
        
        
        _rigidbody2D.AddForce(force);

        float distance = Vector2.Distance(_rigidbody2D.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
            currentWaypoint++;

        if (directionLookEnabled)
        {
            if (_rigidbody2D.velocity.x > 0.5f)
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y,
                    transform.localScale.z);
            else if(_rigidbody2D.velocity.x < -0.5f)
                transform.localScale = new Vector3( Mathf.Abs(transform.localScale.x), transform.localScale.y,
                    transform.localScale.z);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        if(TargetInDistance())
            Gizmos.color = Color.red;
        
        Gizmos.DrawWireSphere(transform.position, activateDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
