using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    private bool playerNearby = false;
    private PlayerController playerController;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            playerController = other.GetComponent<PlayerController>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            playerController = null;
        }
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            CollectCoin();
        }
    }

    private void CollectCoin()
    {
        if (playerController != null)
        {
            playerController.AddCoins(5); // Adds 1 coin to the player's count            
            Destroy(gameObject); // Destroy the coin after collection
        }
    }
}
