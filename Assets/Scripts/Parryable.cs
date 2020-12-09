using UnityEngine;
using UnityEngine.Events;

public class Parryable : MonoBehaviour
{
    public UnityEvent OnParried = default;

    public void ParryObstacle(FrankieController by) {
        OnParried?.Invoke();
    }
}
