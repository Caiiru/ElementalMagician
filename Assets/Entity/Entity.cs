using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Collider2D))]
public class Entity : MonoBehaviour
{
    private EntityStats stats;
    [SerializeField]private int max_HP;
    [SerializeField] private int current_HP;
    
    #region Components

    private Rigidbody2D _rb;
    private Collider2D _coll;
    #endregion
    #region DebugRegion 
    [Header("Debug")]
    [Space]
    public TextMeshProUGUI debug_Text; 
    public bool debug_takeDamageBool = false;
    public int debug_HowMuchDamage;
    public Element debug_Element;

    #endregion

    public virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _coll = GetComponent<Collider2D>(); 
       
    }

    public virtual void Start()
    {
        max_HP = stats.max_HP;
        current_HP = stats.current_HP;
        if(debug_Text)
            debug_Text.text = current_HP + " / " + max_HP;
    }

    public virtual void takeDamage(int _damage, Element damageType)
    {
        current_HP -= CalculateDamage(_damage, damageType);
        if(debug_Text)
            debug_Text.text = current_HP + " / " + max_HP;
        if (isDead())
        {
            current_HP = 0;
            Destroy(this.gameObject);
        }
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
    

    public void Update()
    { 
        if (debug_takeDamageBool)
        {
            debug_takeDamageBool = false;
            takeDamage(debug_HowMuchDamage,debug_Element);
        }
    }

    public virtual void setStats(EntityStats _stats)
    {
        stats = _stats;
    }

    public int getCurrentHealth()
    {
        return current_HP;
    }
}
