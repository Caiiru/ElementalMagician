using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Entity/BaseEntity")]
public class EntityStats : ScriptableObject
{
    [Header("Life")] public int max_HP;
    public int current_HP;
    
    
    [Header("Element Settings")]
    public EntityElement EntityElement;

    public EntityElement elementalWeakness;

    public EntityElement elementalResistance;

}
public enum EntityElement{
    DEFAULT,
    AIR,
    FIRE,
    WATER
}