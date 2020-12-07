using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TerrainObject : MonoBehaviour
{
    // [SerializeField] protected TerrainManager terrain = default;
    [SerializeField] protected UnityEvent onExitTerrain = default;


    private void OnTriggerEnter(Collider other) {
        
    }
}
