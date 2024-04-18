using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D)),RequireComponent(typeof(Collider2D))]
public class FireballScript : Skill_DamageSkill
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


    public override void Start()
    {
        base.Start();
 
    }

    public override void Update()
    {
        base.Update();
        Execute();
    }

    public override void Execute()
    {
        base.Execute();
        /*
        transform.position = new Vector2(transform.position.x,
            transform.position.y) + _velocity * Time.deltaTime;
        */
        //_rb.velocity += _velocity * Time.fixedDeltaTime;

    }
}
