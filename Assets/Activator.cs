using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if( other.GetComponent<QuaterbackController>() != null )
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if( other.GetComponent<QuaterbackController>() != null )
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);
        // if( sectionToManage != null && other.GetComponent<QuaterbackController>() != null )
        //     sectionToManage.gameObject.SetActive(false);
    }
}
