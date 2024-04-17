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
        if (Input.GetKey(KeyCode.S) && !isAiming)
        {
            isAiming = true;
            _stats.canMove = false;
        }

        else if (Input.GetKeyUp(KeyCode.S) && isAiming)
        {
            isAiming = false;
            _stats.canMove = true;
        }
    }

    private void Aim()
    {
        Vector3 aimingDirection = new Vector3(Input.GetAxisRaw("Vertical"),
            Input.GetAxisRaw("Horizontal"),0);
        transform.LookAt(transform.position + aimingDirection);
    }
}
