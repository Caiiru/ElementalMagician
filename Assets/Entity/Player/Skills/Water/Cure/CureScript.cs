using System;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class CureScript : Skill_Utility
{
    private ScriptableStats playerStats;
    private PlayerEntity _playerEntity;
    [HideInInspector]
    public float skillcooldownTime;

    [HideInInspector]
    public bool wasCreated = false;
    [HideInInspector]
    public Transform _spawnPoint;
    
    
    public float _duration;
    private bool canHeal;
    public override void Create(Transform spawnPoint, Vector2 direction)
    { 
        transform.position = spawnPoint.position;
        _playerEntity = GameManager.getInstance().GetPlayerEntity();
        SkillName = _stats.SkillName;
        playerStats = GameManager.getInstance().GetPlayerEntity()
            .gameObject.GetComponent<PlayerController>().getStats();
        _spawnPoint = spawnPoint;
        GameManager.getInstance().GetPlayerEntity().gameObject.
            GetComponent<PlayerController>().ChangeMovementSpeedByMultiply(.2f); // Muda pra 20% da velocidade atual
        
        StartCooldown(99f);
        if (_stats.SkillDuration != 0)
            _duration = _stats.SkillDuration;
        else
            Debug.LogError(_stats.name + " has duration 0");

        wasCreated = true;
    }

    public override void Update()
    {
        base.Update();
        Execute();
    }

    public override void Execute()
    {
        base.Execute();
        transform.position = _spawnPoint.position;
        if (Input.GetButton("Fire1") && _duration > 0 )
        {
            if (skillcooldownTime > _stats.TickSkill)
            {
                if (wasCreated)
                {
                    Heal();
                    skillcooldownTime = 0;
                }
            }

            if (skillcooldownTime <= _stats.TickSkill)
                skillcooldownTime += 0.5f * Time.deltaTime;

            _duration -= 1 * Time.deltaTime;
        }
        else
        {
            GameManager.getInstance().GetPlayerEntity().gameObject.GetComponent<PlayerController>().ChangeMovementSpeedByMultiply(5); // Volta ao normal, 100%
            StartCooldown();
            Debug.Log("Stop Cure");
            Destroy(this.gameObject);
            
        }
    }

    public void Heal()
    {
        _playerEntity.Heal(_stats.value, _stats.elementalType);
        Debug.Log("Cure" );
        transform.GetChild(0).GetComponentInChildren<ParticleSystem>().Play();
    }
}
