using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHolder : MonoBehaviour
{
    [SerializeField] private Skill _skillStats;

    private void Start()
    {
        _skillStats.Create();
    }

    public void setSkill(Skill _skill)
    {
        _skillStats = _skill;
    }

}
