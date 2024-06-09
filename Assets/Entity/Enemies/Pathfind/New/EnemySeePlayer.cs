using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySeePlayer : MonoBehaviour
{
    public float EnemyVisionRange = 8f;
    public float EnemyAttackRange = 2f;
    [Range(0,360)]
    public float fieldOfView = 60f;

    public LayerMask raycastMask;
    public bool isSeeingPlayer = false;

    public Transform player;
    void Start()
    {
        //player = GameManager.getInstance().getPlayerEntity().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (SeePlayer())
        {
            Debug.DrawLine(transform.position, player.position, Color.red);
        }
    }

    public bool SeePlayer()
    {
        Vector3 directionPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(-transform.right, directionPlayer);

        if (angleToPlayer < fieldOfView / 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionPlayer, EnemyVisionRange, raycastMask );
           
            Debug.DrawRay(transform.position, directionPlayer * (EnemyVisionRange), Color.black);
            if (hit.collider != null && hit.collider.transform == player && hit.collider)
            {
                //Debug.Log(hit.collider.transform.name);
                Debug.DrawRay(transform.position, directionPlayer * (EnemyVisionRange / 2), Color.yellow);
                isSeeingPlayer = true;
                return true;
            }
        }

        isSeeingPlayer = false;
        return false;
        
        if ((new Vector2(player.position.x, player.position.y) - getPosition()).sqrMagnitude <
            (EnemyVisionRange * EnemyVisionRange))
        {
            return true;
        }

        return false;
    }

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    private Vector2 getPosition()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, EnemyVisionRange); 
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, EnemyAttackRange); 
        
        Vector3 leftBoundary = Quaternion.Euler(0, 0, -fieldOfView / 2) * -transform.right * EnemyVisionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, fieldOfView / 2) * -transform.right * EnemyVisionRange;
        if (!SeePlayer())
            Gizmos.color = Color.white;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        
    }
}
