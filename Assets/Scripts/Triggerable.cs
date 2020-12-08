using UnityEngine;
using UnityEngine.Events;

public class Triggerable : MonoBehaviour
{
    public UnityEvent onTriggered = default;
    
    internal void TriggeredBy(QuaterbackController quaterback)
    {
        onTriggered?.Invoke();
    }
    
    private void OnDrawGizmosSelected() {
        var triggers = FindObjectsOfType<Trigger>();

        Gizmos.color = Color.green;
        foreach (var trigger in triggers)
        {
            if( trigger.triggerables.Contains( this ) )
                Gizmos.DrawLine(trigger.transform.position, transform.position);
        }
    }
}


