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
    
    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;
    public bool canFly = false;

    [Header("Entity and Attack")] 
    private EnemyStats _stats;
    public float attackRange = 1.4f;
    public LayerMask attackMask;
    public Vector3 attackOffset;
    public float attackCooldown = 1f;

    private float timeSinceLastAttack;
    //public bool canAttack;
    private bool preAttack;
    private bool isAttacking;

    [Tooltip("If is ranged")] public GameObject projectile;
    [Header("Animation")] 
    public Animator animator;
    
    
    #region Gets
    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;


    private Rigidbody2D _rigidbody2D;
    private Seeker seeker;
    #endregion
    
    void Start()
    {
        _stats = this.GetComponent<EnemyEntity>().GetStats();
        seeker = GetComponent<Seeker>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        target = GameManager.GetInstance().GetPlayerTransform();
        if (HasTarget())
        {
            Debug.Log("Found Target");
        }
        InvokeRepeating("UpdatePath",0f,0.5f);

        attackCooldown = _stats.attackCooldown;
        
    }

    public bool HasTarget()
    {
        return target != null;
    }

    private void UpdatePath()
    {
        if (seeker.IsDone() && followEnabled && TargetInDistance() && HasTarget())
        { 
            seeker.StartPath(_rigidbody2D.position, target.position, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void Attack()
    {
        if (!canFly)
            StartCoroutine(MeleeAttack());
        else
            RangedAttack();
        //StartCoroutine(RangedAttack());
        /*
        if (colinfo != null & colinfo.GetComponent<Entity>())
        {
            if (colinfo.GetComponent<Entity>() == GameManager.getInstance().getPlayerEntity())
            {
                Debug.Log("Hit " + colinfo.transform.name);
                colinfo.GetComponent<Entity>().takeDamage(_stats.attackDamage,
                    ElementManager.GetInstance().getElementByEnum(_stats.EntityElement));
            }

        }
        */
    }

    IEnumerator MeleeAttack()
    {
        //animator.SetTrigger("Attack");
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;
        //Collider2D colinfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        // Collider2D colinfo = Physics2D.OverlapCircle(
        //   transform.position + new Vector3(GetLookDirection().x + attackOffset.x, attackOffset.y), attackRange);
        RaycastHit2D hitInfo =
            Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + transform.position.y / 4),
                GetLookDirection(), attackRange,
                attackMask);
        
        if (hitInfo.transform != null && hitInfo.transform.GetComponent<Entity>())
        {
            
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(1f);
            var entityInfo = hitInfo.transform.GetComponent<Entity>();
            if (entityInfo == GameManager.GetInstance().GetPlayerEntity())
            {
                entityInfo.takeDamage(_stats.attackDamage,
                    ElementManager.GetInstance().getElementByEnum(_stats.EntityElement));
            }
        }
    }

    void RangedAttack()
{
    Vector3 direction = (target.position - transform.position).normalized;

    // Ensure the raycast only hits layers specified in attackMask
    RaycastHit2D raycast = Physics2D.Raycast(transform.position, direction, attackRange, attackMask);
    Debug.DrawRay(transform.position, direction * attackRange, Color.magenta, 2f);

    if (raycast.collider != null)
        {
            Debug.Log($"Raycast hit: {raycast.transform.name}");
            
            if (raycast.transform == target)
            {
                Debug.Log("I see the target");

                if (projectile == null)
                    return;

                animator.SetTrigger("Attack");

                // Instantiate the projectile
                var _projectile = Instantiate(projectile, transform.position, Quaternion.identity);

                // Initialize the projectile based on its type
                if (_projectile.GetComponent<CrystalBall_Projectile_Script>())
                {
                    _projectile.GetComponent<CrystalBall_Projectile_Script>().CreateProjectile(
                        transform.position,
                        direction,
                        _stats.attackDamage,
                        ElementManager.GetInstance().getElementByEnum(_stats.EntityElement),
                        this.transform
                    );
                }
                else if (_projectile.GetComponent<Projectile_Script>())
                {
                    _projectile.GetComponent<Projectile_Script>().CreateProjectile(
                        transform.position,
                        direction,
                        _stats.attackDamage,
                        ElementManager.GetInstance().getElementByEnum(_stats.EntityElement),
                        this.transform
                    );
                }
            }
        }
    }


    private void HandleAnimation()
    {
        animator.SetBool("isWalking", _rigidbody2D.velocity.x != 0);
        
        animator.SetBool("PreAttack", TargetInDistanceToAttack());
        
        
        
    }
    void FixedUpdate()
    {
        if (!HasTarget())
        {
            return;
        }
        if (TargetInDistance() && followEnabled && !TargetInDistanceToAttack() && !canFly )
        {
            PathFollow();
        }
         
        if (TargetInDistanceToAttack() && CanAttack())
        {
            timeSinceLastAttack = Time.time;
            Attack();
        }
        
        HandleAnimation();

        if (canFly && !preAttack && !isAttacking)
        {
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
                if (reachedEndOfPath != false)
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
            if (directionLookEnabled)
            {
                if (_rigidbody2D.velocity.x > 0.1f)
                    transform.GetComponentInChildren<SpriteRenderer>().flipX = true;
                    
                else if(_rigidbody2D.velocity.x < -0.1f)
                    transform.GetComponentInChildren<SpriteRenderer>().flipX = false;
            }
            
        }

    }

    private bool CanAttack()
    {  
        
        if (Time.time > timeSinceLastAttack + attackCooldown)
            return true;

        return false;
    }
    private bool TargetInDistance()
    {
        if(_rigidbody2D!=null)
            return Vector2.Distance(_rigidbody2D.position, target.transform.position) < activateDistance;
        
        return false;
    }
    private bool TargetInDistanceToAttack()
    {
        return Vector2.Distance(_rigidbody2D.position, target.transform.position) < attackRange;
        
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
        Vector2 force = new Vector2(direction.x,_rigidbody2D.velocity.y) * speed * Time.deltaTime;

        if (jumpEnabled && isGrounded)
            if (direction.y > jumpNodeHeightRequirement)
                _rigidbody2D.AddForce(Vector2.up * speed * jumpModifier);

        if (!preAttack || !isAttacking)
            _rigidbody2D.velocity = force;
            //_rigidbody2D.AddForce(force);

        float distance = Vector2.Distance(_rigidbody2D.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
            currentWaypoint++;

        if (directionLookEnabled)
        {
            if (_rigidbody2D.velocity.x > 0.1f)
                transform.GetComponentInChildren<SpriteRenderer>().flipX = true;
                    
            else if(_rigidbody2D.velocity.x < -0.1f)
                transform.GetComponentInChildren<SpriteRenderer>().flipX = false;
        }
    }

    public Vector3 lookDirection;
    private Vector3 GetLookDirection()
    { 
        if (_rigidbody2D.velocity.x > 0.1f)
        {
            if(lookDirection!=Vector3.right)
                attackOffset.x *= -1f;
            lookDirection = Vector3.right;
        }
        else if (_rigidbody2D.velocity.x < -0.1f)
        {
            if(lookDirection!=Vector3.left)
                attackOffset.x *= -1f;
            lookDirection = Vector3.left;
        } 
        return lookDirection;
    }

    
    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.white;
        if(TargetInDistance())
            Gizmos.color = Color.red;
        
        Gizmos.DrawWireSphere(transform.position, activateDistance);

        //Gizmos.color = Color.magenta;
        //Gizmos.DrawWireSphere(transform.position, attackRange);

        if (_rigidbody2D != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(
                transform.position + new Vector3(GetLookDirection().x + attackOffset.x, attackOffset.y, 0), attackRange);
        }
    
    }
    
}
