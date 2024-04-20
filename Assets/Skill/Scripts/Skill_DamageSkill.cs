using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_DamageSkill : Skill
{ 
    public Skill_DamageStats _stats;
 


    public override void Start()
    {
        base.Start();
       
        
    }

    public override void Create(Vector2 startPosition, Vector2 direction)
    {
        base.Create(startPosition, direction);
        SkillName = _stats.SkillName;
        SkillCooldown = _stats.SkillCooldown;
        
    }

    public override void Execute()
    {
        base.Execute();
    }

    public override void Update()
    {
        base.Update(); 
    }
}
