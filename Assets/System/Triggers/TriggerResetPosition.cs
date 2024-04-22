using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerResetPosition : MonoBehaviour
{
    public Transform respawnPosition;

    private Collider2D _collider;
    void Start()
    {
        _collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.transform.position = respawnPosition.position;
        }
    }
}
