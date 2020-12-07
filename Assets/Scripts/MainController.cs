﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField] protected float maxSpeed = 10f;
    [SerializeField, Min(0)] protected float accelerationTime = 0.3f;
    [SerializeField] protected float fieldWidth = 5f;
    [SerializeField] protected float defaultRecoveryTime = 0.25f;

    private Vector2 currentSpeed = Vector2.zero;
    private Vector3 MovementSpeed => new Vector3(currentSpeed.x + speedBump.x, currentSpeed.y + speedBump.y, 0);

    private Vector2 speedBump = Vector2.zero;
    private float recoverySpeed = default;

    // Start is called before the first frame update
    void Start()
    {
        // currentSpeed
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var inputMovement = Input.GetAxis("Horizontal");
        var targetSpeed = Vector2.right * inputMovement * maxSpeed;

        if( Mathf.Sign( currentSpeed.x ) != Mathf.Sign( targetSpeed.x ) )
            currentSpeed.x = 0;


        if( accelerationTime > 0 )
            currentSpeed = Vector2.MoveTowards(currentSpeed, targetSpeed, (maxSpeed / accelerationTime) * Time.fixedDeltaTime);
        else
            currentSpeed = targetSpeed;

        speedBump = Vector2.MoveTowards( speedBump, Vector2.zero, recoverySpeed * Time.deltaTime);
    }

    public void Bump( Vector2 bump ) {
        Bump( bump, defaultRecoveryTime );
    }

    public void Bump( Vector2 bump, float recoveryTime ) {
        speedBump = bump;
        recoverySpeed = speedBump.magnitude / recoveryTime;
    }

    private void Update() {
        var newPosition = transform.position + MovementSpeed * Time.deltaTime;
        newPosition.x = Mathf.Clamp( newPosition.x, -fieldWidth/2, fieldWidth/2);

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

        Gizmos.color = Color.red;
        Gizmos.DrawLine( new Vector3(-fieldWidth/2, 10, -1), new Vector3(-fieldWidth/2, -10, -1) );
        Gizmos.DrawLine( new Vector3(fieldWidth/2, 10, -1), new Vector3(fieldWidth/2, -10, -1) );

        Gizmos.color = Color.blue;
        Gizmos.DrawLine( transform.position, transform.position + MovementSpeed * 0.1f);
    }
}
