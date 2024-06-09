using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class EnemyGFX : MonoBehaviour
{
   public AIPath AIPath;

   private void Update()
   {
      if (AIPath.desiredVelocity.x >= -0.01f)
      {
         transform.localScale = new Vector3(-1f, 1f, 1f);
      }
      else if (AIPath.desiredVelocity.x <= -0.01f)
      {
         
         transform.localScale = new Vector3(1f, 1f, 1f);
      }
   }
}
