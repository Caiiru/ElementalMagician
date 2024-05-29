using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Skill : MonoBehaviour
{
     public string SkillName; 

     public Vector3 _direction;
     public Vector2 _velocity;
 
     private float angle;
 
     public virtual void Start()
     { 
          
     }

     public virtual void Update()
     { 
     }

     public virtual void Create(Transform spawnPoint, Vector2 direction)
     {
          
          transform.position = spawnPoint.position; 
          _direction = (direction - new Vector2(transform.position.x, transform.position.y)).normalized;
          angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90f;
          transform.eulerAngles = new Vector3(0, 0, angle); 

     }

     public virtual void Execute()
     {
          
     }
     
} 