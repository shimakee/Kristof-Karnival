using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;


public class NodeGridmapComponent : MonoBehaviour
{
    //TODO: create different verions
    //One inherits or required tilemap component

    //Another that just gets screen coordinates or world position and translates it to current grid on screen
    //therefore never needing more than a grid that fits the screen
    [Header("Tilemap")]
    [SerializeField] Tilemap tilemap;

    [Header("Map details:")]
    [Range(1, 50)] [SerializeField] float mapWidth = 10;
    [Range(1, 50)][SerializeField] int numberOfColumns = 5;
    [Space(10)]

    [Range(1, 50)] [SerializeField] float mapHeight = 10;
    [Range(1, 50)][SerializeField] int numberOfRows = 5; //use resolution as well?

    [Header("Tile details:")]
    [Range(0, 50)][SerializeField] float tileHeight = 0;

    [SerializeField] bool isIsometric;
    [SerializeField] bool allowDiagonalMoves;

    public Node[,] Map { get { return _map; } }


    Vector2 _mapSize;
    Node[,] _map;
    Vector3 _tileSize;

    private void Awake()
    {
        _mapSize = new Vector2(mapWidth, mapHeight);
        _map = new Node[numberOfColumns, numberOfRows];
        _tileSize = AdjustTileSize();
        GenerateNodesOnMap(numberOfColumns, numberOfRows);
        EstablishNodeConnectionForAll(numberOfColumns, numberOfRows);

        if (tilemap == null)
            Debug.LogError("no tilemap attached");

    }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
    }

    void OnDrawGizmos()
    {
        DrawTilesOnMap();
    }

    #region Utility Methods
    public void DrawTilesOnMap()
    {
        if (isIsometric)
        {
            DrawOnMap(ComputePositionForIsometric);
        }
        else
        {
            DrawOnMap(ComputePositionForCartesean);
        }
    }
    public void CheckForObstacles()
    {
        CheckForObstacles(numberOfColumns, numberOfRows);
    }
    public Vector2Int ScreenToGrid(Vector2 position)
    {
        int x = (int) ((position.x / Screen.width) * numberOfColumns);
        int y = (int)((position.y / Screen.height) * numberOfRows);

        return new Vector2Int(x, y);
    }

    public Vector2Int WorldToGrid(Vector3 position)
    {
        Vector3 adjustedPosition = position - transform.position;

        Debug.Log($"adjusted position: {adjustedPosition}");
        int x = Mathf.FloorToInt(adjustedPosition.x / _tileSize.x);
        int y = Mathf.FloorToInt(adjustedPosition.y / _tileSize.y);

        return new Vector2Int(x, y);
    }

    public Vector2 GridToWord(Vector2Int position)
    {
        float xOffset = _tileSize.x / 2;
        float yOffset = _tileSize.y / 2;
        float x = (position.x * _tileSize.x) + xOffset;
        float y = (position.y * _tileSize.y) + yOffset;

        Vector3 adjustedPosition = new Vector3(x, y, 0) + transform.position;

        return adjustedPosition;
    }

    public Vector2 ToNearestTilePosition(Vector2 position)
    {
        var gridPos = WorldToGrid(position);
        var worldPos = GridToWord(gridPos);

        return worldPos;
    }

    #endregion

    #region Core Methods
    Vector3 AdjustTileSize()
    {
        float tileSizeX = _mapSize.x / numberOfColumns;
        float tileSizeY = _mapSize.y / numberOfRows;

        _tileSize = new Vector3(tileSizeX, tileSizeY, tileHeight);
        return _tileSize;
    }

    Vector3 ComputePositionForCartesean(Vector2 position, Vector3 tilesize)
    {
        float xOffset = tilesize.x / 2;
        float yOffset = tilesize.y / 2;

        var xPos = transform.position.x + xOffset + (tilesize.x * position.x);
        var yPos = transform.position.y + yOffset + (tilesize.y * position.y);
        var zPos = transform.position.z;

        Vector3 pos = new Vector3(xPos, yPos, zPos);
        return pos;
    }

    Vector3 ComputePositionForIsometric(Vector2 position, Vector3 tilesize)
    {
        float xOffset = tilesize.x / 2;
        float yOffset = tilesize.y / 2;

        Vector2 vectorOffset = RevertFromIsometric(new Vector2(xOffset, yOffset));

        var xPos = vectorOffset.x + (tilesize.x * position.x);
        var yPos = vectorOffset.y + (tilesize.y * position.y);


        Vector2 origin = RevertFromIsometric(transform.position);
        Vector2 pos = new Vector2(xPos, yPos) + origin;
        return TransformToIsometric(pos);
    }

    void GenerateNodesOnMap(int numberOfColumns, int numberOfRows)
    {
        for (int x = 0; x < numberOfColumns; x++)
        {
            for (int y = 0; y < numberOfRows; y++)
            {
                Vector2 pos;
                if (isIsometric)
                {
                    pos = ComputePositionForIsometric(new Vector2(x, y), _tileSize);
                }
                else
                {
                    pos = ComputePositionForCartesean(new Vector2(x, y), _tileSize);
                }

                bool canPass = !Physics2D.BoxCast(pos, new Vector2(_tileSize.x * .9f, _tileSize.y * .9f), 0, new Vector2(0, 0), 0, LayerMask.GetMask("Tile Obstacles"));

                _map[x, y] = new Node(x, y, canPass);
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

                bool isMoveDiagonal = (x > 0 || x < 0) && (y > 0 || y < 0);
                if (!allowDiagonalMoves && isMoveDiagonal)
                    continue;

                int xCoordinate = current.x + x;
                int yCoordinate = current.y + y;
                bool isBeyondMap = xCoordinate < 0 || xCoordinate >= Map.GetLength(0) ||
                                    yCoordinate < 0 || yCoordinate >= Map.GetLength(1);
                if (isBeyondMap)
                    continue;

                Node neighbor = Map[xCoordinate, yCoordinate];
                if (neighbor == null)
                    continue;

                current.Neighbors.Add(neighbor);
            }
        }
    }
    Vector2 TransformToIsometric(Vector2 vector)
    {
        float x = (vector.x * .7f) + (vector.y * .7f);
        float y = (vector.x * -.7f) + (vector.y * .7f);

        return new Vector2(x, y);
    }

    Vector2 RevertFromIsometric(Vector2 vector)
    {
        float x = (vector.x * .7f) + (vector.y * -.7f);
        float y = (vector.x * .7f) + (vector.y * .7f);

        return new Vector2(x, y);
    }


    void DrawOnMap(Func<Vector2, Vector3, Vector3> computation )
    {
        _mapSize = new Vector2(mapWidth, mapHeight);
        _map = new Node[numberOfColumns, numberOfRows];
        _tileSize = AdjustTileSize();

        for (int x = 0; x < numberOfColumns; x++)
        {
            for (int y = 0; y < numberOfRows; y++)
            {
                //var pos = ComputePosition(x, y, (int)transform.position.z, _tileSize);
                //var pos = ComputePositionForIsometric(x, y, _tileSize);
                //var pos = computation(new Vector2(x, y), _tileSize);
                //bool canPass = !Physics2D.BoxCast(pos, new Vector2(_tileSize.x * .9f, _tileSize.y * .9f), 0, new Vector2(0, 0), 0, LayerMask.GetMask("Tile Obstacles"));
                var node = _map[x, y];
                Vector3 pos = (node != null ) ? new Vector3(node.x, node.y) : computation(new Vector2(x, y), _tileSize);
                bool canPass = (node != null) ?  node.CanPass : !Physics2D.BoxCast(pos, new Vector2(_tileSize.x * .9f, _tileSize.y * .9f), 0, new Vector2(0, 0), 0, LayerMask.GetMask("Tile Obstacles"));


                //Gizmos - can remove
                if (canPass)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireCube(pos, new Vector3(_tileSize.x * .9f, _tileSize.y * .9f, _tileSize.z));
                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(pos, new Vector3(_tileSize.x * .9f, _tileSize.y * .9f, _tileSize.z));
                }
            }
        }
    }

    void CheckForObstacles(int numberOfColumns, int numberOfRows)
    {
        Debug.Log("Checking obstacles");

        for (int x = 0; x < numberOfColumns; x++)
        {
            for (int y = 0; y < numberOfRows; y++)
            {
                var pos = ComputePositionForCartesean(new Vector2(x, y), _tileSize);
                bool canPass = !Physics2D.BoxCast(pos, new Vector2(_tileSize.x * .9f, _tileSize.y * .9f), 0, new Vector2(0, 0));

                var node = _map[x, y];

                if (node != null)
                    node.CanPass = canPass;
            }
        }
    }

    #endregion

}
