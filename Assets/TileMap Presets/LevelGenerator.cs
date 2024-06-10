using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public GameObject starterRoom; // Prefab for the initial room
    public List<GameObject> endRooms; // List of end room prefabs for different connection requirements
    public List<GameObject> tilemapPresets; // List of tilemap presets to use in level generation
    public List<GameObject> specialPresets; // List of special presets that must have all connection points filled
    public int numberOfRooms = 20; // Number of rooms to generate in the level
    public float specialPresetsChance = 0.1f; // Probability of selecting a special preset
    public List<GameObject> enemyPrefabs; // List of enemy prefabs to spawn in the level
    public int maxRooms = 100; // Maximum number of rooms to avoid infinite loops
    private List<Vector3> occupiedPositions = new List<Vector3>(); // List to track occupied room positions
    private List<GameObject> generatedRooms = new List<GameObject>(); // List to track generated rooms
    private List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>(); // List to track available connection points

    private readonly Dictionary<string, string> oppositeDirections = new Dictionary<string, string>
    {
        { "LeftUp", "RightUp" }, // Opposite directions: Left Up -> Right Up
        { "RightUp", "LeftUp" }, // Opposite directions: Right Up -> Left Up
        { "LeftDown", "RightDown" }, // Opposite directions: Left Down -> Right Down
        { "RightDown", "LeftDown" }, // Opposite directions: Right Down -> Left Down
        { "Up", "Down" }, // Opposite directions: Up -> Down
        { "Down", "Up" } // Opposite directions: Down -> Up
    };

    void Start()
    {
        // Start the level generation process when the game starts
        GenerateLevel();
    }

    void GenerateLevel()
    {
        // Place the initial room first
        PlaceStarterRoom();

        int totalRoomsGenerated = 1; // Start with the initial room already placed

        // Generate intermediate rooms
        while (totalRoomsGenerated < numberOfRooms && connectionPoints.Count > 0)
        {
            // Select a random connection point to expand from
            int connectionIndex = Random.Range(0, connectionPoints.Count);
            ConnectionPoint currentConnectionPoint = connectionPoints[connectionIndex];
            connectionPoints.RemoveAt(connectionIndex);

            bool roomPlaced = false;
            int attempts = 0;
            const int maxAttempts = 50; // Maximum attempts to place a room and avoid infinite loop

            while (!roomPlaced && attempts < maxAttempts)
            {
                attempts++;

                GameObject preset = Random.value < specialPresetsChance && specialPresets.Count > 0 
                                    ? InstantiateRandomSpecialPreset() 
                                    : InstantiateRandomPreset();

                if (totalRoomsGenerated == numberOfRooms - 1)
                {
                    // Last room before the end room
                    if (PositionPreset(preset, currentConnectionPoint, "RightDown"))
                    {
                        roomPlaced = true;
                        generatedRooms.Add(preset); // Add the last room to the generated rooms list
                        totalRoomsGenerated++;
                    }
                    else
                    {
                        Destroy(preset);
                    }
                }
                else
                {
                    if (PositionPreset(preset, currentConnectionPoint))
                    {
                        roomPlaced = true;
                        generatedRooms.Add(preset); // Add the placed room to the generated rooms list
                        totalRoomsGenerated++;
                    }
                    else
                    {
                        Destroy(preset);
                    }
                }
            }

            if (!roomPlaced)
            {
                Debug.LogError("Failed to place the room after maximum attempts.");
                break;
            }

            if (totalRoomsGenerated >= numberOfRooms)
            {
                Debug.Log($"Total number of rooms reached: {totalRoomsGenerated}");
                break;
            }
        }

        // Place the end room(s)
        PlaceEndRooms();
    }

    void PlaceStarterRoom()
    {
        if (starterRoom == null)
        {
            Debug.LogError("Starter room is not assigned.");
            return;
        }

        GameObject starter = Instantiate(starterRoom);
        starter.transform.position = Vector3.zero; // Place the starter room at the origin

        Transform connectionPointsContainer = starter.transform.Find("ConnectionPoints");

        if (connectionPointsContainer == null)
        {
            Debug.LogError("The starter room is missing the 'ConnectionPoints' empty object.");
            Destroy(starter);
            return;
        }

        Dictionary<string, Transform> connectionPointsDict =  new Dictionary<string, Transform>();
        foreach (Transform connectionPoint in connectionPointsContainer)
        {
            connectionPointsDict[connectionPoint.name] = connectionPoint;
        }

        if (!connectionPointsDict.ContainsKey("RightDown"))
        {
            Debug.LogError("The starter room does not have a 'RightDown' connection point.");
            Destroy(starter);
            return;
        }

        // Add the "RightDown" connection point of the starter room to the list
        connectionPoints.Add(new ConnectionPoint(connectionPointsDict["RightDown"].position, "RightDown"));
    }

    void PlaceEndRooms()
    {
        if (endRooms == null)
        {
            Debug.LogError("End rooms are not assigned.");
            return;
        }

        bool endRoomsPlaced = false;

        // Try placing the end room(s) at a "RightDown" connection point
        for (int i = 0; i < connectionPoints.Count; i++)
        {
            if (connectionPoints[i].direction == "RightDown")
            {
                GameObject endRoomPrefab = endRooms[Random.Range(0, endRooms.Count)];
                GameObject end = Instantiate(endRoomPrefab);

                if (PositionPreset(end, connectionPoints[i]))
                {
                    endRoomsPlaced = true;
                    break;
                }
                else
                {
                    Destroy(end);
                }
            }
        }

        if (!endRoomsPlaced)
        {
            // Backtrack and attempt to place the end room from a previous room
            BacktrackAndPlaceEndRoom();
        }
    }

    GameObject InstantiateRandomPreset()
    {
        int randomIndex = Random.Range(0, tilemapPresets.Count);
        return Instantiate(tilemapPresets[randomIndex]);
    }

    GameObject InstantiateRandomSpecialPreset()
    {
        int randomIndex = Random.Range(0, specialPresets.Count);
        return Instantiate(specialPresets[randomIndex]);
    }

    bool PositionPreset(GameObject preset, ConnectionPoint currentConnectionPoint, string specificDirection = null)
    {
        Transform connectionPointsContainer = preset.transform.Find("ConnectionPoints");

        if (connectionPointsContainer == null)
        {
            Debug.LogError("The preset is missing the 'ConnectionPoints' empty object.");
            return false;
        }

        if (occupiedPositions.Contains(preset.transform.position))
        {
            Debug.Log("Room overlap detected. Replacing the existing room.");
            ReplaceExistingRoom(preset);
            return false;
        }

        Dictionary<string, Transform> connectionPointsDict = new Dictionary<string, Transform>();
        foreach (Transform connectionPoint in connectionPointsContainer)
        {
            connectionPointsDict[connectionPoint.name] = connectionPoint;
        }

        string targetDirection = specificDirection != null ? specificDirection : oppositeDirections[currentConnectionPoint.direction];
        if (!connectionPointsDict.ContainsKey(targetDirection))
        {
            return false;
        }

        Transform selectedConnectionPoint = connectionPointsDict[targetDirection];
        Vector3 offset = currentConnectionPoint.position - (preset.transform.position + selectedConnectionPoint.localPosition);
        preset.transform.position += offset;

        foreach (var kvp in connectionPointsDict)
        {
            if (kvp.Key != targetDirection)
            {
                connectionPoints.Add(new ConnectionPoint(kvp.Value.position, kvp.Key));
            }
        }

        // Check for room overlap
        if (occupiedPositions.Contains(preset.transform.position))
        {
            Debug.Log("Room overlap detected. Discarding the room.");
            return false;
        }

        // Add the room position to the occupied positions list
        occupiedPositions.Add(preset.transform.position);

        SpawnEnemies(preset);

        return true;
    }

    void ReplaceExistingRoom(GameObject newRoom)
    {
        // Replace the last generated room with the new one
        if (generatedRooms.Count > 0)
        {
            GameObject lastRoom = generatedRooms[generatedRooms.Count - 1];
            generatedRooms.RemoveAt(generatedRooms.Count - 1);
            Destroy(lastRoom);
            generatedRooms.Add(newRoom);
        }
        else
        {
            Debug.LogWarning("No existing room to replace. Skipping replacement.");
        }
    }

    void SpawnEnemies(GameObject preset)
    {
        Transform enemySpawnPoints = preset.transform.Find("EnemySpawnPoints");
        if (enemySpawnPoints != null && enemyPrefabs != null && enemyPrefabs.Count > 0)
        {
            foreach (Transform spawnPoint in enemySpawnPoints)
            {
                // Spawn enemies at the spawn points
                if (Random.value > 0.5f)
                {
                    int randomIndex = Random.Range(0, enemyPrefabs.Count);
                    Instantiate(enemyPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
                }
            }
        }
    }

    private class ConnectionPoint
    {
        public Vector3 position;
        public string direction;

        public ConnectionPoint(Vector3 pos, string dir)
        {
            position = pos;
            direction = dir;
        }
    }

    void BacktrackAndPlaceEndRoom()
    {
        // Iterate through the generated rooms list in reverse order
        for (int i = generatedRooms.Count - 1; i >= 0; i--)
        {
            // Check if the room at the current index has a suitable connection point for the end room
            GameObject room = generatedRooms[i];
            if (CheckConnectionPointAvailability(room.transform.position, "RightDown"))
            {
                // Place the end room from this room
                GameObject endRoomPrefab = ChooseEndRoomPrefabForConnection("RightDown"); // Choose the appropriate end room prefab based on connection requirements
                GameObject end = Instantiate(endRoomPrefab);
                if (PositionPreset(end, new ConnectionPoint(room.transform.position, "RightDown")))
                {
                    Debug.Log("End room successfully placed from a previous room.");
                    break;
                }
                else
                {
                    Destroy(end);
                }
            }
        }
    }

        GameObject ChooseEndRoomPrefabForConnection(string connection)
    {
        // Iterate through the list of end room prefabs and choose the one with the specified connection
        foreach (GameObject endRoomPrefab in endRooms)
        {
            // Check if the end room prefab has the required connection
            //if (endRoomPrefab.GetComponent<Room>().HasConnection(connection))
            {
                return endRoomPrefab;
            }
        }

        // If no suitable end room prefab is found, return the first end room prefab as a fallback
        Debug.LogWarning($"No end room prefab found for connection: {connection}. Using the first end room prefab as a fallback.");
        return endRooms[0];
    }


    bool CheckConnectionPointAvailability(Vector3 position, string direction)
    {
        // Check if the specified direction connection point is available at the given position
        foreach (var point in connectionPoints)
        {
            if (point.position == position && point.direction == direction)
            {
                return true;
            }
        }

        return false;
    }
}


