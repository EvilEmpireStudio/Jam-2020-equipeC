using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour
{
    [SerializeField, Min(1)] protected float maxRunningSpeed = 5f;
    [SerializeField, Min(0)] protected float accelerationTime = 0.5f;
    [SerializeField] protected bool isRunning = false;

    protected float Acceleration => maxRunningSpeed / accelerationTime;

    protected float currentSpeed = 0;

    public void SetRunning(bool value) {
        isRunning = value;
    }

    // Update is called once per frame
    void Update()
    {
        if( !isRunning ) return;

        currentSpeed = Mathf.MoveTowards(currentSpeed, maxRunningSpeed, Acceleration * Time.deltaTime);
        transform.position = transform.position + transform.up * currentSpeed * Time.deltaTime;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;

        var arrowSpot = transform.position + transform.up * 3f;

        Gizmos.DrawLine(transform.position, arrowSpot);
        Gizmos.DrawLine(arrowSpot, arrowSpot - transform.up * 0.3f + transform.right * 0.3f);
        Gizmos.DrawLine(arrowSpot, arrowSpot - transform.up * 0.3f - transform.right * 0.3f);
    }
}
