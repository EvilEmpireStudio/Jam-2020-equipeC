using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour
{
    public UnityEvent OnHitQuaterback = default;
    public UnityEvent OnHitFrankie = default;
    public UnityEvent OnDefended = default;
    public UnityEvent OnDestroyed = default;
    
    private void OnTriggerEnter2D(Collider2D other) {
        QuaterbackController quaterback;
        if( other.gameObject.TryGetComponent<QuaterbackController>(out quaterback) ) {
            quaterback.HitBy( this );
            OnHitQuaterback?.Invoke();
        }

        FrankieController frankie;
        if( other.gameObject.TryGetComponent<FrankieController>(out frankie) ) {
            frankie.HitBy( this );
            OnHitFrankie?.Invoke();
        }
    }

    public void Destroy() {
        OnDestroyed?.Invoke();
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void ParryObstacle(FrankieController by) {
        OnDefended?.Invoke();
    }
}
