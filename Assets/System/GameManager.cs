using System;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        private GameObject levelGenerator;

        [Header("Endgame")] public GameObject gameoverGO;
        public GameObject winGO;
        [Header("GAME")] public GameState state;

        public GameObject[] enemies;

        private Vector3 playerStartPosition;
        private void Start()
        {
            CheckState();
            
 


        }

        private void CustomCommands()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                player.transform.position = playerStartPosition;
            }if (Input.GetKeyDown(KeyCode.F2))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        public void ChangeState(GameState _state)
        {
            Debug.Log("Changing state {state} to {_state}");
            this.state = _state;
            
            CheckState();
        }

        public void CheckState()
        {
            if (state == GameState.PreStart)
            {
                path = GameObject.FindGameObjectWithTag("Pathfind");
                pathfind = path.GetComponent<AstarPath>();
                //firstPreset = LevelGenerator.instance.GetPresetByNumber(0);
                currentPreset = firstPreset;
                path.transform.position = currentPreset.transform.position;
                pathfind.Scan();
                player.GetComponent<PlayerController>().getStats().canMove = false;
                playerStartPosition = player.transform.position;
            }

            if (state == GameState.Starting)
            {
                player.GetComponent<PlayerController>().getStats().canMove = true;
                enemies = GameObject.FindGameObjectsWithTag("Entity");
            }

            if (state == GameState.GameOver)
            {
                gameoverGO.SetActive(true);
            }

            if (state == GameState.Win)
            {
                winGO.SetActive(true);
            }
        }

        public void ChangeLevel(Vector3 direction)
        {
            levelGenerator.transform.position += direction * 38f;
        }

        public void SetLevelGenerator(GameObject _levelGenerator)
        {
            levelGenerator = _levelGenerator;
        }
        private void Update()
        {
            if (Camera.main != null)
            {
                
                //Camera.main.GetComponent<CameraController>().SmoothTransition(player.transform.position,0.5f);
            }
            else
            {
                Debug.Log("No Camera");
            }

            if (enemies.Length <= 0 && (state == GameState.Game || state==GameState.Game))
            {
                ChangeState(GameState.Win);
            }
            CustomCommands();
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

            state = GameState.PreStart;
            player = GameObject.FindGameObjectWithTag("Player");
            playerEntity = player.GetComponent<PlayerEntity>();
        }

        public static GameManager GetInstance()
        { 
            return instance;
        }
        
        #endregion
    }
    
     
}


public enum GameState
{
    PreStart,
    Starting,
    Game,
    GameOver,
    Win
}