using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_DamageSkill : Skill
{
    public int SkillDamage;
    public Skill_DamageStats _stats;
 


    public override void Start()
    {
        base.Start(); 
    }

    public override void Create(Vector2 startPosition, Vector2 direction)
    {
        base.Create(startPosition, direction);
    }

    public override void Execute()
    {
        base.Execute();
    }

    public override void Update()
    {
        base.Update();
        Execute();
    }
}
