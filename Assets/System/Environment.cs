using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public Transform[] PatrolPointsHolder;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    
    
    
    #region Singleton
    private static Environment instance;

    private void Awake()
    {
        if(instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
            
        DontDestroyOnLoad(this);
    }

    public static Environment getInstance()
    { 
        return instance;
    }
    
    #endregion
}


