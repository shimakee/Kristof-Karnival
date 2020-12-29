using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialNode
{

    public bool CanPass;
    public Vector2Int Coordinates;
    public Vector3 WorldPosition;
    public IList<TrialNode> Neighbors { get; private set; }
    public TrialNode ParentNode { get; set; }
    public int Fcost { get { return Gcost + Hcost; } }
    public int baseCost, Hcost, Gcost;

    public void SetNeighbors(IList<TrialNode> tilenodes)
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
    TrialNode()
    {
        Neighbors = new List<TrialNode>();
    }
    TrialNode(int x, int y)
        : this()
    {
        Coordinates = new Vector2Int(x, y);
    }
    TrialNode(Vector2Int coordinates)
        : this ()
    {
        Coordinates = coordinates;
    }

    public TrialNode(int x, int y, bool canPass, Vector3 position)
        : this(x, y)
    {
        CanPass = canPass;
        WorldPosition = position;
    }
    public TrialNode(Vector2Int coordinates, bool canPass, Vector3 position)
        : this(coordinates)
    {
        CanPass = canPass;
        WorldPosition = position;
    }
}
