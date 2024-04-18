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

}