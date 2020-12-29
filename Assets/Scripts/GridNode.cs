using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    public GameObject player;
    public GameObject target;

    [Header("Map details:")]
    [Range(1, 50)] [SerializeField] float mapWidth = 10;
    [Range(1, 50)] [SerializeField] int numberOfColumns = 5;
    [Space(10)]

    [Range(1, 50)] [SerializeField] float mapHeight = 10;
    [Range(1, 50)] [SerializeField] int numberOfRows = 5; //use resolution as well?

    [Header("Tile details:")]
    [Range(0, 50)] [SerializeField] float tileHeight = 0;


    //[SerializeField] bool isIsometric;
    //[SerializeField] bool allowDiagonalMoves;

    [Header("Mask To check collisions")]
    [SerializeField] LayerMask[] collisionMask;

    public Node[,] Map { get { return _map; } }
    public Vector3 TileSize { get { return _tileSize; } }


    Vector2 _mapSize;
    Node[,] _map;
    Vector3 _tileSize;
    Astar _pathfinding;
    AstarPathfinding astarPathfinding;

    private void Awake()
    {
        _mapSize = new Vector2(mapWidth, mapHeight);
        _tileSize = AdjustTileSize();
        CreateConnectedGrid(numberOfColumns, numberOfRows);
        _pathfinding = new Astar();
        astarPathfinding = new AstarPathfinding();
        //EstablishNodeConnectionForAll(numberOfColumns, numberOfRows);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if(_map != null)
        {
            var origin = WorldToGrid(player.transform.position);
            var destination = WorldToGrid(target.transform.position);
            //var path = _pathfinding.FindPath(_map[origin.x, origin.y], _map[destination.x, destination.y]);
            var path = astarPathfinding.FindPath(_map[origin.x, origin.y], _map[destination.x, destination.y]);
            foreach (var node in _map)
            {
                Gizmos.color = (node.CanPass) ? Color.blue : Color.red;
                Gizmos.DrawWireCube(node.WorldPosition, _tileSize * .9f);

                if (node.Coordinates == WorldToGrid(player.transform.position))
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(node.WorldPosition, _tileSize / 2);
                }

                if (path.Contains(node))
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawCube(node.WorldPosition, _tileSize / 2);
                }
            }
        }
    }

   


    #region Utility Methods

    public Vector3 GridToWorld(Vector2Int coordinates)
    {
        return GridToWord(coordinates.x, coordinates.y);
    }
    public Vector3 GridToWord(int x, int y)
    {
        float xOffset = _tileSize.x / 2;
        float yOffset = _tileSize.y / 2;
        float xPos = (x * _tileSize.x) + xOffset;
        float yPos = (y * _tileSize.y) + yOffset;

        Vector3 adjustedPosition = new Vector3(xPos, yPos, 0) + transform.position;

        return adjustedPosition;
    }

    public Vector2Int ScreenToGrid(Vector2 position)
    {
        int x = (int)((position.x / Screen.width) * numberOfColumns);
        int y = (int)((position.y / Screen.height) * numberOfRows);

        return new Vector2Int(x, y);
    }

    public Vector2Int WorldToGrid(Vector3 position)
    {
        Vector3 adjustedPosition = position - transform.position;

        int x = Mathf.FloorToInt(adjustedPosition.x / _tileSize.x);
        int y = Mathf.FloorToInt(adjustedPosition.y / _tileSize.y);

        return new Vector2Int(x, y);
    }
    public Vector2 ToNearestTilePosition(Vector2 position)
    {
        var gridPos = WorldToGrid(position);
        var worldPos = GridToWorld(gridPos);

        return worldPos;
    }

    #endregion

    #region Private Methods
    void CreateConnectedGrid(int column, int rows)
    {
        CreateGrid(column, rows);
        EstablishNodeConnectionForAll(column, rows);
    }

    void CreateGrid(int column, int rows)
    {
        _map = new Node[column, rows];

        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 position = GridToWord(x, y);
                bool canPass = CanPass(position, collisionMask);

                _map[x, y] = new Node(x, y, canPass, position);
            }
        }
    }
    void EstablishNodeConnectionForAll(int numberOfColumns, int numberOfRows)
    {
        for (int x = 0; x < numberOfColumns; x++)
        {
            for (int y = 0; y < numberOfRows; y++)
            {
                EstablishGridNodeConnection(_map[x, y]);
            }
        }
    }

    void EstablishGridNodeConnection(Node current)
    {
        if (current == null)
            Debug.LogError("Node cannot be null in order to assign neighbors");

        current.Neighbors.Clear();

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                //bool isMoveDiagonal = (x > 0 || x < 0) && (y > 0 || y < 0);
                //if (!allowDiagonalMoves && isMoveDiagonal)
                //    continue;

                int xCoordinate = current.Coordinates.x + x;
                int yCoordinate = current.Coordinates.y + y;
                bool isBeyondMap = xCoordinate < 0 || xCoordinate >= Map.GetLength(0) ||
                                    yCoordinate < 0 || yCoordinate >= Map.GetLength(1);
                if (isBeyondMap)
                    continue;

                Node neighbor = _map[xCoordinate, yCoordinate];
                current.Neighbors.Add(neighbor);
            }
        }
    }

    Vector3 AdjustTileSize()
    {
        float tileSizeX = _mapSize.x / numberOfColumns;
        float tileSizeY = _mapSize.y / numberOfRows;

        _tileSize = new Vector3(tileSizeX, tileSizeY, tileHeight);
        return _tileSize;
    }

    bool CanPass(Vector2 position, LayerMask[] layerMasks)
    {
        if (layerMasks == null)
            return true;
        if (layerMasks.Length <= 0)
            return true;

        foreach (var mask in layerMasks)
        {
            bool canPass = !Physics2D.BoxCast(position, new Vector2(_tileSize.x * .9f, _tileSize.y * .9f), 0, Vector2.zero, 0, mask);
            if (!canPass)
                return false;
        }

        return true;
    }

    #endregion
}
