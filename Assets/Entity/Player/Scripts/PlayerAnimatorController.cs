using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using System;
using TarodevController;

public class PlayerAnimatorController : MonoBehaviour
{
    Animator _animator;
    PlayerController _player;
    [SerializeField] private float _playerMovementSpeedX;
    [SerializeField] private bool _debugBoolean;
    
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _player = GameManager.getInstance().playerEntity.transform.GetComponent<PlayerController>();
        //_animator.SetBool(0, true);
    }

    // Update is called once per frame
    void Update()
    {
        MoveAnimation();
    }

    void MoveAnimation()
    {
        _playerMovementSpeedX = _player.playerVelocityX();
        _animator.SetFloat("MoveSpeedX", _playerMovementSpeedX);
        _animator.SetBool("isMoving", _player.isMoving());

        
         
    }
}
