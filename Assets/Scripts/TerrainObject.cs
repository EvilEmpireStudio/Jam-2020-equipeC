using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TerrainObject : MonoBehaviour
{
    [SerializeField] protected bool destroyOnLeaveTerrain = true;
    [SerializeField] protected UnityEvent onExitTerrain = default;

    public TerrainManager Terrain { get; set; }
    new protected Rigidbody2D rigidbody;

    private void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        rigidbody.velocity = Vector2.down * Terrain.TravellingSpeed;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if( other.gameObject.GetComponent<TerrainManager>() == Terrain ) {
            onExitTerrain?.Invoke();
            if( destroyOnLeaveTerrain ) 
                Destroy(gameObject);
        }
    }
}
