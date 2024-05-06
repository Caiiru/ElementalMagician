using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Damage/Stats")]
public class Skill_DamageStats:SkillStats
{
    [Header("Damage Settings")]
    public int SkillDamage;
    public int SkillSpeed;
    [Header("Element Settings")] 
    public Element damageElement;

    [Header("Time Settings")]
    public float SkillDuration;
    [Tooltip("Skills like jet's have a necessity to tick by time to do damage, so every skill with duration > 0 need a timetick")]
    public float TimeTick;

}