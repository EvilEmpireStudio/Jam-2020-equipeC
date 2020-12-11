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

    public float speed = 10f;
    [Range(0,1)] public float followStrengh = 0.75f;

    private bool isDown = false;
    public bool IsDown => isDown;

    private void FixedUpdate() {
        if( isDown ) return;

        GetComponent<Rigidbody2D>().velocity = (targetSpot.transform.position - transform.position) * speed + quaterback.MovementSpeed * followStrengh;
    }

    private void Update() {
        if( isDown ) return;

        if( Input.GetButtonDown("Defend") )
            DestroyObstacles();
    }
    
    public void HitBy(Obstacle obstacle) {
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        Down();
    }

    private void Down() {
        animator.Play("death");

        isDown = true;
        if( downParticles != null )
            Instantiate(downParticles, transform.position, transform.rotation).Play();
        quaterback.FrankieDown(this);
    }

    internal void Rise()
    {
        isDown = false;
        animator.Play("run");
    }

    private void DestroyObstacles()
    {
        animator.Play("kick");

        List<Parryable> obstacles = defendZone.colliders
            .FindAll( collider => collider.GetComponent<Parryable>() != null )
            .ConvertAll<Parryable>( collider => collider.GetComponent<Parryable>() );

        foreach( var obstacle in obstacles ) {
            obstacle.ParryObstacle( this ); 
        }
    }
}
