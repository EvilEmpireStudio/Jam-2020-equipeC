﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeker : MonoBehaviour
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
        var player = GameManager.INSTANCE.Player.transform;
        transform.LookAt(player);

        if( !isRunning ) return;

        currentSpeed = Mathf.MoveTowards(currentSpeed, maxRunningSpeed, Acceleration * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, player.position, currentSpeed * Time.deltaTime);
    }
}
