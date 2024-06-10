using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private int number;
    public TextMeshProUGUI _text;
    void Start()
    {
        
        //_text = transform.GetComponentInChildren<TextMeshProUGUI>();
        _text.fontSize = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNumber(int value)
    {
        this.number = value;
        _text.text = value.ToString();
    }
}
