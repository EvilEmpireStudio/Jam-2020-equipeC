using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public List<Triggerable> triggerables;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if( triggered ) return;

        QuaterbackController quaterback;
        if( other.TryGetComponent<QuaterbackController>(out quaterback) ){
            triggered = true;
            foreach(var triggerable in triggerables)
                triggerable.Triggered();
        }
    }

    private void OnDrawGizmosSelected() {
        if( triggered ) return;
        Gizmos.color = Color.green;

        foreach(var triggerable in triggerables)
            if( triggerable != null )
                Gizmos.DrawLine(transform.position, triggerable.transform.position);
    }
}
