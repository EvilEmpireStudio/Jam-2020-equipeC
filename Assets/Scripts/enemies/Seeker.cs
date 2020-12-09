﻿using UnityEngine;

public class Seeker : MonoBehaviour
{
    [SerializeField, Min(1)] protected float maxRunningSpeed = 5f;
    [SerializeField, Min(0)] protected float accelerationTime = 0.5f;
    [SerializeField, Range(0.0001f, 2), InspectorName("Time to do a 180° rotation (in seconds)") ] 
    protected float rotationTime = 0.5f;
    [SerializeField] protected bool isRunning = false;

    protected float Acceleration => maxRunningSpeed / accelerationTime;
    protected float RotationSpeed => 180 / rotationTime;

    protected float currentSpeed = 0;

    public void SetRunning(bool value) {
        isRunning = value;
    }

    // Update is called once per frame
    void Update()
    {
        if( !isRunning ) return;

        var player = GameManager.INSTANCE.Player.transform;
        var curRotation = transform.rotation;
        transform.LookAt(player);
        var targetRotation = transform.rotation;
        transform.rotation = Quaternion.RotateTowards(curRotation, targetRotation, RotationSpeed * Time.deltaTime);

        currentSpeed = Mathf.MoveTowards(currentSpeed, maxRunningSpeed, Acceleration * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, player.position, currentSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = new Color(0.7f, 0.2f, 1, 1);

        var arrowSpot = transform.position + transform.forward * 3f;

        Gizmos.DrawLine(transform.position, arrowSpot);
        Gizmos.DrawLine(arrowSpot, arrowSpot - transform.forward * 0.3f + transform.right * 0.3f);
        Gizmos.DrawLine(arrowSpot, arrowSpot - transform.forward * 0.3f - transform.right * 0.3f);
    }
}
