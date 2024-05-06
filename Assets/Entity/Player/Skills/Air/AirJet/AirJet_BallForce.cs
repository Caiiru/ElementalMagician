using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AirJet_BallForce : MonoBehaviour
{
    private Collider2D _collider2D;
    private Rigidbody2D _rigidbody2D;
    public float airForce; 
    void Start()
    {
        _collider2D = GetComponent<Collider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.GetComponent<Entity>())
        {
            print("AIRFORCE COLLISION");
            other.transform.GetComponent<Rigidbody2D>().AddForce(_rigidbody2D.velocity.normalized * airForce, ForceMode2D.Impulse);
        }
    }

    public void setForce(float value)
    {
        airForce = value;
    }
}
