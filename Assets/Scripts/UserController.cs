using UnityEngine;

public class UserController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector3 movement;
    private Camera mainCamera;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        mainCamera = Camera.main;  // Get the main camera
    }

    void Update()
    {
        // Capture WASD or arrow key inputs
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Determine movement direction
        movement = new Vector3(moveX, 0f, moveZ).normalized;

        // Rotate player to face the mouse
        RotatePlayerToMouse();
    }

    void FixedUpdate()
    {
        // Move the player using Rigidbody
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void RotatePlayerToMouse()
    {
        // Get the mouse position in world space
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);  // Assuming the ground is on the XZ plane
        float rayLength;

        if (groundPlane.Raycast(ray, out rayLength))
        {
            Vector3 pointToLook = ray.GetPoint(rayLength);

            // Calculate the direction from the player to the mouse position
            Vector3 direction = (pointToLook - transform.position).normalized;
            direction.y = 0;  // Ensure the player stays on the XZ plane

            // Rotate the player to face the direction of the mouse
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(lookRotation);  // Rotate using Rigidbody to keep physics consistency
        }
    }
}
