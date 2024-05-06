using System;
using TarodevController;
using UnityEngine;
using TarodevController;

/*    TO DO LATER:
      Implementar o collider se movendo ate o final da particula.
      Detectar a colisão do collider e empurrar a entidade que ele atingir

*/
public class AirJetScript : JetSkillScript
{
    [SerializeField] private GameObject colliderDetecter;
    public override void Start()
    {
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }
    public override void Create(Transform spawnPoint, Vector2 direction)
    {
        base.Create(spawnPoint, direction);
    }
    public override void Execute()
    {
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
                    EmitCollider();

                    skillcooldownTime = 0;
                }
            }

            if (skillcooldownTime <= _stats.TimeTick)
            {
                skillcooldownTime += 0.5f * Time.deltaTime;

            }
            _duration -= 1 * Time.deltaTime;
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
    public void EmitCollider()
    {
        colliderDetecter.transform.position = transform.position;
        colliderDetecter.GetComponent<Rigidbody2D>().AddForce(GameManager.getInstance().
                getPlayerEntity().gameObject.GetComponent<PlayerController>()
                .getStats().aimingDirection, ForceMode2D.Impulse);
    }
} 


