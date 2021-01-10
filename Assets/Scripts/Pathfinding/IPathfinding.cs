using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public interface IConnectedNode<T> where T : IConnectedNode<T>
    {
        int ConnectedValue { get; set; }
        IList<T> Neighbors { get; }
        bool CanPass { get; set; }
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
        IList<IUnityPathNode> FindPath(IUnityPathNode origin, IUnityPathNode destination);
    }

    public interface IGridNodeMap
    {
        Node[,] Map { get; } //TO change IUnityPathNode
        Vector3 TileSize { get; }
        void CheckForTileCollisions();
        Vector2Int WorldToGrid(Vector3 position);
        Vector3 GridToWorld(Vector2Int position);
        Vector3 GridToWorld(int x, int y);
        Vector3 ToNearestTilePosition(Vector3 position);
    }

    public interface IGridMap<T>
    {
        T[,,] Map { get; }
        Vector3 TileSize { get; }
        T GetGridObject(Vector3Int coordinates);
        Vector3Int WorldToGrid(Vector3 position);
        Vector3 GridToWorld(Vector3Int coordinates);
        Vector3 GridToWorld(int x, int y, int z);
        Vector3 GetNearestTilePosition(Vector3 position);

    }

    public interface IConnectedGridMap<T> : IGridMap<T> where T : IConnectedNode<T> 
    {
        void CheckForTileCollisions();
    }

    public interface IConnectedGridMap2D<T> where T : IConnectedNode<T>
    {
        T[,] Map { get; }
        void CheckForTileCollisions();
        Vector2 TileSize { get; }
        T GetGridObject(Vector2Int coordinates);
        Vector2Int WorldToGrid(Vector3 position);
        Vector3 GridToWorld(Vector2Int coordinates);
        Vector3 GridToWorld(int x, int y);
        Vector3 GetNearestTilePosition(Vector3 position);
    }
