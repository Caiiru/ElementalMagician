using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemySightSensor : MonoBehaviour
{
    public Transform Player { get; private set; } 
    [FormerlySerializedAs("_ignoreMask")] [SerializeField] private LayerMask _entityMask;
    [SerializeField] private int rayMultiplier;

    private void Awake()
    {
        Player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        if (Ping())
        {
            Debug.Log("Player penis");
        }
    }

    public bool Ping()
    {
        if (Player == null)
            return false;
 

        var _hit = Physics2D.RaycastAll(transform.position, Player.position - this.transform.position, rayMultiplier,
            _entityMask);
        Debug.DrawLine(transform.position, Player.position - this.transform.position);
        foreach (var obj in _hit)
        {
            if (obj.collider.tag == "Player")
                return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {  
    }
}
