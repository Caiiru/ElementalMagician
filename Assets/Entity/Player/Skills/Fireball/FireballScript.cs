using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D)),RequireComponent(typeof(Collider2D))]
public class FireballScript : Skill_DamageSkill
{
    private Rigidbody2D _rb;
    private Collider2D _coll; 
    public override void Create(Transform spawnPoint, Vector2 direction)
    {
        base.Create(spawnPoint, direction);
        
        _rb = GetComponent<Rigidbody2D>();
        _coll = GetComponent<Collider2D>(); 
        
        
        _direction = direction;
        transform.position = spawnPoint.position;

        if (direction.x != 1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        
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
        print("Trigger " + other.name);
        if (other.transform.GetComponent<Entity>())
        {
            var otherEntity = other.transform.GetComponent<Entity>();
            otherEntity.takeDamage(_stats.SkillDamage,_stats.damageElement);
        }
        Destroy(this.gameObject);
    }
}
