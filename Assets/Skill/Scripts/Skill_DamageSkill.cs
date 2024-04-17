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
        Create();
    }

    public override void Create()
    {
        SkillName = _stats.SkillName;
        SkillDamage = _stats.SkillDamage;
        SkillCooldown = _stats.SkillCooldown; 
        
    }
}
