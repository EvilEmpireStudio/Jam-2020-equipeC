using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public List<Triggerable> triggerables;

    private void OnTriggerEnter2D(Collider2D other) {
        QuaterbackController quaterback;
        if( other.TryGetComponent<QuaterbackController>(out quaterback) ){
            foreach(var triggerable in triggerables)
                triggerable.TriggeredBy(quaterback);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;

        foreach(var triggerable in triggerables)
            Gizmos.DrawLine(transform.position, triggerable.transform.position);
    }
}
