using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IUnityPathNode
{
    public LayerMask PathBlockMask { get; set; }
    public int ConnectedValue { get; set; }
    public IList<IUnityPathNode> Neighbors { get; private set; }
    public Vector3Int Coordinates { get; set; }
    public Vector3 WorldPosition { get; set; }
    public int BaseCost { get; set; }
    public int Fcost { get { return Gcost + Hcost; } }
    public int Hcost { get; set; }
    public int Gcost { get; set; }
    public IUnityPathNode ParentNode { get; set; }
    public bool CanPass()
    {
        return true;
    }
    public bool CanPass(LayerMask path)
    {
        bool isAllPathBlocked = (path & PathBlockMask) == path;
        if (isAllPathBlocked)
            return false;
        return true;
    }
    public void SetNeighbors(IList<IUnityPathNode> tilenodes)
    {
        if (tilenodes == null || tilenodes.Count <= 0)
            return;

        Neighbors = tilenodes;
    }

    public void ClearNode()
    {
        ParentNode = null;
        Gcost = 0;
        Hcost = 0;
    }
    Node()
    {
        Neighbors = new List<IUnityPathNode>();
    }
    Node(int x, int y, int z)
        : this()
    {
        Coordinates = new Vector3Int(x, y, z);
    }
    Node(Vector3Int coordinates)
        : this ()
    {
        Coordinates = coordinates;
    }

    public Node(int x, int y, int z, LayerMask pathBlock, Vector3 position)
        : this(x, y, z)
    {
        PathBlockMask = pathBlock;
        WorldPosition = position;
    }
    public Node(Vector3Int coordinates, LayerMask pathBlock, Vector3 position)
        : this(coordinates)
    {
        PathBlockMask = pathBlock;
        WorldPosition = position;
    }
}
