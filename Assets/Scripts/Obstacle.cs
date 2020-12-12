using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour
{
    public UnityEvent OnHitQuaterback = default;
    public UnityEvent OnHitFrankie = default;
    
    private void OnTriggerEnter2D(Collider2D other) {
        QuaterbackController quaterback;
        if( other.gameObject.TryGetComponent<QuaterbackController>(out quaterback) && !quaterback.IsDown ) {
            quaterback.HitBy( this );
            OnHitQuaterback?.Invoke();
        }

        FrankieController frankie;
        if( other.gameObject.TryGetComponent<FrankieController>(out frankie) && !frankie.IsDown && frankie.risingUntil < Time.timeSinceLevelLoad ) {
            frankie.HitBy();
            OnHitFrankie?.Invoke();
        }
    }
}
