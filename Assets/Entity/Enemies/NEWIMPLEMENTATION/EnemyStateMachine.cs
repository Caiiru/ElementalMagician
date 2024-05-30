using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField] private NewBaseState _initialState;
    
    public NewBaseState CurrentState { get; set; }
    private void Awake()
    {
        CurrentState = _initialState;
    }

    private void Update()
    {
        CurrentState.Execute(this);
    }
}
