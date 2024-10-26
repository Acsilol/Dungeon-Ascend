using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;
        public bool obligatory;

        // Fields for enemy generation
        public GameObject[] enemyPrefabs; // Array of enemy prefabs for this room type
        public int minEnemies = 0; // Minimum number of enemies in this room
        public int maxEnemies = 3; // Maximum number of enemies in this room

        public int ProbabilityOfSpawning(int x, int y)
        {
            // 0 - cannot spawn, 1 - can spawn, 2 - HAS to spawn
            if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
            {
                return obligatory ? 2 : 1;
            }
            return 0;
        }
    }

    public Vector2Int size;
    public Rule[] rooms;
    public Vector2 offset;
    public int numberOfDeadEnds = 5;
    public GameObject playerPrefab;  // Reference to the Player prefab

    List<Cell> board;

    void Start()
    {
        board = new List<Cell>();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }
        GenerateMainPath();
        AddDeadEnds();
        GenerateDungeon();
        SpawnPlayer(); // Spawn the player in the first room
    }

    void GenerateMainPath()
    {
        int x = 0;
        int y = 0;
        int endX = size.x - 1;
        int endY = size.y - 1;
        int currentCell = x + y * size.x;

        board[currentCell].visited = true;
        board[currentCell].isMainPath = true;

        while (x < endX || y < endY)
        {
            List<int> possibleMoves = new List<int>();
            if (x < endX) possibleMoves.Add(1); // Right
            if (y < endY) possibleMoves.Add(0); // Down

            int moveDirection = possibleMoves[Random.Range(0, possibleMoves.Count)];

            if (moveDirection == 1)
            {
                int newCell = currentCell + 1;
                ConnectRooms(currentCell, newCell);
                currentCell = newCell;
                x++;
            }
            else if (moveDirection == 0)
            {
                int newCell = currentCell + size.x;
                ConnectRooms(currentCell, newCell);
                currentCell = newCell;
                y++;
            }

            board[currentCell].visited = true;
            board[currentCell].isMainPath = true;
        }
    }

    void AddDeadEnds()
    {
        List<int> mainPathCells = new List<int>();
        for (int i = 0; i < board.Count; i++)
        {
            if (board[i].isMainPath)
            {
                mainPathCells.Add(i);
            }
        }

        for (int i = 0; i < numberOfDeadEnds; i++)
        {
            int randomMainCell = mainPathCells[Random.Range(1, mainPathCells.Count - 1)];

            List<int> possibleDirections = new List<int> { 0, 1, 2, 3 };
            int newCell = -1;

            while (possibleDirections.Count > 0 && newCell == -1)
            {
                int randomDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];
                possibleDirections.Remove(randomDirection);

                newCell = GetNeighborIndex(randomMainCell, GetDirectionVector(randomDirection));
                if (newCell != -1 && !board[newCell].visited)
                {
                    ConnectRooms(randomMainCell, newCell);
                    board[newCell].visited = true;
                    break;
                }
            }
        }
    }

    void GenerateDungeon()
    {
        RoomBehaviour[,] roomBehaviours = new RoomBehaviour[size.x, size.y];

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[i + j * size.x];
                if (currentCell.visited)
                {
                    int randomRoomIndex = Random.Range(0, rooms.Length);
                    Vector3 roomPosition = new Vector3(i * offset.x, 0, -j * offset.y);

                    RoomBehaviour newRoom = Instantiate(rooms[randomRoomIndex].room, roomPosition, Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    newRoom.name += $" {i}-{j}";

                    newRoom.SetCell(currentCell);

                    InitializeRoomConnections(newRoom, currentCell);
                    SpawnEnemiesForRoom(newRoom, rooms[randomRoomIndex]);

                    roomBehaviours[i, j] = newRoom;
                }
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                RoomBehaviour room = roomBehaviours[i, j];
                if (room != null)
                {
                    RoomBehaviour upNeighbor = (j > 0) ? roomBehaviours[i, j - 1] : null;
                    RoomBehaviour downNeighbor = (j < size.y - 1) ? roomBehaviours[i, j + 1] : null;
                    RoomBehaviour leftNeighbor = (i > 0) ? roomBehaviours[i - 1, j] : null;
                    RoomBehaviour rightNeighbor = (i < size.x - 1) ? roomBehaviours[i + 1, j] : null;

                    room.SetNeighbors(upNeighbor, downNeighbor, rightNeighbor, leftNeighbor);
                }
            }
        }
    }

    void SpawnEnemiesForRoom(RoomBehaviour room, Rule roomRule)
    {
        int numEnemies = Random.Range(roomRule.minEnemies, roomRule.maxEnemies + 1);

        List<GameObject> roomEnemies = new List<GameObject>();

        for (int i = 0; i < numEnemies; i++)
        {
            GameObject enemyPrefab = roomRule.enemyPrefabs[Random.Range(0, roomRule.enemyPrefabs.Length)];
            Vector3 enemyPosition = room.transform.position + new Vector3(
                Random.Range(-offset.x / 2, offset.x / 2),
                0,
                Random.Range(-offset.y / 2, offset.y / 2)
            );

            GameObject newEnemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity, room.transform);
            roomEnemies.Add(newEnemy);

            EnemyController enemyScript = newEnemy.GetComponent<EnemyController>();
            if (enemyScript != null)
            {
                enemyScript.roomBehaviour = room;
            }
        }

        room.enemies = roomEnemies.ToArray();
    }

    void InitializeRoomConnections(RoomBehaviour room, Cell cell)
    {
        for (int i = 0; i < 4; i++)
        {
            bool hasConnection = cell.status[i];
            if (hasConnection)
            {
                if (room.doors[i] != null) room.doors[i].SetActive(false);
                if (room.walls[i] != null) room.walls[i].SetActive(true);
            }
        }
    }

    void SpawnPlayer()
    {
        if (playerPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(0 * offset.x, 1, 0 * -offset.y);
            Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Player prefab is not assigned in the DungeonGenerator.");
        }
    }

    void ConnectRooms(int currentCell, int newCell)
    {
        if (newCell > currentCell)
        {
            if (newCell - 1 == currentCell)
            {
                board[currentCell].status[2] = true;
                board[newCell].status[3] = true;
            }
            else
            {
                board[currentCell].status[1] = true;
                board[newCell].status[0] = true;
            }
        }
        else
        {
            if (newCell + 1 == currentCell)
            {
                board[currentCell].status[3] = true;
                board[newCell].status[2] = true;
            }
            else
            {
                board[currentCell].status[0] = true;
                board[newCell].status[1] = true;
            }
        }
    }

    int GetNeighborIndex(int currentIndex, Vector2Int direction)
    {
        int x = currentIndex % size.x + direction.x;
        int y = currentIndex / size.x + direction.y;
        if (x >= 0 && x < size.x && y >= 0 && y < size.y)
        {
            return x + y * size.x;
        }
        return -1;
    }

    Vector2Int GetDirectionVector(int direction)
    {
        switch (direction)
        {
            case 0: return new Vector2Int(0, -1);
            case 1: return new Vector2Int(0, 1);
            case 2: return new Vector2Int(1, 0);
            case 3: return new Vector2Int(-1, 0);
            default: return Vector2Int.zero;
        }
    }
}
