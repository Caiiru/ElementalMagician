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

        public GameObject path;
        public AstarPath pathfind;

        [Header("Presets")] [SerializeField] 
        private GameObject firstPreset;
        public GameObject currentPreset;
        

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerEntity = player.GetComponent<PlayerEntity>();
            path = GameObject.FindGameObjectWithTag("Pathfind");
            pathfind = path.GetComponent<AstarPath>();
            firstPreset = LevelGenerator.instance.GetPresetByNumber(0);
            currentPreset = firstPreset;
            path.transform.position = currentPreset.transform.position;
            pathfind.Scan();
 


        }

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