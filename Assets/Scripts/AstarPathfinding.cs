using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarPathfinding : IPathfinding
{
    public HashSet<Node> VisitedNodes { get { return _visitedNodes; } }
    public List<Node> UnvisitedNodes { get { return _unvisitedNodes; } }

    HashSet<Node> _visitedNodes;
    List<Node> _unvisitedNodes;

    public AstarPathfinding()
    {
        _visitedNodes = new HashSet<Node>();
        _unvisitedNodes = new List<Node>();
    }

    public List<Node> FindPath(Node origin, Node destination)
    {
        if (origin == null || destination == null)
            Debug.LogError("Null parameters given");

        _visitedNodes.Clear();
        _unvisitedNodes.Clear();

        _unvisitedNodes.Add(origin);

        bool hasPath = EvaluatePathToDestination(origin, destination, _unvisitedNodes);

        if (hasPath)
            return GetNodesPath(origin, destination);
        return null; // return null or path with empty?
    }


    


    #region Private Core Methods

    List<Node> GetNodesPath(Node origin, Node destination)
    {
        if (destination == null)
            Debug.LogError("Argument cannot be null");

        List<Node> path = new List<Node>();
        var currentNode = destination;

        do
        {
            if (!path.Contains(currentNode))
                path.Add(currentNode);

            if (currentNode.ParentNode == null)
                break;

            currentNode = currentNode.ParentNode;
        } while (currentNode != origin || currentNode.Coordinates != origin.Coordinates);

        path.Reverse();

        return path;
    }

    bool EvaluatePathToDestination(Node origin, Node destination, IList<Node> unvisitedNodes)
    {
        if (origin == null || destination == null)
            Debug.LogError("destination and current node must not be null");
        if (origin == destination || origin.Coordinates == destination.Coordinates)
            return true;


        Node lowestCost = FindLowestCost(unvisitedNodes);


        _visitedNodes.Add(origin);
        _unvisitedNodes.Remove(origin);
        EvaluateNeighbors(origin, destination);

        if (unvisitedNodes.Count <= 0)
        {
            Debug.Log("No path to destination.");
            return false;
        }

        return EvaluatePathToDestination(lowestCost, destination, unvisitedNodes);
    }

    void EvaluateNeighbors(Node current, Node destination)
    {
        for (int i = 0; i < current.Neighbors.Count; i++)
        {
            var neighbor = current.Neighbors[i];

            if (!neighbor.CanPass || _visitedNodes.Contains(neighbor))
                continue;

            GetNeighborPathAndCost(current, neighbor, destination);

            if (!_unvisitedNodes.Contains(neighbor))
                _unvisitedNodes.Add(neighbor);
        }
    }


    Node FindLowestCost(IList<Node> nodes)
    {
        if (nodes == null || nodes.Count <= 0)
            return null;

        Node lowestCost = null;

        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];

            if (lowestCost == null)
            {
                lowestCost = node;
                continue;
            }

            bool isNodeLowerCost = lowestCost.Fcost > node.Fcost || (lowestCost.Fcost == node.Fcost && lowestCost.Hcost > node.Hcost);
            if (isNodeLowerCost)
                lowestCost = node;
        }

        return lowestCost;
    }

    int ComputeDistance(Node first, Node second)
    {
        var xDiff = first.Coordinates.x - second.Coordinates.x;
        var yDiff = first.Coordinates.y - second.Coordinates.y;

        var cost = Mathf.Sqrt((xDiff * xDiff) + (yDiff * yDiff));

        return (int)(cost * 10);
    }

    void GetNeighborPathAndCost(Node current, Node neighbor, Node destination)
    {
            var distance = ComputeDistance(current, neighbor);
            int gCost = distance + current.Gcost + neighbor.baseCost;
            neighbor.Hcost = ComputeDistance(neighbor, destination);
        

            if (gCost < neighbor.Gcost || !_unvisitedNodes.Contains(neighbor))
            {
                neighbor.Gcost = gCost;
                neighbor.ParentNode = current;
            }
    }

    #endregion
}
