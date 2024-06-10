using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemies; 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    public GameObject getManager()
    {
        return this.gameObject;
    }
    #region Singleton

    public static EnemyManager instance;

    private void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(this);
        }

        instance = this;

    }

    #endregion
}
