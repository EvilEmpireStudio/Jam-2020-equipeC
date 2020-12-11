using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : MonoBehaviour
{
    public float randomRange = 0.1f;

    private void Start() {
        GetComponent<Animator>().speed = Random.Range(1 - randomRange, 1 + randomRange);
    }
}
