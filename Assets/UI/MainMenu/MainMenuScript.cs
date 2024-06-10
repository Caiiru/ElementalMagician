using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public bool isSameScene;
    public String firstScene;
    private bool start;
    void Start()
    {
        start = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0) ||
            Input.GetMouseButton(1))
        {
            if (isSameScene && !start)
            {
                if (GameManager.GetInstance().state == GameState.PreStart)
                {
               
                    GameManager.GetInstance().ChangeState(GameState.Starting);
                    this.transform.gameObject.SetActive(false);
                }
            }
            else
            { 
                SceneManager.LoadScene("LevelGeneration");
            } 
            start = true;
          
        }
    }
}
