using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap2D : MonoBehaviour, IConnectedGridMap2D<IUnityPathNode>
{
    //inspector modifications
    [Header("Map details:")]
    [Range(1, 50)] [SerializeField] float mapWidth = 10;
    [Range(1, 50)] [SerializeField] int numberOfColumns = 5;
    [Space(10)]

    [Range(1, 50)] [SerializeField] float mapHeight = 10;
    [Range(1, 50)] [SerializeField] int numberOfRows = 5; //use resolution as well?

    [Range(0, 90)] [SerializeField] float angle = 0;

    [Header("Tile details:")]
    [Range(0, 50)] [SerializeField] float tileHeight = 0;

    [SerializeField] bool isIsometric;
    [SerializeField] bool allowDiagonalConnections;

    [Header("Mask To check collisions")]
    [SerializeField] LayerMask maskToCheckForCollision;

    public Vector2 TileSize { get { return _tileSize; } }
    public IUnityPathNode[,] Map { get { return _map; } }

    Vector2 _mapSize;
    Vector2 _tileSize;
    IUnityPathNode[,] _map;

    #region Unity Mehtods
    private void Awake()
    {
        _mapSize = new Vector2(mapWidth, mapHeight);
        _tileSize = AdjustTileSize();
        _map = CreateConnectedGrid(numberOfColumns, numberOfRows);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion

    #region Utility Methods
    public IUnityPathNode GetGridObject(Vector2Int coordinates)
    {
        return _map[coordinates.x, coordinates.y];
    }
    public void CheckForTileCollisions()
    {
        CheckForTileCollisions(_map, maskToCheckForCollision);
    }

    public Vector3 GridToWorld(Vector2Int coordinates)
    {
        return GridToWorld(coordinates.x, coordinates.y);
    }
    public Vector3 GridToWorld(int x, int y)
    {
        float xOffset = _tileSize.x / 2;
        float yOffset = _tileSize.y / 2;
        float xPos = (x * _tileSize.x) + xOffset;
        float yPos = (y * _tileSize.y) + yOffset;

        Vector3 adjustedPosition = new Vector3(xPos, yPos, 0) + transform.position;

        if (isIsometric)
            adjustedPosition = TransformToIsometric(adjustedPosition);

        return adjustedPosition;
    }
    public Vector2Int WorldToGrid(Vector3 position)
    {
        Vector3 adjustedPosition = position;

        if (isIsometric)
            adjustedPosition = RevertFromIsometric(position);

        adjustedPosition = adjustedPosition - transform.position;

        int x = Mathf.FloorToInt(adjustedPosition.x / _tileSize.x);
        int y = Mathf.FloorToInt(adjustedPosition.y / _tileSize.y);

        return new Vector2Int(x, y);


    }
    public Vector3 GetNearestTilePosition(Vector3 position)
    {
        var gridPos = WorldToGrid(position);
        var worldPos = GridToWorld(gridPos);

        return worldPos;
    }
    #endregion

    #region Private Methods
    IUnityPathNode[,] CreateConnectedGrid(int column, int rows)
    {
        var map = CreateGrid(column, rows);
        EstablishNodeConnectionForAll(map);
        return map;
    }

    IUnityPathNode[,] CreateGrid(int column, int rows)
    {
        var map = new Node[column, rows];

        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 position = GridToWorld(x, y);
                bool canPass = CanPass(position, maskToCheckForCollision);

                map[x, y] = new Node(x, y, 0, canPass, position);
                map[x, y].ConnectedValue = canPass ? 1 : 0;
            }
        }

        return map;
    }

    void EstablishNodeConnectionForAll(IUnityPathNode[,] map)
    {
        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                EstablishGridNodeConnection(map[x, y] , map);
            }
        }
    }

    void EstablishGridNodeConnection(IUnityPathNode current, IUnityPathNode[,] map)
    {
        if (current == null)
            Debug.LogError("Node cannot be null in order to assign neighbors");

        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);

        current.Neighbors.Clear();

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                bool isMoveDiagonal = (x > 0 || x < 0) && (y > 0 || y < 0);
                if (!allowDiagonalConnections && isMoveDiagonal)
                    continue;

                int xCoordinate = current.Coordinates.x + x;
                int yCoordinate = current.Coordinates.y + y;
                bool isBeyondMap = xCoordinate < 0 || xCoordinate >=  mapWidth ||
                                    yCoordinate < 0 || yCoordinate >= mapHeight;
                if (isBeyondMap)
                    continue;

                IUnityPathNode neighbor = map[xCoordinate, yCoordinate];

                //if (neighbor.CanPass && current.CanPass)
                current.Neighbors.Add(neighbor);
            }
        }
    }



    void CheckForTileCollisions(IUnityPathNode[,] map, LayerMask layerMask)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                Vector3 pos = GridToWorld(new Vector2Int(x, y));

                bool canPass = CanPass(pos, layerMask);

                var node = map[x, y];

                if (node != null)
                    node.CanPass = canPass;
                else
                    Debug.Log("node was null on obstacle check", this);

            }
        }
    }

    bool CanPass(Vector2 position, LayerMask layerMask)
    {
        return !Physics2D.BoxCast(position, new Vector2(_tileSize.x * .9f, _tileSize.y * .9f), 0, Vector2.zero, 0, layerMask);
    }
    #endregion

    #region Adjustment Methods
    float DegreesToRadians(float degrees)
    {
        return (float)(degrees * (3.14 / 180));
    }

    float RadiansToDegrees(float radians)
    {
        return (float)(radians * 3.14);
    }

    Vector3 AdjustTileSize()
    {
        float tileSizeX = _mapSize.x / numberOfColumns;
        float tileSizeY = _mapSize.y / numberOfRows;

        _tileSize = new Vector3(tileSizeX, tileSizeY, tileHeight);
        return _tileSize;
    }

    Vector2 TransformToIsometric(Vector2 vector) //TODO: make based on angle
    {
        float cosin = Mathf.Cos(DegreesToRadians(angle));
        float sin = Mathf.Sin(DegreesToRadians(angle));

        Debug.Log($"cos {cosin}");
        Debug.Log($"sin {sin}");


        float x = (vector.x * cosin) + (vector.y * sin);
        float y = (vector.x * -sin) + (vector.y * cosin);

        return new Vector2(x, y);
    }

    Vector2 RevertFromIsometric(Vector2 vector) //TODO: make based on angle
    {
        float cosin = Mathf.Cos(DegreesToRadians(angle));
        float sin = Mathf.Sin(DegreesToRadians(angle));

        Debug.Log($"cos rever {cosin}");
        Debug.Log($"sin rever {sin}");

        float x = (vector.x * cosin) + (vector.y * -sin);
        float y = (vector.x * sin) + (vector.y * cosin);

        return new Vector2(x, y);
    }
    #endregion
}
