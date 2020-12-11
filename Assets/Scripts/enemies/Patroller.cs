using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Patroller : MonoBehaviour
{
    [SerializeField] protected Transform patrolTarget;
    [SerializeField] protected Animator animator;

    private Vector2 targetPosition => new Vector2(patrolTarget.position.x, patrolTarget.position.y); 
    
    private void Start() {
        animator.SetBool("running", true);
    }

    private void Update() {
        // GetComponent<Rigidbody2D>().MovePosition(targetPosition);
        var angle = Vector2.SignedAngle(Vector2.up, targetPosition - GetComponent<Rigidbody2D>().position);
        // var velocity = () / Time.deltaTime;
        GetComponent<Rigidbody2D>().MovePosition(targetPosition);
        GetComponent<Rigidbody2D>().SetRotation(angle);
    }
}
