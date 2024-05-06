using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyEntity : Entity
{
    [Header("Enemy Entity Settings")]
    public EnemyStats EnemyStats;

    [Header("UI_Text")] 
    private GameObject damageText;
    private List<GameObject> damageListText = new List<GameObject>();
    [SerializeField] private int spawnYOffset;
    [SerializeField] private int elevateY;
    [SerializeField] float _time = 0.5f;
    public override void Start()
    {
        setStats(EnemyStats);
        base.Start();
        damageText = transform.GetChild(0).gameObject;
        damageListText.Add(damageText);
        
        damageText.transform.gameObject.SetActive(false);
            
            
            
        var spriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = EnemyStats.enemySprite;
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        gameObject.GetComponent<BoxCollider2D>().size = spriteSize;
        //gameObject.GetComponent<BoxCollider2D>().offset = new Vector2((spriteSize.x / 2), 0);

        for (int i = 0; i < 2; i++)
        {
            var textInstance = Instantiate(damageText);
            textInstance.transform.gameObject.SetActive(false);
            textInstance.transform.SetParent(this.transform);
            damageListText.Add(textInstance);
        }
    }

    private void OnParticleTrigger()
    {
        throw new NotImplementedException();
    }

    public override void takeDamage(int _damage, Element damageType)
    {
        base.takeDamage(_damage, damageType);

        foreach (var text in damageListText)
        {
            if (!text.activeSelf)
            { 
                text.transform.position = new Vector2(transform.position.x, transform.position.y+spawnYOffset);
                text.SetActive(true);
                text.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _damage.ToString();
                text.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().color =
                    damageType.ElementColor;
                
                StartCoroutine(PopupTextDamage(_damage,text,_time));
                LeanTween.moveY(text, transform.position.y + spawnYOffset + elevateY, _time);
                var scale = text.transform.localScale;
                text.transform.localScale = new Vector3(0, 0, 0);
                LeanTween.scale(text, scale/2, _time);
                break;
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    { 
        var jet = other.GetComponentInParent<JetSkillScript>();
        if (jet.getDamageTick() != 0)
        {
            takeDamage(jet.getDamageTick(),jet._stats.damageElement);
            jet.setCanDamageFalse();
            print("TAKING DAMAGE FROM " + other.transform.name);
        }
    }

    IEnumerator PopupTextDamage(int _damage,GameObject obj, float _time)
    {
        yield return new WaitForSeconds(_time);
        obj.SetActive(false);
        obj.transform.localScale = new Vector3(1, 1, 1);

    }
}
