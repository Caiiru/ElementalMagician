using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScriptController : MonoBehaviour
{
    public List<GameObject> backgroundToEnable;

    private void OnEnable()
    {
        foreach (var obj in  backgroundToEnable)
        {
            obj.SetActive(true);
        }
    }
}
