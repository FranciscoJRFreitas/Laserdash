using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; 
    public Vector3 offset;

    private void LateUpdate()
    {
        
        transform.position = player.position + offset;

        transform.rotation = Quaternion.Euler(25f, 0f, 0f);
    }
}
