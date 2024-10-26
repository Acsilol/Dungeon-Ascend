using TMPro;
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
    public float attackDamage = 5f;
    public float attackRange = 2.0f;
    public float attackAngle = 45f;

    public const float maxHealth = 100f;
    public float currentHealth;

    public int currentCoins = 0;

    public TextMeshProUGUI coinCounter;
    public HealthBar healthBar;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();

        // Initialize health and coin count UI
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        UpdateCoinUI();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        movement = new Vector3(moveX, 0f, moveZ).normalized;
        isMoving = movement.magnitude > 0f;
        animator.SetBool("isMoving", isMoving);

        RotatePlayerToMouse();

        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + equippedWeapon.cooldownTime)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void RotatePlayerToMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(ray, out rayLength))
        {
            Vector3 pointToLook = ray.GetPoint(rayLength);
            Vector3 direction = (pointToLook - transform.position).normalized;
            direction.y = 0;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(lookRotation);
        }
    }

    void PerformAttack()
    {
        animator.SetTrigger("primaryAttack");
    }

    public void DealDamageToEnemy()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider enemyCollider in hitEnemies)
        {
            EnemyController enemy = enemyCollider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
                float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);
                if (angleToEnemy <= attackAngle / 2)
                {
                    enemy.TakeDamage(attackDamage);
                }
            }
        }
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        if (coinCounter != null)
        {
            coinCounter.text = currentCoins.ToString();
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void RestoreHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        healthBar.SetHealth(currentHealth);
    }

    private void Die()
    {
        // TODO: HALÁL 
    }
}
