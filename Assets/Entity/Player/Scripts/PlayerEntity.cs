using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerEntity : Entity, IPlayerEntity
{
    [Header("Player Entity Settings")]
    public EntityStats playerStats;
    
   
    public override void Awake()
    {
        if(GameManager.getInstance() != null)
            GameManager.getInstance().SetPlayerEntity(this);
        base.Awake();
        

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
        foreach (var text in popupList)
        {
            if (!text.activeSelf)
            { 
                text.transform.position = new Vector2(transform.position.x, transform.position.y+spawnYOffset);
               
                text.transform.localScale = new Vector3(1, 1, 1);
                text.SetActive(true);
                text.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _damage.ToString();
                text.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().color =
                    damageType.ElementColor;
                
                StartCoroutine(PopupText(_damage,text,_time));
                LeanTween.moveY(text, transform.position.y + spawnYOffset + elevateY, _time);
                var scale = text.transform.localScale;
                text.transform.localScale = new Vector3(0, 0, 0);
                LeanTween.scale(text, scale/2, _time);
                break;
            }
        }
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
