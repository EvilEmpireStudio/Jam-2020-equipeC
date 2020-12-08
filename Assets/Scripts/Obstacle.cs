using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour
{
    public float bumpStrength = 1;
    public UnityEvent OnDefended = default;
    public UnityEvent OnHitQuaterback = default;
    public UnityEvent OnHitFrankie = default;
    
    private void OnTriggerEnter2D(Collider2D other) {
        QuaterbackController quaterback;
        if( other.gameObject.TryGetComponent<QuaterbackController>(out quaterback) ) {
            OnHitQuaterback?.Invoke();
            quaterback.Bump( Vector2.down * bumpStrength );
            
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        FrankieController frankie;
        if( other.gameObject.TryGetComponent<FrankieController>(out frankie) ) {
            OnHitFrankie?.Invoke();
            // frankie.Bump( Vector2.down * bumpStrength );
            
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    public void DestroyObstacle(FrankieController by) {
        if( gameObject.activeSelf ) {
            OnDefended?.Invoke();

            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
