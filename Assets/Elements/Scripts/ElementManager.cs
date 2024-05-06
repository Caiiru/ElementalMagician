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
        var elementToReturn = _element switch
        {
            EntityElement.AIR => elements[0],
            EntityElement.FIRE => elements[1],
            EntityElement.WATER => elements[2],
            _ => noElement
        };

        return elementToReturn;
    }
    private void Update()
    {
        DealWithCooldown();
    }

    void DealWithCooldown()
    {
        foreach(var recipe in recipes)
        {
            if (recipe._skill.currentCooldown >= 0)
            {
                recipe._skill.currentCooldown -= 1 * Time.deltaTime;

            }
            else
            {
                if (!recipe.canUse)
                {
                    Debug.Log(recipe.name + " can be used");
                    recipe.canUse = true;
                }
            }
        }
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
