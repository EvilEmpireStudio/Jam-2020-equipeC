using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] protected GameObject prefabToSpawn = default;
    [SerializeField] protected float width = 0f;
    [SerializeField, Range(0, 360)] protected float spawnAngle = 0f;

    public void SpawnObject() {
        if( prefabToSpawn == null ) return;

        var spawnPosition = transform.position + transform.rotation * Vector3.right * Random.Range(-width/2, width/2);
        var instance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.Euler(0, 0, spawnAngle));
        
        Triggerable triggerable;
        if( instance.TryGetComponent<Triggerable>(out triggerable) )
            triggerable.Triggered();
    }
    
    private void OnDrawGizmosSelected() {
        var leftPosition = transform.position + transform.rotation * Vector3.right * (-width/2);
        var rightPosition = transform.position + transform.rotation * Vector3.right * (width/2);

        Gizmos.color = new Color(0.9f, 0, 1, 1);
        Gizmos.DrawLine( leftPosition, rightPosition );

        var dirVector = Quaternion.Euler(0, 0, spawnAngle) * Vector3.up;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine( leftPosition, leftPosition + dirVector );
        Gizmos.DrawLine( transform.position, transform.position + dirVector );
        Gizmos.DrawLine( rightPosition, rightPosition + dirVector );
    }
}
