using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public EnemyData enemyData;
    protected float currentHealth;
    protected Animator animator;
    protected bool isMoving;
    private NavMeshAgent navAgent;
    public float attackDamage = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2.0f;
    private float lastAttackTime;
    public HealthBar healthBar;
    public RoomBehaviour roomBehaviour; // Reference to the room this enemy belongs to

    [SerializeField] private GameObject coinPrefab; // Reference to the coin prefab


    public virtual void Start()
    {
        currentHealth = enemyData != null ? enemyData.maxHealth : 100f;
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();        

        healthBar.SetMaxHealth(enemyData.maxHealth);
    }

    public void Move(Vector3 direction)
    {
        isMoving = direction.magnitude > 0f;

        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }

        transform.Translate(direction * Time.deltaTime);
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        healthBar.SetHealth(currentHealth);

        if (animator != null)
        {
            animator.SetTrigger("takeDamage");
            StopMovement();
            StartCoroutine(ResumeAfterHitAnimation());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator ResumeAfterHitAnimation()
    {
        float hitAnimationDuration = GetAnimationClipLength("Hit_A");
        yield return new WaitForSeconds(hitAnimationDuration);
        ResumeMovement();
    }

    private float GetAnimationClipLength(string clipName)
    {
        if (animator == null) return 0f;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }
        return 0f;
    }

    public void StopMovement()
    {
        if (navAgent != null)
        {
            navAgent.isStopped = true;
        }
        isMoving = false;
    }

    public void ResumeMovement()
    {
        if (navAgent != null)
        {
            navAgent.isStopped = false;

            if (navAgent.hasPath)
            {
                navAgent.SetDestination(navAgent.destination);
            }
        }
        isMoving = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown)
        {
            AttackPlayer(other.GetComponent<PlayerController>());
            lastAttackTime = Time.time;
        }
    }

    private void AttackPlayer(PlayerController player)
    {
        if (player != null)
        {
            animator.SetTrigger("attack");
            player.TakeDamage(attackDamage);            
        }
    }

    private void Die()
    {
        gameObject.SetActive(false); // Deactivate enemy

        // Instantiate the coin at the enemy's death position
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        // Notify the room that this enemy is defeated
        if (roomBehaviour != null)
        {
            roomBehaviour.CheckEnemies();
        }
    }


}
