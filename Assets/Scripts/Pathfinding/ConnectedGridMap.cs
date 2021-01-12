using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedGridMap : GridMap<IUnityPathNode>, IConnectedGridMap<IUnityPathNode>
{
    //[Header("Mask To check collisions")]
    [SerializeField] LayerMask pathBlockMask;

    //[Header("Checks:")]
    [SerializeField] bool allowDiagonalConnections;
    [SerializeField] bool enableFixedUpdateCollisionCheck = false;
    [SerializeField] bool enableDrawOnSelection = false;

    #region Unity Mehtods
    //private void Awake()
    //{
    //    CreateGrid();
    //}

    private void Start()
    {
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
                    for (int z = 0; z < numberOfDepth; z++)
                    {

                        if (Map != null)
                        {
                            var node = Map[x, y, z];

                            if (node.PathBlockMask > 0)
                            {
                                Gizmos.color = Color.red;
                            }
                            else
                                Gizmos.color = Color.white;

                            
                        }

                        Gizmos.DrawWireCube(GridToWorld(x, y, z), _tileSize * .9f);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!enableDrawOnSelection)
        {
            _mapSize = new Vector3(mapWidth, mapHeight, mapDepth);
            _tileSize = AdjustTileSize();
            _angle.x = xAngle;
            _angle.y = yAngle;
            _angle.z = zAngle;

            for (int x = 0; x < numberOfColumns; x++)
            {
                for (int y = 0; y < numberOfRows; y++)
                {
                    for (int z = 0; z < numberOfDepth; z++)
                    {
                        Gizmos.DrawWireCube(GridToWorld(x, y, z), _tileSize);
                    }
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

    #endregion

    #region Private Methods

    protected override void CreateGrid()
    {
        _mapSize = new Vector3(mapWidth, mapHeight, mapDepth);
        _mapVolumeCount = new Vector3Int(numberOfColumns, numberOfRows, numberOfDepth);
        _tileSize = AdjustTileSize();
        _angle = new Vector3(xAngle, yAngle, zAngle);
        _map = CreateAndPopulateConnectedGrid(numberOfColumns, numberOfRows, numberOfDepth);
        EstablishNodeConnectionForAll(_map);
    }

    IUnityPathNode[,,] CreateAndPopulateConnectedGrid(int column, int rows, int depth)
    {
        Node[,,] map = new Node[column, rows, depth];

        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Vector3 position = GridToWorld(x, y, z);
                    LayerMask pathBlock = CollisionCheck(position, pathBlockMask);

                    var node = map[x, y, z] = new Node(x, y, z, pathBlock, position);
                }
            }
        }

        return map;
    }

    void EstablishNodeConnectionForAll(IUnityPathNode[,,] map)
    {
        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);
        int mapDepth = map.GetLength(2);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int z = 0; z < mapDepth; z++)
                {
                    EstablishGridNodeConnection(map[x, y, z], map);
                }
            }
        }
    }

    void EstablishGridNodeConnection(IUnityPathNode current, IUnityPathNode[,,] map)
    {
        if (current == null || map == null)
            Debug.LogError("Current node or map cannot be null in order to assign neighbors");

        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);
        int mapDepth = map.GetLength(2);

        current.Neighbors.Clear();

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                for (int z = -1; z < 2; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    bool isMoveDiagonal = (x > 0 || x < 0) && (y > 0 || y < 0) && (z > 0 || z < 0);
                    if (!allowDiagonalConnections && isMoveDiagonal)
                        continue;

                    int xCoordinate = current.Coordinates.x + x;
                    int yCoordinate = current.Coordinates.y + y;
                    int zCoordinate = current.Coordinates.z + z;
                    bool isBeyondMap = xCoordinate < 0 || xCoordinate >= mapWidth ||
                                        yCoordinate < 0 || yCoordinate >= mapHeight ||
                                        zCoordinate < 0 || zCoordinate >= mapDepth;
                    if (isBeyondMap)
                        continue;

                    IUnityPathNode neighbor = map[xCoordinate, yCoordinate, zCoordinate];

                    current.Neighbors.Add(neighbor);
                }
            }
        }
    }

    void CheckForTileCollisions(IUnityPathNode[,,] map, LayerMask pathBlockMask)
    {
        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);
        int mapDepth = map.GetLength(2);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int z = 0; z < mapDepth; z++)
                {
                    Vector3 pos = GridToWorld(x, y, z);

                    LayerMask pathBlock = CollisionCheck(pos, pathBlockMask);

                    var node = map[x, y, z];

                    if (node != null)
                        node.PathBlockMask = pathBlock;
                    else
                        Debug.Log("node was null on obstacle check", this);
                }

            }
        }
    }
    LayerMask CollisionCheck(Vector3 position, LayerMask pathBlockMask)
    {
        LayerMask returnMask = 0;
        var hit = Physics.OverlapBox(position, (_tileSize / 2) * .9f, Quaternion.identity, pathBlockMask);

        foreach (var item in hit)
        {
            int layer = 1 << item.gameObject.layer;
            returnMask = returnMask | layer;
        }

        return returnMask;
    }
    #endregion
}
