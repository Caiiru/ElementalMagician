using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.KeypadEnter) || Input.GetMouseButton(0) ||
            Input.GetMouseButton(1))
        {
            if (GameManager.GetInstance().state == GameState.PreStart)
            {
                
                GameManager.GetInstance().ChangeState(GameState.Starting);
                this.transform.gameObject.SetActive(false);
            }
          
        }
    }
}
