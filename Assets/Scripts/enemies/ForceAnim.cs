using UnityEngine;

public class ForceAnim : MonoBehaviour
{
    // public Animator animator;

    private void Start() {
        var animator = GetComponentInChildren<Animator>();
        animator.SetBool("running", true);
        animator.SetInteger("direction", 1);    
    }
}