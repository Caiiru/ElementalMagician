using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    public GameObject starterRoom; // Prefab para o grid inicial
    public List<GameObject> endRooms; // Lista de prefabs para o grid final
    public List<GameObject> tilemapPresets; // Lista de presets do tilemap para usar na geração do nível
    public List<GameObject> specialPresets; // Lista de presets especiais que devem ter todos os pontos de conexão preenchidos
    public int numberOfRooms = 5; // Número de grids a serem gerados no nível
    public float specialPresetsChance = 0.1f; // Probabilidade de um preset especial ser selecionado
    public List<GameObject> enemyPrefabs; // Lista de prefabs de inimigos para spawnar no nível
    public int maxRooms = 100; // Número máximo de grids para evitar loops infinitos
    public int minRooms = 5;
    private List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>(); // Lista para acompanhar os pontos de conexão disponíveis

    private readonly Dictionary<string, string> oppositeDirections = new Dictionary<string, string>
    {
        { "LeftUp", "RightUp" }, // Direções opostas: Esquerda para cima -> Direita para cima
        { "RightUp", "LeftUp" }, // Direções opostas: Direita para cima -> Esquerda para cima
        { "LeftDown", "RightDown" }, // Direções opostas: Esquerda para baixo -> Direita para baixo
        { "RightDown", "LeftDown" }, // Direções opostas: Direita para baixo -> Esquerda para baixo
        { "Up", "Down" }, // Direções opostas: Cima -> Baixo
        { "Down", "Up" } // Direções opostas: Baixo -> Cima
    };

    public List<GameObject> spawnedPresets = new List<GameObject>();

    void Start()
    {
        // Inicia o processo de geração do nível quando o jogo começa
        GameManager.GetInstance().SetLevelGenerator(this.gameObject);
    }

    void GenerateLevel()
    {
        // Coloca o grid inicial primeiro
        PlaceStarterRoom();

        int totalRoomsGenerated = 1; // Começamos com o grid inicial já colocado

        // Gera os grids intermediários
        while (totalRoomsGenerated < numberOfRooms && connectionPoints.Count > 0)
        {
            // Seleciona um ponto de conexão aleatório para expandir a partir dele
            int connectionIndex = Random.Range(0, connectionPoints.Count);
            ConnectionPoint currentConnectionPoint = connectionPoints[connectionIndex];
            connectionPoints.RemoveAt(connectionIndex);

            bool roomPlaced = false;
            int attempts = 0;
            const int maxAttempts = 50; // Tentativas máximas para colocar um grid e evitar loop infinito

            while (!roomPlaced && attempts < maxAttempts)
            {
                attempts++;

                GameObject preset = Random.value < specialPresetsChance && specialPresets.Count > 0 
                                    ? InstantiateRandomSpecialPreset() 
                                    : InstantiateRandomPreset();
                
                preset.transform.SetParent(this.transform);

                if (totalRoomsGenerated == numberOfRooms - 1)
                {
                    // Último grid antes do grid final
                    if (PositionPreset(preset, currentConnectionPoint, "RightDown"))
                    {
                        roomPlaced = true;
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
                break;
            }

            if (totalRoomsGenerated >= numberOfRooms)
            {
                break;
            }
        }

        // Coloca o grid final
        PlaceEndRoom();
    }

    void PlaceStarterRoom()
    {
        if (starterRoom == null)
        {
            return;
        }

        GameObject starter = Instantiate(starterRoom);
        starter.transform.SetParent(this.transform);
        spawnedPresets.Add(starterRoom);
        starter.transform.position = Vector3.zero; // Coloca o grid inicial na origem

        Transform connectionPointsContainer = starter.transform.Find("ConnectionPoints");

        if (connectionPointsContainer == null)
        {
            Destroy(starter);
            return;
        }

        Dictionary<string, Transform> connectionPointsDict = new Dictionary<string, Transform>();
        foreach (Transform connectionPoint in connectionPointsContainer)
        {
            connectionPointsDict[connectionPoint.name] = connectionPoint;
        }

        if (!connectionPointsDict.ContainsKey("RightDown"))
        {
            Destroy(starter);
            return;
        }

        // Adiciona o ponto de conexão "RightDown" do grid inicial à lista
        connectionPoints.Add(new ConnectionPoint(connectionPointsDict["RightDown"].position, "RightDown"));
    }

    void PlaceEndRoom()
    {
        if (endRooms == null || endRooms.Count == 0)
        {
            return;
        }

        bool endRoomPlaced = false;

        // Tenta colocar o grid final em um ponto de conexão "RightDown"
        for (int i = 0; i < connectionPoints.Count; i++)
        {
            if (connectionPoints[i].direction == "RightDown")
            {
                GameObject end = ChooseEndRoomPrefabForConnection("RightDown");
                
                end.transform.SetParent(this.transform);
                spawnedPresets.Add(end);
                if (PositionPreset(end, connectionPoints[i]))
                {
                    endRoomPlaced = true;
                    break;
                }
                else
                {
                    Destroy(end);
                }
            }
        }

        if (!endRoomPlaced)
        {
            // Optionally handle the case where the end room couldn't be placed
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
    
    GameObject ChooseEndRoomPrefabForConnection(string connection)
    {
        foreach (GameObject endRoomPrefab in endRooms)
        {
            if (HasConnection(endRoomPrefab, connection))
            {
                return endRoomPrefab;
            }
        }

        // If no suitable end room prefab is found, return the first end room prefab as a fallback
        Debug.LogWarning($"No end room prefab found for connection: {connection}. Using the first end room prefab as a fallback.");
        return endRooms[0];
    }

    bool HasConnection(GameObject room, string connection)
    {
        Transform connectionPointsContainer = room.transform.Find("ConnectionPoints");

        if (connectionPointsContainer == null)
        {
            return false;
        }

        foreach (Transform connectionPoint in connectionPointsContainer)
        {
            if (connectionPoint.name == connection)
            {
                return true;
            }
        }

        return false;
    }

    bool PositionPreset(GameObject preset, ConnectionPoint currentConnectionPoint, string specificDirection = null)
    {
        Transform connectionPointsContainer = preset.transform.Find("ConnectionPoints");

        if (connectionPointsContainer == null)
        {
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

        spawnedPresets.Add(preset);
        SpawnEnemies(preset);

        return true;
    }

    void SpawnEnemies(GameObject preset)
    {
        Transform enemySpawnPoints = preset.transform.Find("EnemySpawnPoints");
        if (enemySpawnPoints != null && enemyPrefabs != null && enemyPrefabs.Count > 0)
        {
            foreach (Transform spawnPoint in enemySpawnPoints)
            {
                if (Random.value > 0.2f)
                {
                    int randomIndex = Random.Range(0, enemyPrefabs.Count);
                    var enemyInstance = Instantiate(enemyPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
                    enemyInstance.transform.SetParent(preset.transform);
                    EnemyManager.instance.AddEnemy(enemyInstance);
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

    public GameObject GetCurrentPreset()
    {
        return null;
    }

    public GameObject GetPresetByNumber(int value)
    {
        var i = 0;
        foreach (var preset in spawnedPresets)
        {
            if (i == value)
            {
                return preset;
            }
            i++;
        }

        return null;
    }

    private void SetCurrentPresetInConnectionPoints(Transform ConnectionHolder, GameObject preset)
    {
        var connections = ConnectionHolder.GetComponentsInChildren<ConnectionPoint_Script>();
        foreach (var point in connections)
        {
            point.currentPreset = preset;
        }
    }

    private void SetToPresetInConnectionPoints(Transform connectionHolder, GameObject preset)
    {
        var connections = connectionHolder.GetComponentsInChildren<ConnectionPoint_Script>();
        foreach (var point in connections)
        {
            point.toPreset = preset;
        }
    }

    #region Singleton

    public static LevelGenerator instance;

    private void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(this);
        }

        instance = this;
        
        GenerateLevel();
    }

    #endregion
}
