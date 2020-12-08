using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public QuaterbackController targetPlayer;
    [Range(0, 5)] public float followStrength = 0.8f;

    // Update is called once per frame
    void Update()
    {
        var newY =  Mathf.Lerp(transform.position.y, targetPlayer.transform.position.y, followStrength * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
