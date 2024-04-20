using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterJetScript : Skill_DamageSkill
{
   private bool wasCreated = false;

   [SerializeField]private float skillcooldownTime;

   public override void Start()
   {
      base.Start(); 
   }

   public override void Create(Vector2 startPosition, Vector2 direction)
   {
      base.Create(startPosition, direction);
      wasCreated = true;
      skillcooldownTime = SkillCooldown;
      print("Waterjet cooldown: " + skillcooldownTime);
   }

   public override void Execute()
   {
      base.Execute();

      if (Input.GetKey(KeyCode.F))
      {
         if (skillcooldownTime > SkillCooldown)
         {
            if (wasCreated)
            { 
               skillcooldownTime = 0;
            }

         }
         if(skillcooldownTime <= SkillCooldown)
            skillcooldownTime += 0.5f * Time.deltaTime;
            
 
      }
      else
      {
         Destroy(this.gameObject);
      }
   }

   public override void Update()
   {
      base.Update();
      Execute();
   }
}
