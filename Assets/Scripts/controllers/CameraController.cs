using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] protected QuaterbackController targetPlayer;
    [SerializeField, Range(0, 5)] protected float followStrength = 0.8f;
    [SerializeField] public float offsetY = 0.5f;

    private bool follow = false;
    // Update is called once per frame
    void Update()
    {
        if( !follow ) return;
        
        var newY =  Mathf.Lerp(transform.position.y, targetPlayer.transform.position.y + offsetY, followStrength * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void ShouldFollow( bool value ) {
        follow = value;
    } 
}
