using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillsInput : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject InputElementsDisplay;
    private Element _element;

    [SerializeField] private Element firstElement;
    [SerializeField] private Element secondElement;

    private SpriteRenderer firstElementSprite;
    private SpriteRenderer secondElementSprite;
    
    public event Action<Element,int> elementAdded;
    void Start()
    {
        InputElementsDisplay = transform.GetChild(0).gameObject;
        firstElementSprite = InputElementsDisplay.transform.GetChild(0).transform.GetChild(1)
            .GetComponent<SpriteRenderer>();
        secondElementSprite = InputElementsDisplay.transform.GetChild(1).transform.GetChild(1)
            .GetComponent<SpriteRenderer>(); 
    }

    // Update is called once per frame
    void Update()
    {
        GatherElementInput();
    }

    void GatherElementInput()
    {
        if (Input.GetKeyDown(KeyCode.Z)) // Air
        {
            _element = ElementManager.GetInstance().elements[0];
            addElement(_element);
        }if (Input.GetKeyDown(KeyCode.X)) // Fire
        {
            _element = ElementManager.GetInstance().elements[1];
            addElement(_element);
        }
        if (Input.GetKeyDown(KeyCode.C)) // Water
        {
            
            _element = ElementManager.GetInstance().elements[2];
            addElement(_element);
        }

        
    }

    void addElement(Element _element)
    {
        var position = 0;
        
        if (firstElement == null)
        {
            firstElement = _element;
            position = 1;
            firstElementSprite.color = _element.PlayerDisplayColor;
            elementAdded?.Invoke(_element,position);
            return;
        }
        if(secondElement==null)
        {
            secondElement = _element;
            position = 2;
            secondElementSprite.color = _element.PlayerDisplayColor;
            elementAdded?.Invoke(_element,position);   
            return;
        }
        resetDisplay();


    }

    void resetDisplay()
    {
        firstElement = null;
        secondElement = null;
        firstElementSprite.color = Color.clear;
        secondElementSprite.color = Color.clear;
    }
    
    public interface IPlayerSkillsInput
    {
        public event Action<Element, int> elementAdded;
    }
}
