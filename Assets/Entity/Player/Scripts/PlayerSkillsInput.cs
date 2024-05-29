using System;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerSkillsInput : MonoBehaviour
{ 
    private Element _element;
    [SerializeField] private List<Recipe> recipes;
 
    public Transform _skillSpawnPosition;
    
    #region Display
    private GameObject InputElementsDisplay;
    private Element firstElement;
    private Element secondElement;

    private SpriteRenderer firstElementSprite;
    private SpriteRenderer secondElementSprite;

    private GameObject fireSprite, waterSprite, airSprite;
    private Vector3 baseScale;

    private ScriptableStats _playerStats;

    public float scaleMultiplier = 1.2f;
    #endregion

    private Vector2 aimingDirection; 
    public event Action<Element,int> elementAdded;
    void Start()
    {
        InputElementsDisplay = transform.GetChild(0).gameObject;
        /*
        firstElementSprite = InputElementsDisplay.transform.GetChild(0).transform.GetChild(1)
            .GetComponent<SpriteRenderer>();
        secondElementSprite = InputElementsDisplay.transform.GetChild(1).transform.GetChild(1)
            .GetComponent<SpriteRenderer>();
        */
        fireSprite = InputElementsDisplay.transform.GetChild(0).transform.gameObject;
        waterSprite = InputElementsDisplay.transform.GetChild(1).transform.gameObject;
        airSprite = InputElementsDisplay.transform.GetChild(2).transform.gameObject;
        
        recipes = ElementManager.GetInstance().recipes;


        baseScale = fireSprite.transform.localScale;

        _playerStats = GameManager.getInstance().getPlayerEntity().transform.GetComponent<PlayerController>()
            .getStats();
        
        resetDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        GatherElementInput();
        aimingDirection = new Vector2(GetMousePosition().x, GetMousePosition().y);
        /*
        Debug.DrawLine(_skillSpawnPosition.position,
            aimingDirection,
            Color.red);
            */

    }

    Vector3 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void GatherElementInput()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Air
        {
            _element = ElementManager.GetInstance().elements[0];
            addElement(_element);
        }if (Input.GetKeyDown(KeyCode.E)) // Fire
        {
            _element = ElementManager.GetInstance().elements[1];
            addElement(_element);
        }
        if (Input.GetKeyDown(KeyCode.R)) // Water
        {
            
            _element = ElementManager.GetInstance().elements[2];
            addElement(_element);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            CheckRecipes();
        }


    }

    void addElement(Element _element)
    {
        var position = 0; 
        
        if (firstElement == null)
        {
            firstElement = _element;
            switch (firstElement.ElementName)
            {
                case "Fire":
                    fireSprite.transform.GetChild(1).transform.GetComponent<SpriteRenderer>().color = Color.white;
                    fireSprite.transform.localScale = fireSprite.transform.localScale * scaleMultiplier;
                    break;
                
                case "Water":
                    waterSprite.transform.GetChild(1).transform.GetComponent<SpriteRenderer>().color = Color.white;
                    waterSprite.transform.localScale = waterSprite.transform.localScale * scaleMultiplier;
                    break;
                
                case "Air":
                    airSprite.transform.GetChild(1).transform.GetComponent<SpriteRenderer>().color = Color.white;
                    airSprite.transform.localScale = airSprite.transform.localScale * scaleMultiplier;
                    break;
                default:
                    break;
            }
            position = 1; 
            elementAdded?.Invoke(_element,position);
            return;
        }
        if(secondElement==null)
        {
            secondElement = _element;
            position = 2;
            switch (secondElement.ElementName)
            {
                case "Fire":
                    fireSprite.transform.GetChild(1).transform.GetComponent<SpriteRenderer>().color = Color.white;
                    fireSprite.transform.localScale = fireSprite.transform.localScale * scaleMultiplier;
                    break;
                
                case "Water":
                    waterSprite.transform.GetChild(1).transform.GetComponent<SpriteRenderer>().color = Color.white;
                    waterSprite.transform.localScale = waterSprite.transform.localScale * scaleMultiplier;
                    break;
                
                case "Air":
                    airSprite.transform.GetChild(1).transform.GetComponent<SpriteRenderer>().color = Color.white;
                    airSprite.transform.localScale = airSprite.transform.localScale * scaleMultiplier;
                    break;
                default:
                    break;
            }
            
            elementAdded?.Invoke(_element,position);   
            return;
        }
        resetDisplay();


    }

    void resetDisplay()
    {
        firstElement = null;
        secondElement = null;
        //firstElementSprite.color = Color.clear;
        //secondElementSprite.color = Color.clear;
        fireSprite.transform.GetChild(1).transform.GetComponent<SpriteRenderer>().color = Color.grey;
        waterSprite.transform.GetChild(1).transform.GetComponent<SpriteRenderer>().color = Color.gray;
        airSprite.transform.GetChild(1).transform.GetComponent<SpriteRenderer>().color = Color.gray;
        fireSprite.transform.localScale = baseScale;
        airSprite.transform.localScale = baseScale;
        waterSprite.transform.localScale = baseScale;

    }

    void CheckRecipes()
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            if (firstElement == recipes[i].firstElement && secondElement == recipes[i].secondElement)
            {
                if (recipes[i].canUse)
                {
                    recipes[i].canUse = false;

                    var _skill = Instantiate(recipes[i]._skillGO);
                    //var aimingDirection = transform.GetComponentInParent<PlayerController>().getStats().aimingDirection;
                    
                    //_playerStats.aimingDirection = GetMousePosition();
                    _skill.GetComponent<Skill>().Create(_skillSpawnPosition, aimingDirection);
                    Debug.DrawLine(_skillSpawnPosition.position, aimingDirection, Color.green);
                    //Debug.Break();
                    //EditorApplication.isPaused = true;
                    break;
                }
                
            } 
        }
        resetDisplay();
    } 

    
    
    public interface IPlayerSkillsInput
    {
        public event Action<Element, int> elementAdded;
    }
}
