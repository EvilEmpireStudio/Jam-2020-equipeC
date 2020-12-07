using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField] protected float maxSpeed = 10f;
    [SerializeField] protected float accelerationTime = 0.3f;
    [SerializeField] protected float fieldWidth = 5f;

    private Vector2 currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = Vector2.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var inputMovement = Input.GetAxis("Horizontal");
        var targetSpeed = Vector2.right * inputMovement * maxSpeed;
        currentSpeed = Vector2.MoveTowards(currentSpeed, targetSpeed, (maxSpeed / accelerationTime) * Time.fixedDeltaTime);
    }

    private void Update() {
        var newPosition = transform.position + new Vector3(currentSpeed.x, 0, currentSpeed.y) * Time.deltaTime;
        if( newPosition.x >= fieldWidth / 2) {
            newPosition.x = fieldWidth / 2;
            currentSpeed.x = 0;
        }
        else if ( newPosition.x <= -fieldWidth / 2 ) {
            newPosition.x = -fieldWidth / 2;
            currentSpeed.x = 0;
        }
        transform.position = newPosition;
    }
    
    private void OnDrawGizmos() {
        #if UNITY_EDITOR
        // if( rigidbody != null )
            Handles.Label(transform.position, 
                "Input movement: " + Input.GetAxis("Horizontal") + "\n" +
                "Velocity: " + currentSpeed.x
            );
        #endif
    }
}
