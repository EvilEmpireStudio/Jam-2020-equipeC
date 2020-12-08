using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuaterbackController : MonoBehaviour
{
    [SerializeField] protected float forwardSpeed = 3f;
    [SerializeField] protected float lateralSpeed = 10f;
    [SerializeField, Min(0)] protected float lateralAccelerationTime = 0.3f;
    [SerializeField] protected float defaultChocRecoveryTime = 0.25f;

    private float currentLateralSpeed = 0;
    public Vector3 MovementSpeed => new Vector3(currentLateralSpeed + speedBump.x, forwardSpeed + speedBump.y, 0);

    private Vector2 speedBump = Vector2.zero;
    private float recoverySpeed = default;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var targetLateralSpeed = Input.GetAxis("Horizontal") * lateralSpeed;

        if( Mathf.Sign( currentLateralSpeed ) != Mathf.Sign( targetLateralSpeed ) )
            currentLateralSpeed = 0;


        if( lateralAccelerationTime > 0 )
            currentLateralSpeed = Mathf.MoveTowards(currentLateralSpeed, targetLateralSpeed, (lateralSpeed / lateralAccelerationTime) * Time.fixedDeltaTime);
        else
            currentLateralSpeed = targetLateralSpeed;

        speedBump = Vector2.MoveTowards( speedBump, Vector2.zero, recoverySpeed * Time.deltaTime);
        if( speedBump.magnitude < 0.01 )
            speedBump = Vector2.zero;
    }

    public void Bump( Vector2 bump ) {
        Bump( bump, defaultChocRecoveryTime );
    }

    public void Bump( Vector2 bump, float recoveryTime ) {
        speedBump = bump;
        recoverySpeed = speedBump.magnitude / recoveryTime;
    }

    private bool isDown = false;
    public bool IsDown => isDown;

    public void HitBy(Obstacle obstacle) {
        isDown = true;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GameManager.INSTANCE.Lose();
    }

    private void Update() {
        if( IsDown ) return;
        GetComponent<Rigidbody2D>().velocity = MovementSpeed;
    }
    
    private void OnDrawGizmosSelected() {
        #if UNITY_EDITOR
        
        Handles.Label(transform.position, 
            "Input movement: " + Input.GetAxis("Horizontal") + "\n" +
            "Lateral velocity: " + currentLateralSpeed
        );
        #endif

        Gizmos.color = Color.blue;
        Gizmos.DrawLine( transform.position, transform.position + MovementSpeed * 0.1f);
    }
}
