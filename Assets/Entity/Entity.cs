using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Collider2D))]
public class Entity : MonoBehaviour
{
    private EntityStats stats;
    [SerializeField]private int max_HP;
    [SerializeField] private int current_HP;
    
    public List<GameObject> popupList = new List<GameObject>();
    #region Components

    private Rigidbody2D _rb;
    private Collider2D _coll;
    #endregion
    #region DebugRegion 
    [Header("Debug")]
    [Space]
    public TextMeshProUGUI debug_Text; 
    public bool debug_takeDamageBool = false;
    public int debug_HowMuchDamage;
    public Element debug_Element;

    #endregion
    
    [Header("UI_Text")]
    
    public float spawnYOffset = 1.5f;
    public float elevateY = 2f;
    public float _time = 0.5f;
    [HideInInspector]
    public GameObject textGameObject; 

    public virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _coll = GetComponent<Collider2D>(); 
       
    }

    public virtual void Start()
    {
        max_HP = stats.max_HP;
        current_HP = stats.current_HP;
        
        #region PopupText
        textGameObject = transform.GetChild(0).gameObject;
        popupList.Add(textGameObject);
        
        textGameObject.transform.gameObject.SetActive(false);
        for (int i = 0; i < 2; i++)
        {
            var textInstance = Instantiate(textGameObject);
            textInstance.transform.gameObject.SetActive(false);
            textInstance.transform.SetParent(this.transform);
            popupList.Add(textInstance);
        }

        #endregion
        
    }

    public virtual void takeDamage(int _damage, Element damageType)
    {
        current_HP -= CalculateDamage(_damage, damageType);
        
        if (isDead())
        {
            current_HP = 0;
            if(this != GameManager.getInstance().GetPlayerEntity())
                Destroy(this.gameObject);
            else
            {
                this.transform.gameObject.SetActive(false);
            }
        }
    }

    public virtual bool isDead()
    {
        return current_HP <= 0;
    }

    public virtual void setStatus(EntityStats _stats)
    {
        stats = _stats;
    }

    public virtual int CalculateDamage(int value, Element _type)
    {
         
        var weaknessInElement = ElementManager.GetInstance().getElementByEnum(stats.elementalWeakness);
        var resistanceInElement = ElementManager.GetInstance().getElementByEnum(stats.elementalResistance);
        var valueToReturn = value;
        
        if (weaknessInElement == _type)
        {
            valueToReturn *= 2;
        }

        if (resistanceInElement == _type)
        {
            valueToReturn /=2;
        } 
        return valueToReturn;
    }

    public virtual void Heal(int value, Element elementType)
    {
        if (current_HP + value > max_HP)
        {
            current_HP = max_HP;
            value = 0;
        }
        else current_HP += value;
        
        foreach (var text in popupList)
        {
            if (!text.activeSelf)
            { 
                text.transform.position = new Vector2(transform.position.x, transform.position.y+spawnYOffset);
                text.SetActive(true);
                text.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();
                text.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().color =
                    elementType.ElementColor;
                
                StartCoroutine(PopupText(value,text,_time));
                LeanTween.moveY(text, transform.position.y + spawnYOffset + elevateY, _time);
                var scale = text.transform.localScale;
                text.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                LeanTween.scale(text, scale/2, _time);
                break;
            }
        }
    }
    

    public void Update()
    { 
        if (debug_takeDamageBool)
        {
            debug_takeDamageBool = false;
            takeDamage(debug_HowMuchDamage,debug_Element);
        }
    }

    public virtual void setStats(EntityStats _stats)
    {
        stats = _stats;
    }

    public int getCurrentHealth()
    {
        return current_HP;
    }
    
    public IEnumerator PopupText(int _value,GameObject obj, float _time)
    {
        yield return new WaitForSeconds(_time);
        obj.SetActive(false);
        obj.transform.localScale = new Vector3(1, 1, 1);

    }
}
