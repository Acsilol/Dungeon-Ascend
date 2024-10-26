using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private RoomCameraController cameraController;
    public GameObject[] enemiesInRoom; // Array to hold references to the enemies in this room
    private bool playerInside = false;

    void Start()
    {
        // Find the Main Camera and get the RoomCameraController component
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cameraController = mainCamera.GetComponent<RoomCameraController>();
        }

        // Ensure all enemies are initially disabled
        foreach (GameObject enemy in enemiesInRoom)
        {
            if (enemy != null)
            {
                enemy.SetActive(false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && cameraController != null && !playerInside)
        {
            playerInside = true;
            cameraController.MoveToRoom(transform.position);
            ActivateEnemies();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerInside)
        {
            playerInside = false;
            DeactivateEnemies();
        }
    }

    private void ActivateEnemies()
    {      
        foreach (GameObject enemy in enemiesInRoom)
        {
            if (enemy != null)
            {
                enemy.SetActive(true);
            }
        }
    }

    private void DeactivateEnemies()
    {
        foreach (GameObject enemy in enemiesInRoom)
        {
            if (enemy != null)
            {
                enemy.SetActive(false);
            }
        }
    }
}
