using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public int currentPoint = 0;
    public List<Transform> CameraPoints = new List<Transform>();

    private GameObject _mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            CameraPoints.Add(transform.GetChild(i));
        }
        _mainCamera = Camera.main.transform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCameraPoint(int value)
    {
        if (value != currentPoint)
        {
            currentPoint = value;
            _mainCamera.transform.position = new Vector3(CameraPoints[currentPoint].transform.position.x,
                CameraPoints[currentPoint].transform.position.y, -10);
        }
    }
    
    
    #region Singleton

    private void Awake()
    {
        instance = this;
    }

    private static CameraManager instance;

    public static CameraManager GetInstance()
    {
        if (instance == null)
        {
            return null;
        }

        return instance;
    }

    #endregion
}
