using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "FSM/Actions/Patrol")]
public class PatrolAction : FSMAction
{
    public override void Execute(BaseStateMachine statemachine)
    {
        var navMeshAgent = statemachine.GetComponent<NavMeshAgent>();
        var patrolPoints = statemachine.GetComponent<PatrolPoints>();

        if (patrolPoints.hasReached(navMeshAgent))
            navMeshAgent.SetDestination(patrolPoints.getNext().position);
    }
}
