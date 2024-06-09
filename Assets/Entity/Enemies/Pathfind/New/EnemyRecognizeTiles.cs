using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyRecognizeTiles : MonoBehaviour
{
    //primeiro tenho que fazer o inimigo reconhecer os tiles que estão em volta dele baseado no preset que ele está. 
    //depois setar quais são os tiles possiveis para caminhar
    [SerializeField] private GameObject presetGameObject;
    [SerializeField]private Grid presetGrid;
    private Tilemap tilemap;

    [Space] [Header("Debug")] public bool Debug;
    public GameObject walkablePrefab;
    public GameObject nonWalkablePrefab;
    public Vector3 offset;

    private void Awake()
    {
        presetGameObject = this.transform.parent.gameObject;
        presetGrid = presetGameObject.GetComponent<Grid>();
        tilemap = presetGrid.GetComponentInChildren<Tilemap>();
    }

    void Start()
    {
       
        
        //offset = new Vector3(0.5f, 0.5f, 0);
        offset = tilemap.cellSize / 2;
        var tilesToWalk = AnalyzeTilemap();
        //StartCoroutine(AnalyzeTilemap());
    }
    
    
    private List<Vector3> AnalyzeTilemap()
    {
        BoundsInt bounds = tilemap.cellBounds;
        List<Vector3> walkableTiles = new List<Vector3>();
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {  
                Vector3Int localPlace = (new Vector3Int(x, y, (int)tilemap.transform.position.y));
                Vector3 place = tilemap.CellToWorld(localPlace);
                TileBase tile = tilemap.GetTile(localPlace);
                if (tile != null && IsWalkableTile(localPlace))
                {
                    walkableTiles.Add((localPlace + offset)+Vector3.up);
                    if (Debug)
                    {
                        Instantiate(walkablePrefab, place + offset, Quaternion.identity);
                        Instantiate(nonWalkablePrefab, (place + offset) + Vector3.up, Quaternion.identity);
                    }

                }
            }
        }

        return walkableTiles;
        
    }
    
   /*
    IEnumerator AnalyzeTilemap()
    {
        BoundsInt bounds = tilemap.cellBounds;
        List<Vector3> walkableTiles = new List<Vector3>();
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {  
                Vector3Int localPlace = (new Vector3Int(x, y, (int)tilemap.transform.position.y));
                Vector3 place = tilemap.CellToWorld(localPlace);
                TileBase tile = tilemap.GetTile(localPlace);
                if (tile != null)
                {
                    walkableTiles.Add((localPlace + offset)+Vector3.up);
                    yield return new WaitForSeconds(0.05f);
                    GameObject prefab = IsWalkableTile(localPlace) ? walkablePrefab : nonWalkablePrefab;
                    Instantiate(prefab, place+offset, Quaternion.identity); 

                }
            }
        }  
    }
    */

    private bool IsWalkableTile(Vector3Int place)
    {
        var compareTile = tilemap.GetTile(place+Vector3Int.up);
        if(compareTile == null)
            return true;
        return false;
        
    }

    public Tilemap GetTilemap()
    {
        return tilemap;
    }

    public Grid GetGrid()
    {
        return presetGrid;
    }
    /*
    private List<Vector3Int> FindPath(Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        path.Add(start);
        path.Add(end);
        return path;
    }
    */
    private List<Vector3Int> FindPath(Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();
        List<Vector3Int> openSet = new List<Vector3Int>();
        //PriorityQueue<Vector3Int> openSet = new PriorityQueue<Vector3Int>();
        //openSet.Enqueue(start,0);

        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, float> gScore = new Dictionary<Vector3Int, float>();
        gScore[start] = 0;

        Dictionary<Vector3Int, float> fScore = new Dictionary<Vector3Int, float>();
        fScore[start] = HeuristicCoastEsmitate(start, end);

        while (openSet.Count > 0)
        {
            Vector3Int current = openSet[0];
            foreach (var tile in openSet)
            {
                if (fScore.ContainsKey(tile) && fScore[tile] < fScore[current])
                    current = tile;
            }
            if (current == end)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);
            
            foreach (Vector3Int neighbor in GetNeighbors(current))
            {
                if(closedSet.Contains(neighbor))
                    continue;

                float tentativeGScore = gScore[current] + Vector3Int.Distance(current, neighbor);
                if(!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else if (tentativeGScore >= gScore[neighbor])
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + HeuristicCoastEsmitate(neighbor, end);
            }
        }


        path.Add(start);
        path.Add(end);
        return path;
    }

    private float HeuristicCoastEsmitate(Vector3Int a, Vector3Int b)
    {
        return Vector3Int.Distance(a, b);
    }

    private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector3Int> totalPath = new List<Vector3Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }
        totalPath.Reverse();
        return totalPath;
    }

    public List<Vector3Int> GetPath(Vector3Int start, Vector3Int end)
    {
        return FindPath(start, end);
    }

    private List<Vector3Int> GetNeighbors(Vector3Int tile)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        Vector3Int[] directions =
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0)
        };
        foreach (var direction in directions)
        {
            Vector3Int neighbor = tile + direction;
            if (IsWalkableTile(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
