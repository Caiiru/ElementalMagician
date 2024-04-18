using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : Entity
{ 
    public override void Start()
    {
        base.Start(); 
    }

    public override void takeDamage(int _damage, Element damageType)
    {
        base.takeDamage(_damage, damageType); 
    }

    public override int CalculateDamage(int value, Element _type)
    {
        return base.CalculateDamage(value, _type);
    }
}
