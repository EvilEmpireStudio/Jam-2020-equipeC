using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DynamicActivating : MonoBehaviour
{
    public QuaterbackController quaterbackController;
    public float activatingDistance = 200f;
    public float deactivatingDistance = 200f;

    private void FixedUpdate() {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            var preventCulling = 
                !child.gameObject.activeInHierarchy || 
                child.GetComponentsInChildren<PreventCulling>().Any<PreventCulling>( p => p.ShouldPreventCulling(quaterbackController) );

            if( !preventCulling && child.position.y < quaterbackController.transform.position.y - deactivatingDistance ) {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
            else if( child.position.y < quaterbackController.transform.position.y + activatingDistance )
                child.gameObject.SetActive(true);
        }
    }
}
