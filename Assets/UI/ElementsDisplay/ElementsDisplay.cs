using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ElementsDisplay : MonoBehaviour
{
    public List<Element> elements;

    public GameObject test;
    void Start()
    {
        elements = ElementManager.GetInstance().elements; 
        var n = 0;
        foreach (var element in elements)
        {
            var PanelChild = transform.GetChild(n).gameObject.transform.GetChild(0);
            PanelChild.GetComponent<Image>().sprite = elements[n]._sprite;
            n++;
        }

/*  
        foreach (var element in ElementManager.GetInstance().elements)
        {
            elements.Add(element);
            print(element.name + " was added");
        }
        */

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
