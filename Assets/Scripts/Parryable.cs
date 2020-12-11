using UnityEngine;
using UnityEngine.Events;

public class Parryable : MonoBehaviour
{
    public UnityEvent OnParried = default;

    public void ParryObstacle(FrankieController by) {
        var rb = GetComponent<Rigidbody2D>();
        rb.SetRotation( Vector2.SignedAngle(Vector2.up, by.GetComponent<Rigidbody2D>().position - rb.position ) );
        OnParried?.Invoke();
    }
}
