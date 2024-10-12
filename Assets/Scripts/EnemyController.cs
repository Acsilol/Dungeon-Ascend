using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public EnemyData enemyData;  // Reference to ScriptableObject for enemy stats
    protected float currentHealth;
    protected Animator animator; // Reference to the Animator component
    protected bool isMoving;     // Boolean to track if the enemy is moving
    private NavMeshAgent navAgent;

    public virtual void Start()
    {
        // Initialize health from enemy data
        currentHealth = enemyData != null ? enemyData.maxHealth : 100f;

        // Get the Animator component on this GameObject
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();

        // Ensure animator is initialized properly
        if (animator == null)
        {
            Debug.LogWarning("No Animator component found on " + gameObject.name);
        }
    }

    public void Move(Vector3 direction)
    {
        // Check if the enemy is moving based on the direction vector
        isMoving = direction.magnitude > 0f;

        // Update the animator with the isMoving boolean
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }

        // Example of moving the enemy based on the direction
        transform.Translate(direction * Time.deltaTime);
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (animator != null)
        {
            animator.SetTrigger("takeDamage");
            StopMovement();

            // Start coroutine to resume movement after the hit animation duration
            StartCoroutine(ResumeAfterHitAnimation());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator ResumeAfterHitAnimation()
    {
        // Wait for the length of the Hit animation
        float hitAnimationDuration = GetAnimationClipLength("Hit_A"); // Replace "Hit" with the actual animation name

        yield return new WaitForSeconds(hitAnimationDuration);
        ResumeMovement();
    }

    private float GetAnimationClipLength(string clipName)
    {
        if (animator == null) return 0f;

        // Find the animation clip length based on the clip name
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

            // Set a destination to force the NavMeshAgent to move
            if (navAgent.hasPath)
            {
                navAgent.SetDestination(navAgent.destination); // Reaffirm the destination
            }
        }
        isMoving = true;
    }

    protected virtual void Die()
    {
        Debug.Log($"{enemyData.enemyName} defeated!");
        Destroy(gameObject); // Placeholder for death behavior
    }
}
