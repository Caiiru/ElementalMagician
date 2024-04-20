using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
public abstract class Skill : MonoBehaviour
{
     public string SkillName;
     public float SkillCooldown;

     public Vector2 _direction;
     public Vector2 _velocity;
     public virtual void Start()
     { 
          
     }

     public virtual void Update()
     { 
     }

     public virtual void Create(Vector2 startPosition, Vector2 direction)
     {
          
        
     }

     public virtual void Execute()
     {
          
     }
} 