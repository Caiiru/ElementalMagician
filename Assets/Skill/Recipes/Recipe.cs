using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class Recipe : ScriptableObject
{
    public Element firstElement;
    public Element secondElement;

    public SkillStats _skill;

    public GameObject _skillGO;
}
