using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public interface IConnectedNode<T> where T : IConnectedNode<T>
    {
        int ConnectedValue { get; set; }
        IList<T> Neighbors { get; }
        LayerMask PathBlockMask { get; set; }
        bool CanPass(LayerMask path);
        bool CanPass();
    }

    public interface IUnityPathNode : IConnectedNode<IUnityPathNode>
    {
    int BaseCost { get; set; }
    int Gcost { get; set; }
    int Hcost { get; set; }
    int Fcost { get;}
    Vector3Int Coordinates { get; set; }
    Vector3 WorldPosition { get; set; }
    IUnityPathNode ParentNode { get; set; }
    }
    public interface IPathfinding
    {
        List<IUnityPathNode> FindPath(IUnityPathNode origin, IUnityPathNode destination, LayerMask pathMask);
    }

    public interface IGridMap<T>
    {
        T[,,] Map { get; }
        T this[int x,int y, int z] { get; }
        Vector3 TileSize { get; }
        Vector3 MapSize { get; }
        Vector3Int MapVolumeCount { get; }
        T GetGridObject(Vector3Int coordinates);
        Vector3Int WorldToGrid(Vector3 position);
        Vector3 GridToWorld(Vector3Int coordinates);
        Vector3 GridToWorld(int x, int y, int z);
        Vector3 GetNearestTilePosition(Vector3 position);
    }
    public interface IConnectedGridMap<T>: IGridMap<T> where T : IUnityPathNode
    {
        void CheckForTileCollisions();
        void EstablishConnection();
    }
public interface IConnectedGridMap2D<T> where T : IUnityPathNode
{
    T[,] Map { get; }
    T this[int x, int y] { get; }
    T GetGridObject(Vector2Int coordinates);
    Vector2 TileSize { get; }
    Vector2 MapSize { get; }
    Vector2Int MapVolumeCount { get; }
    Vector2Int WorldToGrid(Vector3 position);
    Vector2 GridToWorld(Vector2Int coordinates);
    Vector2 GridToWorld(int x, int y);
    Vector2 GetNearestTilePosition(Vector3 position);
    void CheckForTileCollisions();
    void EstablishConnection();
}
