using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    public GameObject[] walls; // 0 - Up, 1 - Down, 2 - Right, 3 - Left
    public GameObject[] doors; // Corresponding doors for each wall direction
    public GameObject[] enemies; // Enemies in this room

    private Cell roomCell; // Reference to the specific cell data for this room
    private bool enemiesDefeated = false;

    // Treasure room properties
    public bool isTreasureRoom = false;
    public GameObject chest; // Reference to the chest in the treasure room
    private bool chestOpened = false;

    // References to neighboring RoomBehaviours in each direction
    public RoomBehaviour upNeighbor;
    public RoomBehaviour downNeighbor;
    public RoomBehaviour rightNeighbor;
    public RoomBehaviour leftNeighbor;

    void Start()
    {
        foreach (GameObject door in doors)
        {
            if (door != null) door.SetActive(false);
        }
        foreach (GameObject wall in walls)
        {
            if (wall != null) wall.SetActive(true);
        }

        // Check if the chest exists in the treasure room and subscribe to its opening event
        if (isTreasureRoom && chest != null)
        {
            ChestInteraction chestScript = chest.GetComponent<ChestInteraction>();
            if (chestScript != null)
            {
                chestScript.OnChestOpened += OnChestOpened;
            }
        }
    }

    // Method to set the specific cell for this room
    public void SetCell(Cell cell)
    {
        roomCell = cell;
    }

    // Method to set neighbor references for two-way connections
    public void SetNeighbors(RoomBehaviour up, RoomBehaviour down, RoomBehaviour right, RoomBehaviour left)
    {
        upNeighbor = up;
        downNeighbor = down;
        rightNeighbor = right;
        leftNeighbor = left;
    }

    // Called when the chest is opened in a treasure room
    private void OnChestOpened()
    {
        chestOpened = true;
        CheckRoomOpenCondition();
    }

    public void CheckEnemies()
    {
        Debug.Log("CheckEnemies called");

        // Assume all enemies are defeated until proven otherwise
        enemiesDefeated = true;

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null && enemy.activeInHierarchy)
            {
                enemiesDefeated = false;
                Debug.Log("An enemy is still active, cannot open room yet.");
                break;
            }
        }

        if (enemiesDefeated)
        {
            CheckRoomOpenCondition();
        }
    }

    private void CheckRoomOpenCondition()
    {
        // If it's a treasure room, ensure the chest is opened. Otherwise, check enemies defeated.
        if (isTreasureRoom && !chestOpened)
        {
            Debug.Log("Chest is not yet opened in the treasure room.");
            return;
        }
        else if (!isTreasureRoom && !enemiesDefeated)
        {
            Debug.Log("Enemies are not yet defeated in this room.");
            return;
        }

        // Open the room if conditions are met
        OpenRoom();
    }

    private void OpenRoom()
    {
        if (roomCell == null)
        {
            Debug.LogError("Room cell data is not set!");
            return;
        }

        for (int i = 0; i < walls.Length; i++)
        {
            if (roomCell.status[i])
            {
                if (walls[i] != null) walls[i].SetActive(false);
                if (doors[i] != null) doors[i].SetActive(true);
                OpenNeighborWall(i);
            }
        }
    }

    private void OpenNeighborWall(int direction)
    {
        switch (direction)
        {
            case 0: if (upNeighbor != null && upNeighbor.walls[1] != null) upNeighbor.walls[1].SetActive(false); break;
            case 1: if (downNeighbor != null && downNeighbor.walls[0] != null) downNeighbor.walls[0].SetActive(false); break;
            case 2: if (rightNeighbor != null && rightNeighbor.walls[3] != null) rightNeighbor.walls[3].SetActive(false); break;
            case 3: if (leftNeighbor != null && leftNeighbor.walls[2] != null) leftNeighbor.walls[2].SetActive(false); break;
        }
    }
}
