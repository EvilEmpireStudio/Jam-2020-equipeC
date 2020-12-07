using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MainController : MonoBehaviour
{
    [SerializeField] protected float maxSpeed = 10f;
    [SerializeField] protected float accelerationTime = 0.3f;
    [SerializeField] protected float fieldWidth = 10f;

    private Rigidbody rigidbody;
    private float currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        currentSpeed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
