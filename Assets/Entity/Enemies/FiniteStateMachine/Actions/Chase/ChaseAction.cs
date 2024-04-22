using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "FSM/Actions/Chase")]
public class ChaseAction : FSMAction
{
    public override void Execute(BaseStateMachine statemachine)
    {
        var navMeshAgent = statemachine.GetComponent<NavMeshAgent>();
        var enemySightSensor = statemachine.GetComponent<EnemySightSensor>();

        navMeshAgent.SetDestination(enemySightSensor.Player.position);
    }
}
