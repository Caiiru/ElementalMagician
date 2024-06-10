using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoint_Script : MonoBehaviour
{
    public GameObject currentPreset;
    public GameObject toPreset;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 GetDirection()
    {
        switch (transform.name)
        {
            case "RightDown":
                return Vector3.right;
                break;
            case "LeftDown":
                return Vector3.left;
                break;
            case "Up":
                return Vector3.up;
                break;
            case "Down":
                return Vector3.down;
        }

        return Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        { 
            GameManager.GetInstance().ChangeLevel(GetDirection());
        }
    }
}
