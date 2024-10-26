using UnityEngine;

public class IsometricCameraFollow : MonoBehaviour
{
    public Transform player;         // Reference to the player’s transform
    public Vector3 offset;           // Offset from the player’s position (customize this in Inspector)
    public float smoothSpeed = 0.025f; // Adjust this for how smooth the camera follows (lower is slower)

    private void LateUpdate()
    {
        // Desired position based on player position and offset
        Vector3 desiredPosition = player.position + offset;
        
        // Smoothly interpolate between current camera position and desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;
    }
}
