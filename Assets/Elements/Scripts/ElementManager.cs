using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ElementManager : MonoBehaviour
{
    public List<Element> elements;
    public List<Recipe> recipes;
    public Element noElement;
    public Element getElementByEnum(EntityElement _element)
    {
        var elementToReturn = noElement;
        if (_element == EntityElement.AIR) elementToReturn = elements[0];
        else if (_element == EntityElement.FIRE) elementToReturn = elements[1];
        else if (_element == EntityElement.WATER) elementToReturn = elements[2];

        return elementToReturn;
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
