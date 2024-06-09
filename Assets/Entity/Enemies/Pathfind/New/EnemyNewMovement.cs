using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(EnemyRecognizeTiles))]
[RequireComponent(typeof(EnemySeePlayer))]
public class EnemyNewMovement : MonoBehaviour
{
    public Transform target;  // Referência para o jogador (pode ser removido se não for necessário)
    public float speed = 2f;  // Velocidade do movimento da IA
    public List<Vector3> path;  // Lista de posições no caminho
    [SerializeField] private int targetIndex;  // Índice do próximo ponto no caminho
    [SerializeField] private int faceDirection = -1;  // Direção que o inimigo está virado

    private EnemyRecognizeTiles pathfind;  // Referência para o script de reconhecimento de tiles
    private EnemySeePlayer enemyEyes;  // Referência para o script de visão do jogador
    private Tilemap tilemap;  // Referência para o Tilemap
    private Grid grid;  // Referência para o Grid

    [Space] [Header("Debug")] public GameObject pathIndicator;  // Indicador de caminho para depuração
    public Vector3 currentTarget;  // Posição do alvo atual

    void Start()
    {
        faceDirection = -1;
        pathfind = GetComponent<EnemyRecognizeTiles>();
        tilemap = pathfind.GetTilemap();
        grid = pathfind.GetGrid();
        enemyEyes = GetComponent<EnemySeePlayer>();

        InitializePath();
    }

    private void InitializePath()
    {
        Vector3Int startGridPos = grid.WorldToCell(transform.position);
        Vector3Int endGridPos = grid.WorldToCell(target.position);
        List<Vector3Int> pathInt = pathfind.GetPath(startGridPos, endGridPos);
        path = new List<Vector3>();
        
        foreach (var pos in pathInt)
        {
            path.Add(tilemap.CellToWorld(pos) + tilemap.cellSize / 2);
            //Instantiate(pathIndicator, tilemap.CellToWorld(pos) + tilemap.cellSize / 2, Quaternion.identity);
        }
    }

    private void RotateByFaceDirection()
    {
        if (faceDirection == -1)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    private void SetFaceDirection(int newFaceDirection)
    {
        Debug.Log("Changing Face Direction");
        this.faceDirection = newFaceDirection;
        RotateByFaceDirection();
    }

    private IEnumerator FollowPath()
    {
        if (path.Count > 0)
        {
            Vector3 currentWaypoint = path[0];
            targetIndex = 0;  // Certifique-se de inicializar targetIndex

            while (true)
            {
                Debug.Log((transform.position - currentWaypoint).sqrMagnitude < (0.05f)*(0.05f));
                if ((transform.position - currentWaypoint).sqrMagnitude < (0.05f)*(0.05f))
                {
                    targetIndex++;
                    if (targetIndex >= path.Count)
                    {
                        Debug.Log("Break");
                        yield break;
                    }

                    currentWaypoint = path[targetIndex];
                }

                Vector3 waypointDirection = (currentWaypoint - transform.position).normalized;
                int newFaceDirection = waypointDirection.x < 0 ? -1 : 1;
                if (newFaceDirection != faceDirection)
                {
                    //SetFaceDirection(newFaceDirection);
                }

                currentTarget = currentWaypoint;
                var target = new Vector2(currentWaypoint.x, transform.position.y);
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                yield return null;  // Espera até o próximo frame
            }
        }
    }

    private void HandleMovement()
    {
        if (path != null)
        {
            Vector3 targetPosition = path[targetIndex];
            if (Vector3.Distance(transform.position, targetPosition) > 1f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;
                float distanceBefore = Vector3.Distance(transform.position, targetPosition);

                transform.position = new Vector3(transform.position.x, transform.position.y,0) + new Vector3(moveDir.x,0,0) * speed * Time.deltaTime;
            }
            else
            {
                targetIndex ++;
                if (targetIndex >= path.Count)
                {
                    StopMoving();
                }
            }
        }
    }

    private void StopMoving()
    {
        path = null;
    }

    void MoveTowardsPlayer()
    {
        Vector3Int startGridPos = grid.WorldToCell(transform.position);
        Vector3Int endGridPos = grid.WorldToCell(enemyEyes.GetPlayerPosition());

        List<Vector3Int> pathInt = pathfind.GetPath(startGridPos, endGridPos);
        path = new List<Vector3>();

        foreach (var pos in pathInt)
        {
            path.Add(tilemap.CellToWorld(pos) + tilemap.cellSize / 2);
        }
        StopCoroutine(FollowPath());
        StartCoroutine(FollowPath());
    }

    void Update()
    {
        if (enemyEyes.SeePlayer())
        {
            MoveTowardsPlayer();
        }
    }
}
