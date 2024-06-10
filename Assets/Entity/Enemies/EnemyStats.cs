using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/Enemy/BaseEnemyStats")]
public class EnemyStats : EntityStats
{
    [Header("Attack Settings")] 
    public float attackCooldown;
    public int attackDamage;
    [Header("Enemy Settings")] 
    public int JumpForce;
    public Sprite enemySprite;
}

