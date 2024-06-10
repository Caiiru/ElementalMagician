using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterboltScript : Skill_DamageSkill
{
    private Rigidbody2D _rb;
    private Collider2D _coll; 
    public override void Create(Transform spawnPoint, Vector2 direction)
    {
        base.Create(spawnPoint, direction);
        
        _rb = GetComponent<Rigidbody2D>();
        _coll = GetComponent<Collider2D>(); 
        
        
        _velocity = _direction * _stats.SkillSpeed;
        
        _rb.AddForce(_velocity, ForceMode2D.Impulse); 
    }

    public override void Execute()
    {
        base.Execute();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    { 
        if (other.transform.GetComponent<Entity>())
        {
            if (other.transform.GetComponent<Entity>() != GameManager.getInstance().GetPlayerEntity())
            {
                var otherEntity = other.transform.GetComponent<Entity>();
                otherEntity.takeDamage(_stats.SkillDamage, _stats.damageElement);
                other.transform.GetComponent<Rigidbody2D>()
                    .AddForce(_velocity.normalized * (_stats.SkillDamage), ForceMode2D.Impulse);
            }
        }
        if (other.transform.gameObject != GameManager.getInstance().GetPlayerEntity().transform.gameObject)
        {
            Destroy(this.gameObject);
        }
    }
}
