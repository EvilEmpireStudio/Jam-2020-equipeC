using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [SerializeField] protected float travellingSpeed = 3f;
    public float TravellingSpeed => travellingSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // foreach( var child in this.transform.GetComponentsInChildren<TerrainObject>() ) {
        //     child.transform.position = child.transform.position + new Vector3(0, travellingSpeed * Time.fixedDeltaTime, 0);
        // }
    }
}
