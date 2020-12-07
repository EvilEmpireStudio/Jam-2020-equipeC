using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FrankieController : MonoBehaviour
{
    public GameObject targetSpot = default;
    public MainController quaterback = default;
    public ColliderList defendZone = default;

    public float speed = 10f;
    [Range(0,1)] public float followStrengh = 0.75f;

    private void FixedUpdate() {
        GetComponent<Rigidbody2D>().velocity = (targetSpot.transform.position - transform.position) * speed + quaterback.MovementSpeed * followStrengh;
    }

    private void Update() {
        if( Input.GetButtonDown("Defend") )
            DestroyObstacles();
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
