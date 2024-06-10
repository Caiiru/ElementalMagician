using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace System
{
    public partial class GameManager: MonoBehaviour
    {
        public PlayerEntity playerEntity;
        public GameObject player;

        public bool SetPlayerEntity(PlayerEntity _player)
        {
            if (playerEntity != null)
            {
                return false;
            }
            playerEntity = _player; 
            return true;
            
        }
        public PlayerEntity GetPlayerEntity()
        { 
            return playerEntity;
        }

        public Transform GetPlayerTransform()
        {
            return player.transform;
        }
        
        #region Singleton
        private static GameManager instance;

        private void Awake()
        {
            if(instance != null && instance != this)
                Destroy(gameObject);
            else
                instance = this;
            
            DontDestroyOnLoad(this);
        }

        public static GameManager getInstance()
        { 
            return instance;
        }
        
        #endregion
    }
     
}