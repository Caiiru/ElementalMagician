using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Utility : Skill
{
    public Skill_UtilityStats _stats;
    public override void Create(Transform spawnPoint, Vector2 direction)
    {
        base.Create(spawnPoint, direction);
        SkillName = _stats.SkillName;
        StartCooldown();
    }
    public void StartCooldown()
    {

        _stats.currentCooldown = _stats.cooldown;
    }
    public void StartCooldown(float value)
    {
        _stats.currentCooldown = value;
    }

    public override void Update()
    {
        base.Update(); 
    }
    public override void Execute()
    {
        base.Execute();
    }
}
