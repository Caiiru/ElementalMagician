using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public GameObject starterRoom; 
    public List<GameObject> endRooms; 
    public List<GameObject> tilemapPresets; 
    public List<GameObject> specialPresets; 
    public int numberOfRooms = 20; 
    public float specialPresetsChance = 0f; 
    public List<GameObject> enemyPrefabs; 
    public int maxRooms = 100; 
    public int minimumRooms = 5; // Minimum number of rooms required

    private List<Vector3> occupiedPositions = new List<Vector3>(); 
    private List<GameObject> generatedRooms = new List<GameObject>(); 
    private List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>(); 

    private readonly Dictionary<string, string> oppositeDirections = new Dictionary<string, string>
    {
        { "LeftUp", "RightUp" }, 
        { "RightUp", "LeftUp" }, 
        { "LeftDown", "RightDown" }, 
        { "RightDown", "LeftDown" }, 
        { "Up", "Down" }, 
        { "Down", "Up" }
    };

    private Dictionary<ConnectionPoint, List<GameObject>> triedPresets = new Dictionary<ConnectionPoint, List<GameObject>>(); 

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        PlaceStarterRoom();

        int totalRoomsGenerated = 1; 

        while (totalRoomsGenerated < numberOfRooms && connectionPoints.Count > 0)
        {
            int connectionIndex = Random.Range(0, connectionPoints.Count);
            ConnectionPoint currentConnectionPoint = connectionPoints[connectionIndex];
            connectionPoints.RemoveAt(connectionIndex);

            bool roomPlaced = false;
            int attempts = 0;
            const int maxAttempts = 100; 

            triedPresets[currentConnectionPoint] = new List<GameObject>(); 

            while (!roomPlaced && attempts < maxAttempts)
            {
                attempts++;

                GameObject preset = Random.value < specialPresetsChance && specialPresets.Count > 0
                                    ? InstantiateRandomSpecialPreset()
                                    : InstantiateRandomPreset();

                if (triedPresets[currentConnectionPoint].Contains(preset))
                {
                    Destroy(preset);
                    continue; 
                }

                triedPresets[currentConnectionPoint].Add(preset);

                if (PositionPreset(preset, currentConnectionPoint))
                {
                    roomPlaced = true;
                    generatedRooms.Add(preset); 
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
                break;
            }
        }

        PlaceEndRooms();

        // Check if the number of rooms generated meets the minimum requirement
        if (totalRoomsGenerated < minimumRooms)
        {
            Debug.LogError($"Only {totalRoomsGenerated} rooms generated, which is less than the minimum required {minimumRooms}. Retrying generation.");
            RetryGeneration();
        }
    }

    void PlaceStarterRoom()
    {
        if (starterRoom == null)
        {
            Debug.LogError("Starter room is not assigned.");
            return;
        }

        GameObject starter = Instantiate(starterRoom);
        starter.transform.position = Vector3.zero; 

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
        const int maxAttempts = 18; // Increased max attempts for placing end rooms

        // Shuffle the end rooms list to randomize the order of selection
        ShuffleEndRooms();

        // Try placing the end room(s) at any available connection point
        for (int i = 0; i < connectionPoints.Count; i++)
        {
            ConnectionPoint connectionPoint = connectionPoints[i];

            // Track tried presets for this connection point
            List<GameObject> triedPresets = new List<GameObject>();

            int attempts = 0;

            while (!endRoomsPlaced && attempts < maxAttempts)
            {
                attempts++;

                foreach (GameObject endRoomPrefab in endRooms)
                {
                    if (triedPresets.Contains(endRoomPrefab))
                    {
                        continue; // Skip if this preset has already been tried for this connection point
                    }

                    triedPresets.Add(endRoomPrefab);

                    GameObject end = Instantiate(endRoomPrefab);
                    Debug.Log($"Attempting to place end room prefab: {endRoomPrefab.name} at connection point: {connectionPoint.direction}");

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

    void ShuffleEndRooms()
    {
        for (int i = 0; i < endRooms.Count; i++)
        {
            GameObject temp = endRooms[i];
            int randomIndex = Random.Range(i, endRooms.Count);
            endRooms[i] = endRooms[randomIndex];
            endRooms[randomIndex] = temp;
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
        if (IsOverlap(preset))
        {
            Debug.Log($"Room overlap detected at position: {preset.transform.position} for prefab {preset.name}. Replacing the last room with a new room.");
            ReplaceLastRoomWithAlternativeDirection(preset, currentConnectionPoint);
            return false;
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
        //SpawnEnemies(preset);

        return true;
    }

    bool IsOverlap(GameObject room)
    {
        // Check if the room position overlaps with any occupied positions
        if (occupiedPositions.Contains(room.transform.position))
        {
            return true;
        }

        // Check overlap with the starter room, assuming the starter room is at Vector3.zero
        if (room.transform.position == Vector3.zero)
        {
            return true;
        }

        return false;
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

        Debug.LogError("Failed to place end room after backtracking. Retrying generation.");
        RetryGeneration();
    }

    void RetryGeneration()
    {
        // Clear existing generated rooms and occupied positions
        foreach (GameObject room in generatedRooms)
        {
            Destroy(room);
        }
        generatedRooms.Clear();
        occupiedPositions.Clear();

        // Reset connection points
        connectionPoints.Clear();

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Other existing methods and variables

    // void SpawnEnemies(GameObject room)
    // {
    //     foreach (Transform child in room.transform)
    //     {
    //         if (child.CompareTag("SpawnPoint"))
    //         {
    //             GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
    //             Instantiate(enemyPrefab, child.position, Quaternion.identity);
    //         }
    //     }
    // }

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
