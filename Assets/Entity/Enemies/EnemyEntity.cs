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
 
}
