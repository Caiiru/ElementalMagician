using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class SkillStats : ScriptableObject
{
    public string SkillName;
    public float cooldown;
    //[HideInInspector]
    public float currentCooldown;
}
