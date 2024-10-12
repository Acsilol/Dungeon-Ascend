using UnityEngine;
using UnityEngine.AI;

public class EnemyFollowAI : MonoBehaviour
{
    public Transform playerTransform;
    private NavMeshAgent navAgent;
    private EnemyController enemyController;

    void Start()
    {
        // Get the NavMeshAgent component on this enemy
        navAgent = GetComponent<NavMeshAgent>();
        enemyController = GetComponent<EnemyController>();

        // Set the NavMeshAgent's speed to match the moveSpeed from EnemyData
        if (enemyController != null && enemyController.enemyData != null)
        {
            navAgent.speed = enemyController.enemyData.moveSpeed;
        }

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning("Player not found! Make sure the player has the 'Player' tag.");
            }
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            // Set the agent's destination to the player's position
            navAgent.SetDestination(playerTransform.position);

            // Set the isMoving variable in EnemyController based on NavMeshAgent's velocity
            bool isMoving = navAgent.velocity.magnitude > 0.1f;
            enemyController.Move(navAgent.velocity.normalized);

            // Optional: You could also add logic to check the distance and start an attack if close enough
            if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                // Play attack animation or trigger an attack here
            }
        }
    }
}
