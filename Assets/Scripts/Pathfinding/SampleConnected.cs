using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleConnected : GridMap<IUnityPathNode>, IConnectedGridMap2D<IUnityPathNode>
{
    //[Header("Mask To check collisions")]
    [SerializeField] LayerMask pathBlockMask;

    //[Header("Checks:")]
    [SerializeField] bool allowDiagonalConnections;
    [SerializeField] bool enableFixedUpdateCollisionCheck = false;
    [SerializeField] bool enableDrawOnSelection = false;

    public IUnityPathNode this[int x, int y] { get { return _map2D[x,y]; } }
    new public IUnityPathNode this[int x, int y, int z] { get { return _map2D[x, y]; } }
    new public IUnityPathNode[,] Map { get { return _map2D; } }
    new public Vector2 TileSize { get { return _tileSize; } }
    new public Vector2 MapSize { get { return _mapSize; } }
    new public Vector2Int MapVolumeCount { get { return (Vector2Int)_mapVolumeCount; } }

    IUnityPathNode[,] _map2D;

    #region Unity Methods

    //private void Awake()
    //{
    //    CreateGrid();
    //}

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
            _mapSize = new Vector2(mapWidth, mapHeight);
            _mapVolumeCount = new Vector3Int(numberOfColumns, numberOfRows, 0);
            _tileSize = AdjustTileSize();
            //_angle.x = xAngle;
            //_angle.y = yAngle;
            _angle.z = zAngle;

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

    #region Utilities Method
    public void CheckForTileCollisions()
    {
        CheckForTileCollisions(_map2D, pathBlockMask);
    }

    public void EstablishConnection()
    {
        EstablishNodeConnectionForAll(_map2D);
    }

    public IUnityPathNode GetGridObject(Vector2Int coordinates)
    {
        return GetGridObject(new Vector3Int(coordinates.x, coordinates.y, 0));
    }

    new public Vector2 GetNearestTilePosition(Vector3 position)
    {
        return GetNearestTilePosition(position);
    }

    public Vector2 GridToWorld(Vector2Int coordinates)
    {
        return GridToWorld(new Vector3Int(coordinates.x, coordinates.y, 0));
    }

    public Vector2 GridToWorld(int x, int y)
    {
        return GridToWorld(x, y, 0);
    }

    new public Vector2Int WorldToGrid(Vector3 position)
    {
        return (Vector2Int)WorldToGrid(new Vector3(position.x, position.y, 0));
    }
    #endregion

    #region Private Methods
    protected override void CreateGrid()
    {
        _mapSize = new Vector2(mapWidth, mapHeight);
        _mapVolumeCount = new Vector3Int(numberOfColumns, numberOfRows, 0);
        _tileSize = AdjustTileSize();
        _map2D = CreateAndPopulateConnectedGrid(numberOfColumns, numberOfRows);
        EstablishNodeConnectionForAll(_map2D);
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

                map[x, y] = new Node(x, y, 0, pathBlock, position);
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
    protected override Vector3 AdjustTileSize()
    {
        float tileSizeX = _mapSize.x / numberOfColumns;
        float tileSizeY = _mapSize.y / numberOfRows;

        _tileSize = new Vector3(tileSizeX, tileSizeY, 0);
        return _tileSize;
    }
    #endregion
}
