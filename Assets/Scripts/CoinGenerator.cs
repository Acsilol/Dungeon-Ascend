using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    public GameObject coinPrefab; // Reference to the coin prefab
    public int minCoins = 10;     // Minimum number of coins to spawn
    public int maxCoins = 15;     // Maximum number of coins to spawn
    public Vector2 spawnAreaSize = new Vector2(5f, 5f); // Size of the spawn area (width x depth)
    public float spawnHeight = 0.5f; // Height at which coins spawn, to avoid sinking below the floor
    public Transform parentTransform; // The parent object to organize coins under (e.g., chest_large)

    void Start()
    {
        GenerateCoins();
    }

    private void GenerateCoins()
    {
        int numCoins = Random.Range(minCoins, maxCoins);

        for (int i = 0; i < numCoins; i++)
        {
            Vector3 spawnPosition = GetRandomPositionInArea();
            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);

            // Set the coin's parent to the specified parentTransform (chest_large)
            if (parentTransform != null)
            {
                coin.transform.SetParent(parentTransform);
            }

            coin.tag = "Coin"; // Ensure the coin is tagged as "Coin" for future removal
        }
    }

    private Vector3 GetRandomPositionInArea()
    {
        float xOffset = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float zOffset = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        return transform.position + new Vector3(xOffset, spawnHeight, zOffset);
    }
}
