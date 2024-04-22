using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/In line of sight")]
public class InLineOfSightDecision : Decision
{
    public override bool Decide(BaseStateMachine machine)
    {
        var enemyInLineOfSight = machine.GetComponent<EnemySightSensor>();
        return enemyInLineOfSight.Ping();
    }
}
