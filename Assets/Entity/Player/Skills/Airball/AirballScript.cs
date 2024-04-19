using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)),RequireComponent(typeof(Collider2D))]
public class AirballScript : Skill_DamageSkill
{
   private Rigidbody2D _rb;
   private Collider2D _coll; 
   public override void Create(Vector2 startPosition, Vector2 direction)
   {
      base.Create(startPosition, direction);
      _rb = GetComponent<Rigidbody2D>();
      _coll = GetComponent<Collider2D>(); 
        
        
      _direction = direction;
      transform.position = startPosition; 
      _velocity = _direction * _stats.SkillSpeed;
      
      _rb.AddForce(_velocity, ForceMode2D.Impulse);
   }
   public override void Update()
   {
      base.Update();
      Execute();
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.transform.GetComponent<Entity>())
      {
         var otherEntity = other.transform.GetComponent<Entity>();
         otherEntity.takeDamage(_stats.SkillDamage,_stats.damageElement);

         var otherRigidbody = other.transform.GetComponent<Rigidbody2D>();
         
         otherRigidbody.AddForce(_velocity, ForceMode2D.Impulse);
      }
      Destroy(this.gameObject);
   }
}
