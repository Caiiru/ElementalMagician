using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="FSM/State")]
public sealed partial class State : BaseState
{
    public List<FSMAction> Action = new List<FSMAction>();
    public List<Transition> Transitions = new List<Transition>();

    public override void Execute(BaseStateMachine machine)
    {
         
    }
}
