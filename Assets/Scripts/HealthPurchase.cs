using UnityEngine;

public class HealthPurchase : MonoBehaviour
{
    public int coinCost = 30;       // Cost in coins for health purchase
    public int healthGain = 30;     // Amount of health to add
    private PlayerController playerController;
    private bool playerNearby = false; // To track if the player is in the trigger zone

    void Start()
    {
        // Find and reference the PlayerController
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }    
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            playerNearby = true;           
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the object exiting the trigger is the player
        if (other.CompareTag("Player"))
        {
            playerNearby = false;           
        }
    }

    void Update()
    {
        // Check if the player is nearby and the 'E' key is pressed
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            PurchaseHealth();
        }
    }

    private void PurchaseHealth()
    {
        // Check if the player has enough coins and can increase health
        if (playerController != null && playerController.currentCoins >= coinCost)
        {
            if (playerController.currentHealth < PlayerController.maxHealth)
            {
                playerController.currentCoins -= coinCost;     // Deduct coins
                playerController.RestoreHealth(healthGain);    // Restore health
                playerController.AddCoins(0);                  // Update coin UI

            }
        }
    }
}
