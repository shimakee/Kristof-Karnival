﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridNode : MonoBehaviour, IGridNodeMap
{
    //temp
    public GameObject player;
    public GameObject target;
    Astar _pathfinding;

    [Header("Map details:")]
    [Range(1, 50)] [SerializeField] float mapWidth = 10;
    [Range(1, 50)] [SerializeField] int numberOfColumns = 5;
    [Space(10)]

    [Range(1, 50)] [SerializeField] float mapHeight = 10;
    [Range(1, 50)] [SerializeField] int numberOfRows = 5; //use resolution as well?

    [Header("Tile details:")]
    [Range(0, 50)] [SerializeField] float tileHeight = 0;

    [SerializeField] bool isIsometric;
    [SerializeField] bool allowDiagonalConnections;

    [Header("Mask To check collisions")]
    [SerializeField] LayerMask collisionMask;

    public Node[,] Map { get { return _map; } }
    public Vector3 TileSize { get { return _tileSize; } }


    Vector2 _mapSize;
    Node[,] _map;
    Vector3 _tileSize;
    Dictionary<int, HashSet<int>> EquivalencyList = new Dictionary<int, HashSet<int>>();

    #region Unity Methods

    private void Awake()
    {
        _mapSize = new Vector2(mapWidth, mapHeight);
        _tileSize = AdjustTileSize();
        CreateConnectedGrid(numberOfColumns, numberOfRows);

        //temp
        _pathfinding = new Astar();
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckForTileCollisions();
        DetermineConnectedness();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if(_map != null)
        {
            DrawOnMap();

            var origin = WorldToGrid(player.transform.position);
            var destination = WorldToGrid(target.transform.position);
            var path = _pathfinding.FindPath(_map[origin.x, origin.y], _map[destination.x, destination.y]);
            foreach (var node in _map)

            {

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


    #endregion

    #region Drawing methods
    public void DrawOnMap()
    {
        for (int x = 0; x < numberOfColumns; x++)
        {
            for (int y = 0; y < numberOfRows; y++)
            {
                if (Map == null)
                    return;

                var node = Map[x, y];

                if (node.CanPass)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireCube(node.WorldPosition, new Vector3(_tileSize.x * .9f, _tileSize.y * .9f, _tileSize.z));
                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(node.WorldPosition, new Vector3(_tileSize.x * .9f, _tileSize.y * .9f, _tileSize.z));
                }
            }
        }
    }
    #endregion

    #region Utility Methods

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
    public Vector3 ToNearestTilePosition(Vector3 position)
    {
        var gridPos = WorldToGrid(position);
        var worldPos = GridToWorld(gridPos);

        return worldPos;
    }

    public void CheckForTileCollisions()
    {
        CheckForTileCollisions(numberOfColumns, numberOfRows);
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
                Vector3 position = GridToWorld(x, y);
                bool canPass = CanPass(position, collisionMask);

                _map[x, y] = new Node(x, y, canPass, position);
                _map[x, y].ConnectedValue = canPass ? 1 : 0;
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
                if (!allowDiagonalConnections && isMoveDiagonal)
                    continue;

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

   

    void CheckForTileCollisions(int numberOfColumns, int numberOfRows)
    {
        for (int x = 0; x < numberOfColumns; x++)
        {
            for (int y = 0; y < numberOfRows; y++)
            {
                Vector3 pos = GridToWorld(new Vector2Int(x, y));

                bool canPass = CanPass(pos, collisionMask);

                var node = Map[x, y];

                if (node != null)
                    node.CanPass = canPass;
                else
                    Debug.Log("node was null on obstacle check", this);

            }
        }
    }

    bool CanPass(Vector2 position, LayerMask layerMasks)
    {

        return !Physics2D.BoxCast(position, new Vector2(_tileSize.x * .9f, _tileSize.y * .9f), 0, Vector2.zero, 0, layerMasks);
    }

    #endregion

    #region Computation and adjustment methods
    Vector3 AdjustTileSize()
    {
        float tileSizeX = _mapSize.x / numberOfColumns;
        float tileSizeY = _mapSize.y / numberOfRows;

        _tileSize = new Vector3(tileSizeX, tileSizeY, tileHeight);
        return _tileSize;
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

    #endregion

    #region Connectedness
    void DetermineConnectedness()
    {
        DetermineConnectedness(numberOfColumns, numberOfRows);
    }
    void DetermineConnectedness(int column, int row)
    {
        int counter = 0;

        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                var current = _map[x, y];
                //if (current.ConnectedValue == 0)
                //    continue;

                //evaluate neighbors
                counter = EvaluateConnectedNeighbors(counter, current);
            }
        }

        ReduceEquivalencyList(EquivalencyList);

        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                var current = _map[x, y];

                current.ConnectedValue = LowestValueEquivalent(current.ConnectedValue, EquivalencyList);
            }
        }
    }

    int EvaluateConnectedNeighbors(int counter, Node current)
    {
        int separationCounter = 0;
        HashSet<int> neighborsValue = new HashSet<int>();
        for (int x = -1; x < 1; x++)
        {
            for (int y = -1; y < 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                bool isMoveDiagonal = (x > 0 || x < 0) && (y > 0 || y < 0);
                if (!allowDiagonalConnections && isMoveDiagonal)
                    continue;

                int xCoordinate = current.Coordinates.x + x;
                int yCoordinate = current.Coordinates.y + y;
                bool isBeyondMap = xCoordinate < 0 || xCoordinate >= _map.GetLength(0) ||
                                    yCoordinate < 0 || yCoordinate >= _map.GetLength(1);
                if (isBeyondMap)
                {
                    separationCounter++;
                    continue;
                }

                Node neighbor = _map[xCoordinate, yCoordinate];

                if (neighbor.ConnectedValue <= 0)
                {
                    separationCounter++;
                    continue;
                }

                neighborsValue.Add(neighbor.ConnectedValue);
            }
        }

        bool hasConflict = neighborsValue.Count > 1;

        if (current.ConnectedValue == 0 && !hasConflict)
            return counter;

        if (hasConflict) 
        {
            //resolve conflict //TODO: better to use heap since its already sorted
            int lowestInt = 0;
            var arrayInt = neighborsValue.ToArray();

            for (int i = 0; i < arrayInt.Length; i++)
            {
                if (i == 0)
                    lowestInt = arrayInt[i];

                if (arrayInt[i] < lowestInt)
                    lowestInt = arrayInt[i];
            }

            if (current.ConnectedValue != 0)
                current.ConnectedValue = lowestInt;

            if (!EquivalencyList.ContainsKey(lowestInt))
                EquivalencyList[lowestInt] = new HashSet<int>();

            EquivalencyList[lowestInt].UnionWith(neighborsValue);
        }
        else if (neighborsValue.Count == 1)
        {
            current.ConnectedValue = neighborsValue.First();
        }

        if ((separationCounter == 3 && allowDiagonalConnections) || (separationCounter == 2 && !allowDiagonalConnections))
        {
            counter++;
            current.ConnectedValue = counter;

            if (!EquivalencyList.ContainsKey(counter))
                EquivalencyList[counter] = new HashSet<int>();

            EquivalencyList[counter].Add(counter);
        }

        return counter;
    }

    int LowestValueEquivalent(int counter, Dictionary<int, HashSet<int>> list)
    {

        var lowestEquivalent = counter;

        for (int i = list.Count -1; i >= 0; i--)
        {
            var item = list.ElementAt(i);
            if (item.Value.Contains(lowestEquivalent))
                lowestEquivalent = item.Key;
        }

        return lowestEquivalent;
    }

    void ReduceEquivalencyList(Dictionary<int, HashSet<int>> list)
    {
        for (int index = 0; index < list.Count; index++)
        {
            var item = list.ElementAt(index);

            for (int i = list.Count -1; i >= 0; i--)
            {
                var toCheck = list.ElementAt(i);

                if (item.Value.Contains(toCheck.Key) && item.Value != toCheck.Value)
                {
                    list[item.Key].UnionWith(list[toCheck.Key]);
                    list.Remove(toCheck.Key);
                }
            }
        }
    }

    #endregion
    }