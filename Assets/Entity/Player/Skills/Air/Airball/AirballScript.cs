using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)),RequireComponent(typeof(Collider2D))]
public class AirballScript : Skill_DamageSkill
{
   private Rigidbody2D _rb;
   private Collider2D _coll; 
   public override void Create(Transform spawnPoint, Vector2 direction)
   {
      base.Create(spawnPoint, direction);
      _rb = GetComponent<Rigidbody2D>();
      _coll = GetComponent<Collider2D>(); 
        
      transform.position = spawnPoint.position; 
      
      _velocity = _direction * _stats.SkillSpeed;
         
      _rb.AddForce(_velocity, ForceMode2D.Impulse);
   }
   
   
   public override void Update()
   {
      base.Update();
      Execute();
      
      _rb.AddForce(_velocity,ForceMode2D.Force);
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.transform.GetComponent<Entity>())
      {
         if (other.transform.GetComponent<Entity>() != GameManager.getInstance().GetPlayerEntity())
         {
            var otherEntity = other.transform.GetComponent<Entity>();
         
            otherEntity.takeDamage(_stats.SkillDamage,_stats.damageElement);

            var otherRigidbody = other.transform.GetComponent<Rigidbody2D>();
         
            otherRigidbody.AddForce(_velocity, ForceMode2D.Impulse);
          
         }
        
      }
      if (other.transform.gameObject != GameManager.getInstance().GetPlayerEntity().transform.gameObject)
      {
         Destroy(this.gameObject);
      }
   }
}
