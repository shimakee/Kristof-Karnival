using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathfinding
{
    List<Node> FindPath(Node origin, Node destination);
}

public interface IGridNodeMap
{
    void CheckForTileCollisions();
    Vector2Int WorldToGrid(Vector3 position);
    Vector3 GridToWorld(Vector2Int position);
    Vector3 GridToWorld(int x, int y);
    Vector3 ToNearestTilePosition(Vector3 position);
}
