using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IConnectedNode
{
    int ConnectedValue { get; set; }
    IList<IConnectedNode> Neighbors { get; set; }
}

public interface IUnityPathNode : IConnectedNode
{
    int BaseCost { get; set; }
    int Gcost { get; set; }
    int Hcost { get; set; }
    int FCost { get; set; }
    LayerMask ObstacleLayers { get; set; }
    Vector3Int Coordinates { get; set; }
    Vector3 WorldPosition { get; set; }
    IUnityPathNode Parentode { get; set; }
    new IList<IUnityPathNode> Neighbors { get; set; }
}
public interface IPathfinding
{
    List<Node> FindPath(Node origin, Node destination);
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
