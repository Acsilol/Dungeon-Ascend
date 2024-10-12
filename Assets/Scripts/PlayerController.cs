using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Weapon equippedWeapon;

    private Rigidbody rb;
    private Vector3 movement;
    private Camera mainCamera;
    private Animator animator;
    private bool isMoving;
    private float lastAttackTime; 
    public float attackDamage = 20f;
    public float attackRange = 2.0f;
    public float attackAngle = 45f;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        mainCamera = Camera.main;  
        animator = gameObject.GetComponent<Animator>(); 

        lastAttackTime = -equippedWeapon.cooldownTime;
    }

    void Update()
    {
        // Capture WASD or arrow key inputs
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Determine movement direction
        movement = new Vector3(moveX, 0f, moveZ).normalized;

        isMoving = movement.magnitude > 0f;        
        animator.SetBool("isMoving", isMoving);  // Set the animator parameter

        // Rotate player to face the mouse
        RotatePlayerToMouse();

        // Check for primary attack (MB1) and if cooldown has passed
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + equippedWeapon.cooldownTime)
        {
            PerformAttack();
            lastAttackTime = Time.time;  // Update the time of the last attack
        }
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

    void PerformAttack()
    {
        // Trigger the animation
        animator.SetTrigger("primaryAttack");     
    }

     public void DealDamageToEnemy()
    {
        // Find all colliders within the attack range
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider enemyCollider in hitEnemies)
        {
            // Check if the collider has the EnemyController component
            EnemyController enemy = enemyCollider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // Calculate the direction from the player to the enemy
                Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;

                // Check if the enemy is within the specified angle in front of the player
                float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);
                if (angleToEnemy <= attackAngle / 2)
                {
                    // Deal damage to the enemy
                    enemy.TakeDamage(0f);
                    Debug.Log("Dealt " + attackDamage + " damage to " + enemy.name);
                }
            }
        }
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        equippedWeapon = newWeapon;
        lastAttackTime = -equippedWeapon.cooldownTime; // Reset cooldown for immediate use
    }
}
