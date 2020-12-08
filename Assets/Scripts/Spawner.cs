using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] protected GameObject prefabToSpawn = default;
    [SerializeField] protected float width = 0f;
    [SerializeField, Range(0, 360)] protected float spawnAngle = 0f;

    // protected float nextThrow = 3f;

    // private void FixedUpdate() {
    //     if( Time.timeSinceLevelLoad > nextThrow ) {
    //         SpawnObstacle();
    //         nextThrow = Time.timeSinceLevelLoad + frequency;
    //     }
    // }

    public void SpawnObject() {
        var spawnPosition = transform.position + transform.rotation * Vector3.right * Random.Range(-width/2, width/2);
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.Euler(0, spawnAngle, 0));
    }
    
    private void OnDrawGizmosSelected() {
        var leftPosition = transform.position + transform.rotation * Vector3.right * (-width/2);
        var rightPosition = transform.position + transform.rotation * Vector3.right * (width/2);

        Gizmos.color = Color.red;
        Gizmos.DrawLine( leftPosition, rightPosition );

        var dirVector = Quaternion.Euler(0, spawnAngle, 0) * Vector3.down;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine( leftPosition, leftPosition + dirVector );
        Gizmos.DrawLine( transform.position, transform.position + dirVector );
        Gizmos.DrawLine( rightPosition, rightPosition + dirVector );
    }
}
