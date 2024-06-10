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

        [Header("Presets")] [SerializeField] 
        private GameObject firstPreset;
        public GameObject currentPreset;

        private GameObject levelGenerator;

        [Header("Endgame")] public GameObject gameoverGO;
        public GameObject winGO;
        [Header("GAME")] public GameState state;

        public List<GameObject> enemies;

        private Vector3 playerStartPosition;

        public AstarPath pathfind;
        private void Start()
        {
            ChangeState(GameState.PreStart); 
            
 


        }

        private void CustomCommands()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                player.transform.position = playerStartPosition; 
                player.GetComponent<PlayerController>().getStats().canMove = true;
            }if (Input.GetKeyDown(KeyCode.F2))
            {  
                SceneManager.LoadScene("MainMenu");
            }
        } public void EnemyDie(GameObject enemy)
        {
            int i = 0;
            foreach (var _enemy in enemies)
            { 
                if (_enemy == enemy)
                {
                    enemies.RemoveAt(i);
                }

                i++;
            }             
        }

        public void ChangeState(GameState _state)
        { 
            this.state = _state;
            
            CheckState();
        }

        public void CheckState()
        {
            if (state == GameState.PreStart)
            { 
                firstPreset = LevelGenerator.instance.GetPresetByNumber(0);  
                playerStartPosition = player.transform.position;
                ChangeState(GameState.Starting);
            }

            if (state == GameState.Starting)
            {
                pathfind.Scan();
                if (!pathfind.isScanning)
                { 
                    var _enemies = GameObject.FindGameObjectsWithTag("Entity");
                    foreach (var enemy in _enemies)
                    {
                        if (enemy.GetComponent<EnemyEntity>().getCurrentHealth() > 1)
                        { 
                            enemies.Add(enemy);
                            
                        }
                    }

                    ChangeState(GameState.Game);
                }
                else
                { 
                    Debug.Log("Is Scanning");
                    CheckState();
                }
                
            }

            if (state == GameState.GameOver)
            {
                StartCoroutine(GameOver());
                gameoverGO.SetActive(true); 
            }

            if (state == GameState.Win)
            {
                winGO.SetActive(true); 
                StartCoroutine(GameOver());
            }
        }

        IEnumerator GameOver()
        {
            yield return new WaitForSeconds(5f);
            SceneManager.LoadScene("MainMenu");
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

            if (enemies.Count <= 0 && (state == GameState.Game || state==GameState.Game))
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
            
            //DontDestroyOnLoad(this);

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