using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity
{
    [Header("Player Entity Settings")]
    public EntityStats playerStats;

    public override void Start()
    {
        setStats(playerStats);
        base.Start();
    }
     
}
