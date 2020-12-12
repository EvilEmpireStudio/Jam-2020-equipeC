using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventCulling : MonoBehaviour
{
    public float distanceMax = 20f;

    internal bool ShouldPreventCulling(QuaterbackController quaterbackController)
    {
        return Vector3.Distance(transform.position, quaterbackController.transform.position) <= distanceMax;
    }
}
