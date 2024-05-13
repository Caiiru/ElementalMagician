using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Skill : MonoBehaviour
{
     public string SkillName; 

     public Vector2 _direction;
     public Vector2 _velocity;

     [SerializeField]
     private float angle;

     [SerializeField] private Vector2 targetDirection;
     public virtual void Start()
     { 
          
     }

     public virtual void Update()
     { 
     }

     public virtual void Create(Transform spawnPoint, Vector2 direction)
     {
          
          targetDirection = new Vector2((transform.position.x + direction.x) - transform.position.x, (transform.position.y + direction.y) - transform.position.y).normalized;
          angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
          transform.eulerAngles = new Vector3(0, 0, angle);
          if(targetDirection != null)
               Debug.DrawLine(transform.position,targetDirection * 2,Color.red);
          
           

     }

     public virtual void Execute()
     {
          
     }
     
} 