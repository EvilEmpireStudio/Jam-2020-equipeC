using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Patroller : MonoBehaviour
{
    [SerializeField] protected Transform patrolTarget;

    private Vector2 targetPosition => new Vector2(patrolTarget.position.x, patrolTarget.position.y); 

    private void Update() {
        GetComponent<Rigidbody2D>().MovePosition(targetPosition);
        // GetComponent<Rigidbody2D>().velocity = targetPosition - GetComponent<Rigidbody2D>().position;
    }
}
