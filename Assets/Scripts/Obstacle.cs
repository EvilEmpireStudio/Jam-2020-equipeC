using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float bumpStrength = 1;
    
    private void OnTriggerEnter2D(Collider2D other) {
        var quaterback = other.gameObject.GetComponent<MainController>();
        if( quaterback != null ) {
            quaterback.Bump( Vector2.down * bumpStrength );
            
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    public void DestroyObstacle(FrankieController by) {
        if( gameObject.activeSelf ) {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
