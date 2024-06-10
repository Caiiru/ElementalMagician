using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LifeController : MonoBehaviour
{
    public PlayerEntity _player;
    private IPlayerEntity _playerEntity;
    
    [Header("Health Config")]
    [SerializeField]
    private int howMuchHearts;

    public GameObject HeartGO;

    public List<GameObject> hearts;
    void Start()
    { 
        _entity = GameManager.GetInstance().GetPlayerEntity();
        if (_entity)
        {
            _playerEntity = _entity.transform.GetComponentInParent<IPlayerEntity>();
        } 
        if (_playerEntity != null)
        {
            _playerEntity.playerTakeDamage += UpdateHearts;
        }
        
       
        _player = GameManager.GetInstance().GetPlayerEntity();
        if (_player)
        {  
            howMuchHearts = (GameManager.GetInstance().GetPlayerEntity().playerStats.max_HP)/2;
        }
        CreateHeart(howMuchHearts);
    }

   

    private PlayerEntity _entity;

    private void OnEnable()
    { 
        
        
    }
    private void OnDisable()
    {   
        if(_playerEntity != null)
            _playerEntity.playerTakeDamage -= UpdateHearts;
    }

    private void CreateHeart(int _howMuch)
    {
        for (int i = 0; i < _howMuch; i++)
        {
            var instance = Instantiate(HeartGO, transform);
            hearts.Add(instance);
            instance.transform.localPosition = new Vector2(-875 + (100 * i),480);
            instance.transform.localScale = new Vector2(0.9f, 0.9f);
        }

        UpdateHearts(_howMuch * 2);
    }
 
    private void UpdateHearts(int _life)
    {
        var hpToSprite = _life;
        foreach (var heart in hearts)
        {
            
            var _valueToSet = 0;

            if (hpToSprite - 2 >= 0)
            {
                _valueToSet = 2;
                hpToSprite -= 2;
            }else if (hpToSprite - 1 >= 0)
            {
                _valueToSet = 1;
                hpToSprite -= 1;
            }
            else
            {
                _valueToSet = 0;
            }
            /*
            if (hpToSprite != 0)
            {
                if (hpToSprite % 2 == 0)
                {
                    _valueToSet = 2;
                    hpToSprite -= 2;
                }
                else
                {
                    _valueToSet = hpToSprite % 2;
                    hpToSprite -= 1;
                }

            }
            */ 
            heart.GetComponent<Ui_PlayerHeart>().OnUpdateHeart(_valueToSet);
            
            
            if (hpToSprite < 0)
                hpToSprite = 0;
        }
    }
}
