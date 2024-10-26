using UnityEngine;
using System;
using System.Collections;

public class ChestInteraction : MonoBehaviour
{
    public Transform chestLid; // Reference to the lid of the chest
    public event Action OnChestOpened; // Event to notify when the chest is opened
    private bool playerNearby = false;
    private bool isOpening = false; // Prevents re-triggering the opening
    private bool isChestOpened = false; // Checks if the chest has been fully opened
    private PlayerController playerController; // Reference to PlayerController

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            // Find the PlayerController script on the player object
            playerController = other.GetComponent<PlayerController>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (!isOpening && !isChestOpened)
            {
                StartCoroutine(OpenChestLid());
            }
            else if (isChestOpened)
            {
                CollectCoins();
            }
        }
    }

    IEnumerator OpenChestLid()
    {
        isOpening = true; // Prevent further interactions during opening

        float duration = 1.0f; // Duration in seconds for the lid to open
        float elapsed = 0f;
        Quaternion initialRotation = chestLid.localRotation;
        Quaternion targetRotation = Quaternion.Euler(-90f, 0f, 0f); // Target rotation on X-axis

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            chestLid.localRotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed / duration);
            yield return null;
        }

        chestLid.localRotation = targetRotation; // Ensure it exactly reaches the target rotation at the end
        isOpening = false;
        isChestOpened = true;

        // Trigger the OnChestOpened event to notify RoomBehaviour
        OnChestOpened?.Invoke();
    }

    private void CollectCoins()
    {
        // Loop through all child objects and destroy those tagged as "Coin"
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Coin"))
            {
                Destroy(child.gameObject);
            }
        }

        // Add collected coins to the player's coin count
        if (playerController != null)
        {
            playerController.AddCoins(20);
        }
    }
}
