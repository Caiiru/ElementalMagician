using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementManager : MonoBehaviour
{
    public List<Element> elements;
    public List<Recipe> recipes;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    #region Singleton

    private void Awake()
    {
        instance = this;
    }

    private static ElementManager instance;

    public static ElementManager GetInstance()
    {
        if (instance == null)
        {
            instance = new ElementManager();
        }

        return instance;
    }

    #endregion
}
