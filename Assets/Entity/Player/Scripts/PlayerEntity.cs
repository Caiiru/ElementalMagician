using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerEntity : Entity, IPlayerEntity
{
    [Header("Player Entity Settings")]
    public EntityStats playerStats;
    
   
    public override void Awake()
    {
        base.Awake();
        GameManager.getInstance().SetPlayerEntity(this);

    }

    public override void Start()
    {
        setStats(playerStats);
        base.Start();
    }


    public override void takeDamage(int _damage, Element damageType)
    {
        base.takeDamage(_damage, damageType);
        
        playerTakeDamage?.Invoke(getCurrentHealth());
    }

    public override void setStats(EntityStats _stats)
    { 
        base.setStats(_stats);
    }
    public event Action<int> playerTakeDamage;
}
public interface IPlayerEntity
{
    public event Action <int> playerTakeDamage;
}
