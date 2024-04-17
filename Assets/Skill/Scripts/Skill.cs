using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
public abstract class Skill : MonoBehaviour
{
     public string SkillName;
     public int SkillCooldown;

     public Vector2 _direction;
     public Vector2 _velocity;
     public virtual void Start()
     {
          Create();
     }

     public virtual void Create()
     { 
          
     }

     public virtual void Execute()
     {
          
     }
} 