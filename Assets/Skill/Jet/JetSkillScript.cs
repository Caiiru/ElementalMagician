using System;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class JetSkillScript : Skill_DamageSkill
{
    [HideInInspector]
    public bool wasCreated = false;
    [HideInInspector]
    public bool canDamage = false;
    [HideInInspector]
    public ScriptableStats playerStats;
    [HideInInspector]
    public float _duration;
    [HideInInspector]
    public Transform _spawnPoint;
    [HideInInspector]
    public float skillcooldownTime;

    [Header("SFX settings")]
    [SerializeField]
    private ParticleSystem _particleSystem;
    public override void Start()
    { 
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }
    public override void Create(Transform spawnPoint, Vector2 direction)
    {

        base.Create(spawnPoint,direction);
        _direction = direction;
        playerStats = GameManager.getInstance().getPlayerEntity()
           .gameObject.GetComponent<PlayerController>().getStats();

        _spawnPoint = spawnPoint;
        changePositionAndRotation(spawnPoint.position, direction);
        SkillName = _stats.SkillName;
        wasCreated = true;
        skillcooldownTime = _stats.TimeTick;
        _particleSystem.Play();
        GameManager.getInstance().getPlayerEntity().gameObject.GetComponent<PlayerController>().ChangeMovementSpeedByMultiply(.2f); // Muda pra 20% da velocidade atual
        StartCooldown(99f); // set to 99 because was bugging and not work the cooldown
        if (_stats.SkillDuration != 0)
            _duration = _stats.SkillDuration;
        else
            Debug.LogError(_stats.name + " has duration 0");
    }

    public override void Execute()
    {
        base.Execute();
        if (playerStats.canMove)
        {
            playerStats.isAiming = true;
            //playerStats.canMove = false;
        }


        if (Input.GetKey(KeyCode.F) && _duration > 0)
        { 
            changePositionAndRotation(_spawnPoint.position,
                GameManager.getInstance().
                getPlayerEntity().gameObject.GetComponent<PlayerController>()
                .getStats().aimingDirection);

            if (skillcooldownTime > _stats.TimeTick)
            {
                if (wasCreated)
                {
                    canDamage = true;
                    skillcooldownTime = 0;
                }
            }

            if (skillcooldownTime <= _stats.TimeTick)
            {
                skillcooldownTime += 0.5f * Time.deltaTime;

            }
            _duration -= 1 * Time.deltaTime;
            print(_direction);
        }
        else
        {
            playerStats.isAiming = false;
            //playerStats.canMove = true;
            GameManager.getInstance().getPlayerEntity().gameObject.GetComponent<PlayerController>().ChangeMovementSpeedByMultiply(5); // Volta ao normal, 100%
            StartCooldown();
            Destroy(this.gameObject);
        }
    }

    public void setCanDamageFalse()
    {
        canDamage = false;
    }

    public int getDamageTick()
    {
        if (canDamage)
            return _stats.SkillDamage;

        return 0;
    }

    public override void Update()
    {
        base.Update();
        Execute();
    }

    public void changePositionAndRotation(Vector2 position, Vector2 dir)
    {
        transform.position = position;
        if (dir.x == -1)
        {
            this.transform.rotation = Quaternion.Euler(0, dir.x * 180, 0);
        }
        else if (dir.x == 1)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
