using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public GameObject starterRoom; // Prefab for the initial room
    public List<GameObject> endRooms; // List of end room prefabs for different connection requirements
    public List<GameObject> tilemapPresets; // List of tilemap presets to use in level generation
    public List<GameObject> specialPresets; // List of special presets that must have all connection points filled
    public int numberOfRooms = 20; // Number of rooms to generate in the level
    public float specialPresetsChance = 0f; // Probability of selecting a special preset
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

    private Dictionary<ConnectionPoint, List<GameObject>> triedPresets = new Dictionary<ConnectionPoint, List<GameObject>>(); // Tracks tried presets for each connection point

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
            const int maxAttempts = 100; // Maximum attempts to place a room and avoid infinite loop

            triedPresets[currentConnectionPoint] = new List<GameObject>(); // Initialize the tried presets list for this connection point

            while (!roomPlaced && attempts < maxAttempts)
            {
                attempts++;

                GameObject preset = Random.value < specialPresetsChance && specialPresets.Count > 0
                                    ? InstantiateRandomSpecialPreset()
                                    : InstantiateRandomPreset();

                if (triedPresets[currentConnectionPoint].Contains(preset))
                {
                    Destroy(preset);
                    continue; // Skip if this preset has already been tried for this connection point
                }

                triedPresets[currentConnectionPoint].Add(preset);
                Debug.Log($"Attempting to place preset: {preset.name} at connection point: {currentConnectionPoint.direction}");

                
                
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
    if (endRooms == null || endRooms.Count == 0)
    {
        Debug.LogError("End rooms list is not assigned or empty.");
        return;
    }

    bool endRoomsPlaced = false;

    // Try placing the end room(s) at any available connection point
    for (int i = 0; i < connectionPoints.Count; i++)
    {
        ConnectionPoint connectionPoint = connectionPoints[i];

        foreach (GameObject endRoomPrefab in endRooms)
        {
            GameObject end = Instantiate(endRoomPrefab);
            Debug.Log($"Attempting to place end room prefab: {endRoomPrefab.name}");

            if (PositionPreset(end, connectionPoint))
            {
                endRoomsPlaced = true;
                Debug.Log($"End room prefab {endRoomPrefab.name} successfully placed at connection point {connectionPoint.direction}");
                break;
            }
            else
            {
                Destroy(end);
            }
        }

        if (endRoomsPlaced)
        {
            break;
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
            Debug.LogError($"The preset {preset.name} is missing the 'ConnectionPoints' empty object.");
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
            Debug.LogError($"No connection point found in the preset {preset.name} for direction: {targetDirection}");
            return false;
        }

        Transform selectedConnectionPoint = connectionPointsDict[targetDirection];
        Vector3 offset = currentConnectionPoint.position - (preset.transform.position + selectedConnectionPoint.localPosition);
        preset.transform.position += offset;

        // Check for overlap
        if (occupiedPositions.Contains(preset.transform.position))
        {
            Debug.Log($"Room overlap detected at position: {preset.transform.position} for prefab {preset.name}. Moving the room 1 unit forward in the direction of the overlap.");
            MoveRoomForward(preset, targetDirection);

            // Check for overlap again after moving the room forward once
            if (occupiedPositions.Contains(preset.transform.position))
            {
                Debug.Log($"Room overlap detected after moving forward at position: {preset.transform.position} for prefab {preset.name}. Replacing the last room with a new room.");
                ReplaceLastRoomWithAlternativeDirection(preset, currentConnectionPoint);
                return false;
            }
        }

        // Ensure the connection is valid
        if (!IsValidConnection(currentConnectionPoint.direction, targetDirection))
        {
            Debug.LogError($"Invalid connection: {currentConnectionPoint.direction} cannot connect to {targetDirection} for prefab {preset.name}");
            ReplaceLastRoomWithAlternativeDirection(preset, currentConnectionPoint);
            return false;
        }

        // Log the connection between rooms
        Debug.Log($"Room {preset.name} connected at position {preset.transform.position}. Connection point: {currentConnectionPoint.direction} connected to {targetDirection}.");

        // Add connection points to the list for further expansion
        foreach (Transform connectionPoint in connectionPointsContainer)
        {
            if (connectionPoint.name != targetDirection)
            {
                connectionPoints.Add(new ConnectionPoint(connectionPoint.position, connectionPoint.name));
            }
        }

        // Add room position to occupied positions list
        occupiedPositions.Add(preset.transform.position);

        // Optionally spawn enemies in the room
        SpawnEnemies(preset);

        return true;
    }

    bool IsValidConnection(string directionA, string directionB)
    {
        return oppositeDirections.ContainsKey(directionA) && oppositeDirections[directionA] == directionB;
    }

    void MoveRoomForward(GameObject room, string direction)
    {
        Vector3 moveVector = Vector3.zero;

        switch (direction)
        {
            case "LeftUp":
                moveVector = new Vector3(-39, 0, 0);
                break;
            case "RightUp":
                moveVector = new Vector3(39, 0, 0);
                break;
            case "LeftDown":
                moveVector = new Vector3(-39, 0, 0);
                break;
            case "RightDown":
                moveVector = new Vector3(39, 0, 0);
                break;
            case "Up":
                moveVector = new Vector3(0, 19, 0);
                break;
            case "Down":
                moveVector = new Vector3(0, -19, 0);
                break;
        }

        room.transform.position += moveVector;
        Debug.Log($"Moved room {room.name} forward to {room.transform.position} in direction {direction}.");
    }

    void ReplaceLastRoomWithAlternativeDirection(GameObject newRoom, ConnectionPoint currentConnectionPoint)
    {
        if (generatedRooms.Count > 0)
        {
            GameObject lastRoom = generatedRooms[generatedRooms.Count - 1];
            generatedRooms.RemoveAt(generatedRooms.Count - 1);
            occupiedPositions.Remove(lastRoom.transform.position);
            Destroy(lastRoom);

            List<string> alternativeDirections = new List<string>(oppositeDirections.Values);
            alternativeDirections.Remove(currentConnectionPoint.direction);

            foreach (string alternativeDirection in alternativeDirections)
            {
                if (PositionPreset(newRoom, currentConnectionPoint, alternativeDirection))
                {
                    generatedRooms.Add(newRoom);
                    Debug.Log($"Replaced last room with a new room {newRoom.name} at position {newRoom.transform.position} using alternative direction {alternativeDirection}.");
                    return;
                }
            }

            Debug.LogWarning($"No valid alternative direction found. Using the first available direction for room {newRoom.name}.");
            if (PositionPreset(newRoom, currentConnectionPoint, alternativeDirections[0]))
            {
                generatedRooms.Add(newRoom);
                Debug.Log($"Replaced last room with a new room {newRoom.name} at position {newRoom.transform.position} using fallback direction {alternativeDirections[0]}.");
            }
        }
    }

    void BacktrackAndPlaceEndRoom()
    {
        for (int i = generatedRooms.Count - 1; i >= 0; i--)
        {
            GameObject lastRoom = generatedRooms[i];
            ConnectionPoint lastRoomConnectionPoint = connectionPoints[connectionPoints.Count - 1];

            if (lastRoomConnectionPoint.direction == "RightDown")
            {
                GameObject endRoomPrefab = endRooms[Random.Range(0, endRooms.Count)];
                GameObject end = Instantiate(endRoomPrefab);
                Debug.Log($"Attempting to place end room prefab: {endRoomPrefab.name} at backtracked position.");

                if (PositionPreset(end, lastRoomConnectionPoint))
                {
                    generatedRooms.Add(end);
                    Debug.Log("End room successfully placed after backtracking.");
                    return;
                }
                else
                {
                    Destroy(end);
                }
            }
        }

        Debug.LogError("Failed to place end room after backtracking.");
    }

    void SpawnEnemies(GameObject room)
    {
        foreach (Transform child in room.transform)
        {
            if (child.CompareTag("SpawnPoint"))
            {
                GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                Instantiate(enemyPrefab, child.position, Quaternion.identity);
            }
        }
    }

    public class ConnectionPoint
    {
        public Vector3 position;
        public string direction;

        public ConnectionPoint(Vector3 pos, string dir)
        {
            position = pos;
            direction = dir;
        }
    }
}
