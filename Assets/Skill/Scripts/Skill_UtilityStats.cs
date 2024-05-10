using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Utility/Stats")]
public class Skill_UtilityStats : SkillStats
{
    [Header("Time Settings")] 
    public float SkillDuration;
    public float TickSkill; 
    [Header("Value Settings")][Tooltip("If its a skill that have any value ")] 
    public int value;

    [Header("Skill")] public Element elementalType;
}
