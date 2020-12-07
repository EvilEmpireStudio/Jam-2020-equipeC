using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontSpawner : MonoBehaviour
{
    [SerializeField] protected TerrainManager terrain = default;
    [SerializeField] protected TerrainObject obstaclePrefab = default;
    [SerializeField] protected float width = 5;
    [SerializeField] protected float frequency = 1;

    protected float nextThrow = 3f;

    private void FixedUpdate() {
        if( Time.timeSinceLevelLoad > nextThrow ) {
            SpawnObstacle();
            nextThrow = Time.timeSinceLevelLoad + frequency;
        }
    }

    private void SpawnObstacle() {
        var offset = Random.Range(-width/2, width/2);
        Instantiate(obstaclePrefab, transform.position + new Vector3(offset, 0, 0), Quaternion.identity).Terrain = terrain;
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        var offset = new Vector3(width / 2, 0, 0);
        Gizmos.DrawLine( transform.position - offset, transform.position + offset);
    }
}
