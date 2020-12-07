using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoachController : MonoBehaviour
{
    protected Quaternion targetRotation = Quaternion.identity;
    
    private void Update() {
        if( Input.GetButtonDown("RotateRight") )
            transform.rotation *= Quaternion.Euler(0, 0, 90);
        
        
        if( Input.GetButtonDown("RotateLeft") )
            transform.rotation *= Quaternion.Euler(0, 0, -90);
    }
}
