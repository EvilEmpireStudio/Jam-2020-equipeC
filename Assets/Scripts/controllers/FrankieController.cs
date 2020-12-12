using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FrankieController : MonoBehaviour
{
    public GameObject targetSpot = default;
    public QuaterbackController quaterback = default;
    public ColliderList defendZone = default;
    public ParticleSystem downParticles = default;
    public Animator animator = default;
    public float timeRisingInvulnerable = 0.5f;
    public string defendInputName = "DefendUp";

    public AudioSource gruntSource = default;
    public AudioSource kickSource = default;
    public AudioSource deathSource = default;


    public float speed = 10f;
    [Range(0,1)] public float followStrengh = 0.75f;

    private bool isDown = false;
    public bool IsDown => isDown;

    private void FixedUpdate() {
        if( isDown ) return;

        var velocity = (targetSpot.transform.position - transform.position) * speed + quaterback.MovementSpeed * followStrengh;
        GetComponent<Rigidbody2D>().velocity = velocity;
        GetComponent<Rigidbody2D>().SetRotation( Vector2.SignedAngle(Vector2.up, velocity) );
    }

    private void Update() {
        if( isDown ) return;

        if( Input.GetButtonDown(defendInputName) )
            DestroyObstacles();
    }
    
    public void HitBy() {
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        Down();
    }

    private void Down() {
        animator.Play("death");
        deathSource.Play();

        isDown = true;
        if( downParticles != null )
            Instantiate(downParticles, transform.position, transform.rotation).Play();
        quaterback.FrankieDown(this);
    }

    public float risingUntil = 0f;
    internal void Rise()
    {
        isDown = false;
        risingUntil = Time.timeSinceLevelLoad + timeRisingInvulnerable;
        animator.Play("run");
    }

    private void DestroyObstacles()
    {
        animator.Play("kick");
        gruntSource.Play();

        List<Parryable> obstacles = defendZone.colliders
            .FindAll( collider => collider.GetComponent<Parryable>() != null )
            .ConvertAll<Parryable>( collider => collider.GetComponent<Parryable>() );

        var any = false;
        foreach( var obstacle in obstacles ) {
            obstacle.ParryObstacle( this ); 
            any = true;
        }

        if( any )
            kickSource.Play();
    }
}
