using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Caiiru.Utils;

public class Grid <TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    [SerializeField]private int[,] gridArray;
    [SerializeField]private Node[,] nodeArray;
    public Vector3 originPosition;

    public GameObject textCanvas;

    public Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        
    }
    private void Start()
    {
        CreateGrid(4,4,4, new Vector3(5,2,0));
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetValue(Utils.GetMouseWorldPosition(),56);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(GetValue(Utils.GetMouseWorldPosition()));
        }
    }

    public void CreateGrid(int width, int height, float cellSize, Vector3 originPosition)
    {
        gridArray = new int[width, height];
        nodeArray = new Node[width, height];
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            { 
                /*
                var instance = Instantiate(textCanvas, GetWorldPosition(x,y) + new Vector3(cellSize, cellSize,0) * .5f, Quaternion.identity);
                instance.transform.SetParent(this.transform);
                nodeArray[x, y] = instance.GetComponent<Node>();
                
                if (instance.GetComponentInChildren<TextMeshProUGUI>() != null)
                {
                    
                    instance.GetComponentInChildren<TextMeshProUGUI>().fontSize = 2f;
                    instance.GetComponentInChildren<TextMeshProUGUI>().text = gridArray[x, y].ToString(); 
                }
                */
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x,y +1 ), Color.white,100f);
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x+1,y  ),Color.white,100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0,height),GetWorldPosition(width,height),Color.white,100f);
        Debug.DrawLine(GetWorldPosition(width,0),GetWorldPosition(width,height),Color.white,100f);
        SetValue(2,1,56);
        
    }

    

    private void SetValue(int x, int y, int value)
    { 
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            nodeArray[x,y].SetNumber(value);
        }
      
    }
    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x,y,value);
    }

    public int GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
            return gridArray[x, y];
        else
            return 0;

    }

    public int GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }
    

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);

    }

    

    private Vector3 GetWorldPosition(int x, int y)
    {
       
        return new Vector3(x, y) * cellSize + originPosition;
    }
}
