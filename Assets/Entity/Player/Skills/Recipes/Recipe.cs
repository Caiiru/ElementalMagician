using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Recipe")]
public class Recipe : ScriptableObject
{
    public Element firstElement;
    public Element secondElement;

    public SkillStats _skill;

    public GameObject _skillGO;
}
