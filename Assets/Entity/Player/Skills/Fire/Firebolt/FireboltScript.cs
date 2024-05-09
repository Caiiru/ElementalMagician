using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireboltScript : Skill_DamageSkill
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
        print("Trigger " + other.name);
        if (other.transform.GetComponent<Entity>())
        {
            var otherEntity = other.transform.GetComponent<Entity>();
            otherEntity.takeDamage(_stats.SkillDamage,_stats.damageElement);
		    other.transform.GetComponent<Rigidbody2D>().AddForce(_velocity.normalized * (_stats.SkillDamage), ForceMode2D.Impulse);
        }
        Destroy(this.gameObject);
    }
}
