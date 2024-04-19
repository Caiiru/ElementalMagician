using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace System
{
    public partial class GameManager: MonoBehaviour
    {
        public PlayerEntity playerEntity; 

        public bool SetPlayerEntity(PlayerEntity _player)
        {
            if (playerEntity != null)
            {
                return false;
            }
            playerEntity = _player; 
            return true;
            
        }
        public PlayerEntity getPlayerEntity()
        { 
            return playerEntity;
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