using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCameraMove : MonoBehaviour
{
    private CameraManager _cameraManager;
    public int cameraNumber;
    public Transform playerPosition;
    private float _cooldown = 0.5f;
    private bool cameraWasMoved;

    public List<GameObject> objectToActive = new List<GameObject>();
    public List<GameObject> objectsToDesactive = new List<GameObject>();
    void Start()
    {
        _cameraManager = CameraManager.GetInstance();
    }

    private void Update()
    {
        if (cameraWasMoved)
        {
            var _timer = _cooldown;
            _timer -= Time.deltaTime;
            if (_timer < 0)
            {
                cameraWasMoved = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player") && !cameraWasMoved)
        {
            _cameraManager.ChangeCameraPoint(cameraNumber);
            if (playerPosition != null)
            {
                other.transform.position = playerPosition.position;
            }

            if (objectToActive.Count != 0)
            {
                foreach (var _object in objectToActive)
                {
                    _object.transform.gameObject.SetActive(true);
                }
            }

            if (objectsToDesactive.Count != 0)
            {
                foreach (var _object in objectsToDesactive)
                {
                    _object.transform.gameObject.SetActive(false);
                }
            }

            cameraWasMoved = true;
        }
    }
}
