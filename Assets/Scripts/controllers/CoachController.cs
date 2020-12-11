using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoachController : MonoBehaviour
{
    public QuaterbackController quaterback;
    protected Quaternion targetRotation = Quaternion.identity;
    
    private void Update() {
        transform.position = quaterback.transform.position;
        
        if( Input.GetButtonDown("RotateRight") )
            transform.rotation *= Quaternion.Euler(0, 0, 90);
        
        
        if( Input.GetButtonDown("RotateLeft") )
            transform.rotation *= Quaternion.Euler(0, 0, -90);
    }
}
