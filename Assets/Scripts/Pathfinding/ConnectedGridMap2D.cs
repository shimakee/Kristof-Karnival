using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedGridMap2D : MonoBehaviour, IConnectedGridMap2D<IUnityPathNode>
{
    //inspector modifications
    [Header("Map details:")]
    [Range(0, 50)] [SerializeField] protected float mapWidth = 10;
    [Range(0, 50)] [SerializeField] protected int numberOfColumns = 5;
    [Space(10)]

    [Range(0, 50)] [SerializeField] protected float mapHeight = 10;
    [Range(0, 50)] [SerializeField] protected int numberOfRows = 5; //use resolution as well?
    [Space(10)]

    [Header("Grid angle:")]
    [Range(-90, 90)] [SerializeField] protected float angle = 0;

    //[Header("Mask To check collisions")]
    [SerializeField] LayerMask pathBlockMask;

    //[Header("Checks:")]
    [SerializeField] bool allowDiagonalConnections;
    [SerializeField] bool enableFixedUpdateCollisionCheck = false;
    [SerializeField] bool enableDrawOnSelection = false;

    public IUnityPathNode this[int x, int y] { get { return Map[x, y]; } }
    public IUnityPathNode[,] Map { get { return _map; } }
    public Vector2 MapSize { get { return _mapSize; } }
    public Vector2Int MapVolumeCount { get { return _mapVolumeCount; } }
    public Vector2 TileSize { get { return _tileSize; } }

    protected IUnityPathNode[,] _map;
    protected Vector2 _mapSize;
    protected Vector2Int _mapVolumeCount;
    protected Vector2 _tileSize;
    protected float _angle = 0;


    #region Unity Methods
    private void Awake()
    {
        CreateGrid();
    }

    private void FixedUpdate()
    {
        if (enableFixedUpdateCollisionCheck)
            CheckForTileCollisions();
    }
    private void OnDrawGizmosSelected()
    {
        if (enableDrawOnSelection)
        {
            for (int x = 0; x < numberOfColumns; x++)
            {
                for (int y = 0; y < numberOfRows; y++)
                {
                    if (Map != null)
                    {
                        var node = Map[x, y];

                        if (node.PathBlockMask > 0)
                        {
                            Gizmos.color = Color.red;
                        }
                        else
                            Gizmos.color = Color.white;
                    }

                    Gizmos.DrawWireCube(GridToWorld(x, y), _tileSize * .9f);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!enableDrawOnSelection)
        {
            _mapSize = new Vector3(mapWidth, mapHeight);
            _tileSize = AdjustTileSize();
            _angle = angle;

            for (int x = 0; x < numberOfColumns; x++)
            {
                for (int y = 0; y < numberOfRows; y++)
                {
                    Gizmos.DrawWireCube(GridToWorld(x, y), _tileSize);
                }
            }
        }
    }

    #endregion

    #region Utility Methods
    public void CheckForTileCollisions()
    {
        CheckForTileCollisions(_map, pathBlockMask);
    }

    public void EstablishConnection()
    {
        EstablishNodeConnectionForAll(_map);
    }
    public IUnityPathNode GetGridObject(Vector2Int coordinates)
    {
        return _map[coordinates.x, coordinates.y];
    }
    public Vector2 GridToWorld(Vector2Int coordinates)
    {
        return GridToWorld(coordinates.x, coordinates.y);
    }
    public Vector2 GridToWorld(int x, int y)
    {
        float xOffset = _tileSize.x / 2;
        float yOffset = _tileSize.y / 2;

        float xPos = (x * _tileSize.x) + xOffset;
        float yPos = (y * _tileSize.y) + yOffset;

        Vector2 adjustedPosition = new Vector2(xPos, yPos);

        adjustedPosition = Transform2DVector(adjustedPosition, _angle);
        adjustedPosition = adjustedPosition + (Vector2)transform.position;

        return adjustedPosition;
    }
    public Vector2Int WorldToGrid(Vector3 position)
    {
        Vector3 adjustedPosition = position;

        adjustedPosition = adjustedPosition - transform.position;
        adjustedPosition = Transform2DVector(position, -_angle);

        int x = Mathf.FloorToInt((adjustedPosition.x / _tileSize.x));
        int y = Mathf.FloorToInt((adjustedPosition.y / _tileSize.y));

        return new Vector2Int(x, y);


    }
    public Vector2 GetNearestTilePosition(Vector3 position)
    {
        var gridPos = WorldToGrid(position);
        var worldPos = GridToWorld(gridPos);

        return worldPos;
    }
    #endregion

    #region Private Methods
    void CreateGrid()
    {
        _mapSize = new Vector2(mapWidth, mapHeight);
        _mapVolumeCount = new Vector2Int(numberOfColumns, numberOfRows);
        _tileSize = AdjustTileSize();
        _angle = angle;
        _map = CreateAndPopulateConnectedGrid(numberOfColumns, numberOfRows);
        EstablishNodeConnectionForAll(_map);
    }

    IUnityPathNode[,] CreateAndPopulateConnectedGrid(int column, int rows)
    {
        Node[,] map = new Node[column, rows];

        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 position = GridToWorld(x, y);
                LayerMask pathBlock = CollisionCheck(position, pathBlockMask);

                var node = map[x, y] = new Node(x, y, 0, pathBlock, position);
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
                
                EstablishGridNodeConnection(map[x, y], map);
            }
        }
    }

    void EstablishGridNodeConnection(IUnityPathNode current, IUnityPathNode[,] map)
    {
        if (current == null || map == null)
            Debug.LogError("Current node or map cannot be null in order to assign neighbors");

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
                bool isBeyondMap = xCoordinate < 0 || xCoordinate >= mapWidth ||
                                    yCoordinate < 0 || yCoordinate >= mapHeight;
                if (isBeyondMap)
                    continue;

                IUnityPathNode neighbor = map[xCoordinate, yCoordinate];

                current.Neighbors.Add(neighbor);
            }
        }
    }

    void CheckForTileCollisions(IUnityPathNode[,] map, LayerMask pathBlockMask)
    {
        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3 pos = GridToWorld(x, y);

                LayerMask pathBlock = CollisionCheck(pos, pathBlockMask);

                var node = map[x, y];

                if (node != null)
                    node.PathBlockMask = pathBlock;
                else
                    Debug.Log("node was null on obstacle check", this);
            }
        }
    }
    LayerMask CollisionCheck(Vector2 position, LayerMask pathBlockMask)
    {
        LayerMask returnMask = 0;
        //var hit = Physics.OverlapBox(position, (_tileSize / 2) * .9f, Quaternion.identity, pathBlockMask);
        var hit = Physics2D.OverlapBoxAll(position, _tileSize * .9f, 0);

        foreach (var item in hit)
        {
            int layer = 1 << item.gameObject.layer;
            returnMask = returnMask | layer;
        }

        return returnMask;
    }

    #endregion

    #region Adjustments

    float DegreesToRadians(float degrees)
    {
        return (float)(degrees * (3.14 / 180));
    }

    float RadiansToDegrees(float radians)
    {
        return (float)(radians * 3.14);
    }

    protected virtual Vector3 AdjustTileSize()
    {
        float tileSizeX = _mapSize.x / numberOfColumns;
        float tileSizeY = _mapSize.y / numberOfRows;

        _tileSize = new Vector3(tileSizeX, tileSizeY);
        return _tileSize;
    }

    Vector2 Transform2DVector(Vector2 vector, float angle) //TODO: make based on angle
    {
        float cosin = Mathf.Cos(DegreesToRadians(angle));
        float sin = Mathf.Sin(DegreesToRadians(angle));

        float x = (vector.x * cosin) + (vector.y * -sin);
        float y = (vector.x * sin) + (vector.y * cosin);

        return new Vector2(x, y);
    }
    #endregion
}
