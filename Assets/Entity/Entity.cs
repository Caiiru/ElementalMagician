using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public EntityStats stats;
    private int max_HP;
    private int current_HP;
    
    public virtual void Start()
    {
        max_HP = stats.max_HP;
        current_HP = stats.current_HP;
    }

    public virtual void takeDamage(int _damage, Element damageType)
    {
        current_HP -= CalculateDamage(_damage,damageType);
        
        if(isDead())
            Destroy(this);
    }

    public virtual bool isDead()
    {
        return current_HP <= 0;
    }

    public virtual void setStatus(EntityStats _stats)
    {
        stats = _stats;
    }

    public virtual int CalculateDamage(int value, Element _type)
    {

        var weaknessInElement = ElementManager.GetInstance().getElementByEnum(stats.elementalWeakness);
        var resistanceInElement = ElementManager.GetInstance().getElementByEnum(stats.elementalResistance);
        var valueToReturn = value;
        
        if (weaknessInElement == _type)
        {
            valueToReturn *= 2;
        }

        if (resistanceInElement == _type)
        {
            valueToReturn = 0;
        }
        
        return valueToReturn;
    }
}
