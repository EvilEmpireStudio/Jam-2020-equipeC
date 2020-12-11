using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoachController : MonoBehaviour
{
    public QuaterbackController quaterback;
    public Animator coachAnimator;
    public float rotationSpeed = 90f;
    protected Quaternion targetRotation = Quaternion.identity;

    private int direction = 0;
    
    private void Update() {
        transform.position = quaterback.transform.position;
        
        if( Input.GetButtonDown("RotateRight") ) direction ++;
        if( Input.GetButtonUp("RotateLeft") ) direction ++;
            // transform.rotation *= Quaternion.Euler(0, 0, 90);
        
        if( Input.GetButtonDown("RotateLeft") ) direction --;
        if( Input.GetButtonUp("RotateRight") ) direction --;

        coachAnimator.SetInteger("direction", direction);
            
        transform.rotation *= Quaternion.Euler(0, 0, -rotationSpeed * direction * Time.deltaTime);
    }
}
