using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] protected QuaterbackController targetPlayer;
    [SerializeField, Range(0, 5)] protected float followStrength = 0.8f;
    [SerializeField] public float offsetY = 0.5f;
    [SerializeField] public bool follow = true;

    // Update is called once per frame
    void Update()
    {
        var targetY = targetPlayer.transform.position.y + offsetY;
        if( !follow || transform.position.y > targetY ) return;
        
        var newY =  Mathf.Lerp(transform.position.y, targetY, followStrength * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void ShouldFollow( bool value ) {
        follow = value;
    } 
}
