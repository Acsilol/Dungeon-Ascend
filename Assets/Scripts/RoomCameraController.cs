using UnityEngine;

public class RoomCameraController : MonoBehaviour
{
    public Transform player;
    public float transitionSpeed = 5f;
    public float cameraHeight = 15f;
    private Vector3 targetPosition;
    private Quaternion fixedRotation;
    
    void Start()
    {
        // Set the fixed rotation with a 60-degree X rotation
        fixedRotation = Quaternion.Euler(60f, 0f, 0f);

        // Initialize the camera's position based on the player's starting room
        CenterOnStartingRoom();
        
        // Apply the fixed rotation to the camera
        transform.rotation = fixedRotation;
    }

    void Update()
    {
        // Smoothly move the camera towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * transitionSpeed);

        // Ensure the camera rotation stays fixed at 60 degrees on the X-axis
        transform.rotation = fixedRotation;
    }

    public void MoveToRoom(Vector3 roomCenterPosition)
    {
        // Set the target position to the center of the room with a fixed height
        targetPosition = new Vector3(roomCenterPosition.x + 6, cameraHeight, roomCenterPosition.z - 8);
    }

    private void CenterOnStartingRoom()
    {
        // Set target position to the player's starting position to center the camera
        targetPosition = new Vector3(player.position.x + 6, cameraHeight, player.position.z - 8);
        transform.position = targetPosition; // Instantly position the camera on the target
    }
}
