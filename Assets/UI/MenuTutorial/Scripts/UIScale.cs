using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScale : MonoBehaviour
{
    public Vector3 scaleValue;
    public float _timeToScale;
    private Vector3 normalScale;
    void Start()
    {
        //LeanTween.easeInBounce(3, 3, 10);
        InvokeRepeating("Scale",_timeToScale*2,55);
        normalScale = transform.localScale;

    }

    // Update is called once per frame
    void Update()
    {
        //LeanTween.scale(this.transform.gameObject, scaleValue, _timeToScale);
    }

    void Scale()
    {
        StartCoroutine("setScale");
    }

    IEnumerator setScale()
    {
        LeanTween.scale(this.transform.gameObject, scaleValue, _timeToScale);
        yield return new WaitForSeconds(_timeToScale);
        LeanTween.scale(this.transform.gameObject, normalScale, _timeToScale);
        yield return new WaitForSeconds(_timeToScale);
    }
}
