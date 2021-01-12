using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap<T> : MonoBehaviour, IGridMap<T> 
{
    //inspector modifications
    [Header("Map details:")]
    [Range(0, 50)] [SerializeField] protected float mapWidth = 10;
    [Range(0, 50)] [SerializeField] protected int numberOfColumns = 5;
    [Space(10)]

    [Range(0, 50)] [SerializeField] protected float mapHeight = 10;
    [Range(0, 50)] [SerializeField] protected int numberOfRows = 5; //use resolution as well?
    [Space(10)]

    [Range(0, 50)] [SerializeField] protected float mapDepth = 10;
    [Range(0, 50)] [SerializeField] protected int numberOfDepth = 5; //use resolution as well?
    [Space(10)]

    [Header("Grid angle:")]
    [Range(-90, 90)] [SerializeField] protected float xAngle = 0;
    [Range(-90, 90)] [SerializeField] protected float yAngle = 0;
    [Range(-90, 90)] [SerializeField] protected float zAngle = 0;

    public T this[int x, int y, int z] { get { return Map[x, y, z]; } }
    public T[,,] Map { get { return _map; } }
    public Vector3 MapSize { get { return _mapSize; } }
    public Vector3Int MapVolumeCount { get { return _mapVolumeCount; } }
    public Vector3 TileSize { get { return _tileSize; } }
    public Vector3 Angle { get { return _angle; } }

    protected T[,,] _map;
    protected Vector3 _mapSize;
    protected Vector3Int _mapVolumeCount;
    protected Vector3 _tileSize;
    protected Vector3 _angle = Vector3.zero;

    #region Unity methods

    private void Awake()
    {
        CreateGrid();
    }
    #endregion

    #region Private Methods

    protected virtual void CreateGrid()
    {
        _mapSize = new Vector3(mapWidth, mapHeight, mapDepth);
        _mapVolumeCount = new Vector3Int(numberOfColumns, numberOfRows, numberOfDepth);
        _tileSize = AdjustTileSize();
        _angle = new Vector3(xAngle, yAngle, zAngle);
        _map = new T[numberOfColumns, numberOfRows, numberOfDepth];
    }
    #endregion


    #region Utility Methods
    public T GetGridObject(Vector3Int coordinates)
    {
        return _map[coordinates.x, coordinates.y, coordinates.z];
    }
    public Vector3 GridToWorld(Vector3Int coordinates)
    {
        return GridToWorld(coordinates.x, coordinates.y, coordinates.z);
    }
    public Vector3 GridToWorld(int x, int y, int z)
    {
        float xOffset = _tileSize.x / 2;
        float yOffset = _tileSize.y / 2;
        float zOffset = _tileSize.z / 2;

        float xPos = (x * _tileSize.x) + xOffset;
        float yPos = (y * _tileSize.y) + yOffset;
        float zPos = (z * _tileSize.z) + zOffset;

        Vector3 adjustedPosition = new Vector3(xPos, yPos, zPos);

        adjustedPosition = RotateVector(adjustedPosition, _angle);
        adjustedPosition = adjustedPosition + transform.position;
        //if (isIsometric)

        return adjustedPosition;
    }
    public Vector3Int WorldToGrid(Vector3 position)
    {
        Vector3 adjustedPosition = position;

        //if (isIsometric)
        adjustedPosition = adjustedPosition - transform.position;
        //adjustedPosition = RevertRotationOnVector(position, -_angle);
        adjustedPosition = RotateVector(position, -_angle);

        int x = Mathf.FloorToInt((adjustedPosition.x / _tileSize.x));
        int y = Mathf.FloorToInt((adjustedPosition.y / _tileSize.y));
        int z = Mathf.FloorToInt((adjustedPosition.z / _tileSize.z));

        return new Vector3Int(x, y, z);


    }
    public Vector3 GetNearestTilePosition(Vector3 position)
    {
        var gridPos = WorldToGrid(position);
        var worldPos = GridToWorld(gridPos);

        return worldPos;
    }
    #endregion

    #region Adjustments

    protected float DegreesToRadians(float degrees)
    {
        return (float)(degrees * (3.14 / 180));
    }

    protected float RadiansToDegrees(float radians)
    {
        return (float)(radians * 3.14);
    }

    protected virtual Vector3 AdjustTileSize()
    {
        float tileSizeX = _mapSize.x / numberOfColumns;
        float tileSizeY = _mapSize.y / numberOfRows;
        float tileSizeZ = _mapSize.z / numberOfDepth;

        _tileSize = new Vector3(tileSizeX, tileSizeY, tileSizeZ);
        return _tileSize;
    }

    protected Vector3 RotateVector(Vector3 vector, Vector3 angle)
    {
        var referenceVector = new Vector3(vector.x, vector.y, vector.z);
        //transform in z
        var transformedInZ = Transform2DVector(new Vector2(referenceVector.x, referenceVector.y), angle.z);
        referenceVector.x = transformedInZ.x;
        referenceVector.y = transformedInZ.y;
        //transform in y
        var transformedInY = Transform2DVector(new Vector2(referenceVector.z, referenceVector.x), angle.y);
        referenceVector.z = transformedInY.x;
        referenceVector.x = transformedInY.y;
        //transform in x
        var transformedInX = Transform2DVector(new Vector2(referenceVector.y, referenceVector.z), angle.x);
        referenceVector.y = transformedInX.x;
        referenceVector.z = transformedInX.y;

        return referenceVector;
    }

    protected Vector2 Transform2DVector(Vector2 vector, float angle) //TODO: make based on angle
    {
        float cosin = Mathf.Cos(DegreesToRadians(angle));
        float sin = Mathf.Sin(DegreesToRadians(angle));

        float x = (vector.x * cosin) + (vector.y * -sin);
        float y = (vector.x * sin) + (vector.y * cosin);

        return new Vector2(x, y);
    }
    #endregion

}
