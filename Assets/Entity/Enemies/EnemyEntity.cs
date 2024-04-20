using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : Entity
{
    [Header("Enemy Entity Settings")]
    public EnemyStats EnemyStats;
    public override void Start()
    {
        setStats(EnemyStats);
        base.Start();
        var spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = EnemyStats.enemySprite;
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        gameObject.GetComponent<BoxCollider2D>().size = spriteSize;
        //gameObject.GetComponent<BoxCollider2D>().offset = new Vector2((spriteSize.x / 2), 0);
    }

    private void OnParticleTrigger()
    {
        throw new NotImplementedException();
    }

    private void OnParticleCollision(GameObject other)
    { 
        var jet = other.GetComponentInParent<WaterJetScript>();
        if (jet.getDamageTick() != 0)
        {
            takeDamage(jet.getDamageTick(),jet._stats.damageElement);
            jet.setCanDamageFalse();
            print("TAKING DAMAGE FROM " + other.transform.name);
        }
    }
}
