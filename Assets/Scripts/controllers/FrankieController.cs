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

    public float speed = 10f;
    [Range(0,1)] public float followStrengh = 0.75f;

    public bool isDown = false;

    private void FixedUpdate() {
        if( isDown ) return;

        GetComponent<Rigidbody2D>().velocity = (targetSpot.transform.position - transform.position) * speed + quaterback.MovementSpeed * followStrengh;
    }

    private void Update() {
        if( isDown ) return;

        if( Input.GetButtonDown("Defend") )
            DestroyObstacles();
    }

    private void Down() {
        isDown = true;
        if( downParticles != null )
            Instantiate(downParticles, transform.position, transform.rotation).Play();
    }

    private void DestroyObstacles()
    {
        List<Obstacle> obstacles = defendZone.colliders
            .FindAll( collider => collider.GetComponent<Obstacle>() != null )
            .ConvertAll<Obstacle>( collider => collider.GetComponent<Obstacle>() );

        foreach( var obstacle in obstacles ) {
            obstacle.DestroyObstacle( this ); 
        }
    }
}
