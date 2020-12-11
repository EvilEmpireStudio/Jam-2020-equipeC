using UnityEditor;
using UnityEngine;

public class Seeker : MonoBehaviour
{
    [SerializeField, Min(1)] protected float maxRunningSpeed = 5f;
    [SerializeField, Min(0)] protected float accelerationTime = 0.5f;
    [SerializeField, Range(0.0001f, 5), InspectorName("Time to do a 180° rotation (in seconds)") ] 
    protected float rotationTime = 0.5f;
    [SerializeField] protected bool isRunning = false;
    [SerializeField] protected Animator animator = default;
    public AudioSource startRunSource = default;

    public QuaterbackController debug;

    protected float Acceleration => maxRunningSpeed / accelerationTime;
    protected float RotationSpeed => 180 / rotationTime;

    protected float currentSpeed = 0;

    public void SetRunning(bool value) {
        isRunning = value;
        animator.SetBool("running", value);
        if( value ) {
            startRunSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if( !isRunning ) return;

        var rb = GetComponent<Rigidbody2D>();
        var targetPos = GameManager.INSTANCE.Player.GetComponent<Rigidbody2D>().position;
        var delta = Vector2.SignedAngle( targetPos - rb.position, transform.up );
        var maxRotation = RotationSpeed * Time.deltaTime;
        delta = Mathf.Clamp( delta, -maxRotation, maxRotation);
        rb.SetRotation( rb.rotation - delta );

        currentSpeed = Mathf.MoveTowards(currentSpeed, maxRunningSpeed, Acceleration * Time.deltaTime);
        rb.velocity = transform.up * currentSpeed;
    }

    private void OnDrawGizmosSelected() {
        #if UNITY_EDITOR
        if( debug != null ) {
            var rb = GetComponent<Rigidbody2D>();
            var targetPos = debug.GetComponent<Rigidbody2D>().position;
            var delta = Vector2.SignedAngle( targetPos - rb.position, rb.transform.up );
            
            Handles.Label( transform.position, "Delta: " + delta );
        }
        #endif

        Gizmos.color = new Color(0.7f, 0.2f, 1, 1);

        var arrowSpot = transform.position + transform.up * 3f;

        Gizmos.DrawLine(transform.position, arrowSpot);
        Gizmos.DrawLine(arrowSpot, arrowSpot - transform.up * 0.3f + transform.right * 0.3f);
        Gizmos.DrawLine(arrowSpot, arrowSpot - transform.up * 0.3f - transform.right * 0.3f);
    }
}
