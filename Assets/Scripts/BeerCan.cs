using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeerCan : MonoBehaviour
{
    [SerializeField, Range(1, 4)] protected int frankieRaised = 1;
    [SerializeField] protected UnityEvent OnPickup;

    private void OnTriggerEnter2D(Collider2D other) {
        QuaterbackController quaterback;
        if( other.gameObject.TryGetComponent<QuaterbackController>(out quaterback) ) {
            quaterback.RaiseFrankie(frankieRaised);

            OnPickup?.Invoke();
        }
    }

}
