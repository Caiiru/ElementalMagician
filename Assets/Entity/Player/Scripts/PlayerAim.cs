using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] bool isAiming;

    public ScriptableStats _stats;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GatherInput();
        if(isAiming)
         Aim();
    }

    void GatherInput()
    {
        if (Input.GetKey(KeyCode.Z) && !isAiming)
        {
            isAiming = true;
            _stats.canMove = false;
        }

        else if (Input.GetKeyUp(KeyCode.Z) && isAiming)
        {
            isAiming = false;
            _stats.canMove = true;
        }
    }

    private void Aim()
    {
        Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"),0);
        Vector3 targetPosition = (transform.position + inputDirection);
        _stats.aimingDirection = inputDirection;
        Debug.DrawLine(transform.position,targetPosition, Color.red);

        //float newAngle = Mathf.Atan2(targetPosition.y, targetPosition.x) * Mathf.Rad2Deg;
        //var currentAngle = transform.rotation.eulerAngles.z;
        //float angularDistance = newAngle - currentAngle;   
        //transform.rotation = Quaternion.Euler(0,0,newAngle);
        //float angle = Mathf.Atan2()
        //transform.rotation = Quaternion.FromToRotation(Vector3.up, targetPosition - transform.position);
    }
}
