using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;


public class NodeGridmapComponent : MonoBehaviour
{
    [Header("Map details:")]
    [Range(1, 50)] [SerializeField] float mapWidth = 10;
    [Range(1, 50)] [SerializeField] int numberOfColumns = 5;
    [Space(10)]

    [Range(1, 50)] [SerializeField] float mapHeight = 10;
    [Range(1, 50)] [SerializeField] int numberOfRows = 5; //use resolution as well?

    [Header("Tile details:")]
    [Range(0, 50)] [SerializeField] float tileHeight = 0;


    [SerializeField] bool isIsometric;
    [SerializeField] bool allowDiagonalMoves;

    [Header("Mask To check collisions")]
    [SerializeField] LayerMask[] layerMasks;

    //public Node[,] Map { get { return _map; } }
    public Vector3 TilSize { get { return _tileSize; } }


    Vector2 _mapSize;
    public Node[,] Map;
    Vector3 _tileSize;

    private void Awake()
    {
        _mapSize = new Vector2(mapWidth, mapHeight);
        Map = new Node[numberOfColumns, numberOfRows];
        GenerateNodesOnMap(numberOfColumns, numberOfRows);
        EstablishNodeConnectionForAll(numberOfColumns, numberOfRows);
        _tileSize = AdjustTileSize();

        //if (tilemap == null)
        //    Debug.LogError("no tilemap attached");

    }

    private void Start()
    {
        CheckForObstacles();
    }

    private void FixedUpdate()
    {
        //CheckForObstacles();
    }

    void OnDrawGizmos()
    {
        DrawTilesOnMap();
    }

    #region Utility Methods
    public void ResetMap()
    {
        for (int x = 0; x < numberOfColumns; x++)
        {
            for (int y = 0; y < numberOfRows; y++)
            {
                Map[x, y].ClearNode();
            }
        }
    }
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

                //bool canPass = !Physics2D.BoxCast(pos, new Vector2(_tileSize.x * .9f, _tileSize.y * .9f), 0, new Vector2(0, 0), 0, LayerMask.GetMask("Tile Obstacles"));
                bool canPass = CanPass(pos, layerMasks);

                Map[x, y] = new Node(x, y, canPass);

                //if(Map[x,y] != null)
                //    Debug.Log($"Generated node for x {x} y{y}:");
            }
        }
    }

    void EstablishNodeConnectionForAll(int numberOfColumns, int numberOfRows)
    {
        for (int x = 0; x < numberOfColumns; x++)
        {
            for (int y = 0; y < numberOfRows; y++)
            {
                EstablishGridNodeConnection(Map[x, y]);
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
        for (int x = 0; x < numberOfColumns; x++)
        {
            for (int y = 0; y < numberOfRows; y++)
            {
                if (Map == null)
                    return;

                var node = Map[x, y];

                Vector3 pos = computation(new Vector2(node.x, node.y), _tileSize);
                //Vector3 pos = (node != null ) ? computation(new Vector2(node.x, node.y), _tileSize) : computation(new Vector2(x, y), _tileSize);
                //bool canPass = (node != null) ?  node.CanPass : !Physics2D.BoxCast(pos, new Vector2(_tileSize.x * .9f, _tileSize.y * .9f), 0, new Vector2(0, 0), 0, LayerMask.GetMask("Tile Obstacles"));


                //Gizmos - can remove
                if (node.CanPass)
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
        for (int x = 0; x < numberOfColumns; x++)
        {
            for (int y = 0; y < numberOfRows; y++)
            {
                Vector3 pos;
                if(isIsometric)
                    pos = ComputePositionForIsometric(new Vector2(x, y), _tileSize);
                else
                    pos = ComputePositionForCartesean(new Vector2(x, y), _tileSize);

                bool canPass = CanPass(pos, layerMasks);

                var node = Map[x, y];

                if (node != null)
                    node.CanPass = canPass;
                else
                    Debug.Log("node was null on obstacle check", this);

            }
        }
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
