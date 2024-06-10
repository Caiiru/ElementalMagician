using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    public GameObject starterRoom; // Prefab para o grid inicial
    public GameObject endRoom; // Prefab para o grid final
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
        //InvokeRepeating("GenerateLevel",2,2);
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
                //Debug.LogError("Falha ao colocar o grid após tentativas máximas.");
                break;
            }

            if (totalRoomsGenerated >= numberOfRooms)
            {
                //Debug.Log($"Número total de grids atingido: {totalRoomsGenerated}");
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
           // Debug.LogError("grid inicial não está atribuído.");
            return;
        }

        GameObject starter = Instantiate(starterRoom);
        starter.transform.SetParent(this.transform);
        spawnedPresets.Add(starterRoom);
        starter.transform.position = Vector3.zero; // Coloca o grid inicial na origem

        Transform connectionPointsContainer = starter.transform.Find("ConnectionPoints");
        //SetCurrentPresetInConnectionPoints(connectionPointsContainer,starter);

        if (connectionPointsContainer == null)
        {
            //Debug.LogError("O grid inicial está sem o empty 'ConnectionPoints'.");
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
           // Debug.LogError("O grid inicial não possui um ponto de conexão 'RightDown'.");
            Destroy(starter);
            return;
        }

        // Adiciona o ponto de conexão "RightDown" do grid inicial à lista
        connectionPoints.Add(new ConnectionPoint(connectionPointsDict["RightDown"].position, "RightDown"));
    }

    void PlaceEndRoom()
    {
        if (endRoom == null)
        {
          //  Debug.LogError("grid final não está atribuído.");
            return;
        }

        bool endRoomPlaced = false;

        // Tenta colocar o grid final em um ponto de conexão "RightDown"
        for (int i = 0; i < connectionPoints.Count; i++)
        {
            if (connectionPoints[i].direction == "RightDown")
            {
                GameObject end = Instantiate(endRoom);
                
                end.transform.SetParent(this.transform);
                spawnedPresets.Add(endRoom);
                if (PositionPreset(end, connectionPoints[i]))
                {
                    //SetCurrentPresetInConnectionPoints(end.transform.Find("ConnectionPoints"),end);
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
            //Debug.LogError("Falha ao colocar o grid final.");
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
           // Debug.LogError("Preset está sem o empty 'ConnectionPoints'.");
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
            //Debug.LogError($"Preset não possui um ConnectionPoint '{targetDirection}' adequado.");
            return false;
        }

        Transform selectedConnectionPoint = connectionPointsDict[targetDirection];
        //SetCurrentPresetInConnectionPoints(preset.transform.Find("ConnectionPoints"),preset);
        Vector3 offset = currentConnectionPoint.position - (preset.transform.position + selectedConnectionPoint.localPosition);
        preset.transform.position += offset;

        //Debug.Log($"Colocando o preset em: {preset.transform.position} com deslocamento: {offset}");

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
    Debug.Log("Enemy spawn function ");
    Transform enemySpawnPoints = preset.transform.Find("EnemySpawnPoints");
    if (enemySpawnPoints != null && enemyPrefabs != null && enemyPrefabs.Count > 0)
    {
        foreach (Transform spawnPoint in enemySpawnPoints)
        {
            // Inimigos nos spawnpoints
            if (Random.value > 0.2f)
            {
                int randomIndex = Random.Range(0, enemyPrefabs.Count);
                var enemyInstance = Instantiate(enemyPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
                //Debug.Log("Spawn Enemy ");
                //enemyInstance.transform.SetParent(EnemyManager.instance.getManager().transform);
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

    private void SetCurrentPresetInConnectionPoints(Transform ConnectionHolder,GameObject preset)
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

