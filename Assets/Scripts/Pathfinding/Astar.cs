using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar : IPathfinding
{
    public IList<IUnityPathNode> FindPath(IUnityPathNode origin, IUnityPathNode destination, LayerMask pathMask)
    {
        List<IUnityPathNode> unvisitedList = new List<IUnityPathNode>();
        HashSet<IUnityPathNode> visitedList = new HashSet<IUnityPathNode>();

        unvisitedList.Add(origin);

        while (unvisitedList.Count > 0)
        {
            IUnityPathNode currentNode = FindLowestCost(unvisitedList);
            visitedList.Add(currentNode);
            unvisitedList.Remove(currentNode);
            EvaluateNeighbors(currentNode, destination, visitedList, unvisitedList, pathMask);
        }

        return GetNodesPath(origin, destination);
    }

    IList<IUnityPathNode> GetNodesPath(IUnityPathNode origin, IUnityPathNode destination)
    {
        if (destination == null || origin == null)
            Debug.LogError("Argument cannot be null");

        List<IUnityPathNode> path = new List<IUnityPathNode>();
        var currentNode = destination;

        while (currentNode != origin || currentNode.Coordinates != origin.Coordinates)
        {
            if (!path.Contains(currentNode))
                path.Add(currentNode);

            if (currentNode.ParentNode == null)
            {
                path.Clear();
                break;
            }

            currentNode = currentNode.ParentNode;
        } 

        path.Reverse();

        return path;
    }

    IUnityPathNode FindLowestCost(IList<IUnityPathNode> nodes)
    {
        if (nodes == null || nodes.Count <= 0)
            return null;

        IUnityPathNode lowestCost = null;
        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];

            //if (!node.CanPass)
            //    continue;

            if (lowestCost == null)
            {
                lowestCost = node;
                continue;
            }

            bool isNodeCostLower = node.Fcost < lowestCost.Fcost || (node.Fcost == lowestCost.Fcost && node.Hcost < lowestCost.Hcost);
            if (isNodeCostLower)
                lowestCost = node;

        }

        return lowestCost;
    }

    void EvaluateNeighbors(IUnityPathNode current, IUnityPathNode destination, HashSet<IUnityPathNode> visitedList, IList<IUnityPathNode> unvisitedList, LayerMask pathMask)
    {
        for (int i = 0; i < current.Neighbors.Count; i++)
        {
            var neighbor = current.Neighbors[i];

            if (!neighbor.CanPass(pathMask) || visitedList.Contains(neighbor))
                continue;

            ComputeCostToNeighbor(current, neighbor, destination, unvisitedList);

            if (!unvisitedList.Contains(neighbor))
                unvisitedList.Add(neighbor);
        }
    }

    int ComputeDistance(IUnityPathNode first, IUnityPathNode second)
    {
        var xDiff = first.Coordinates.x - second.Coordinates.x;
        var yDiff = first.Coordinates.y - second.Coordinates.y;

        var cost = Mathf.Sqrt((xDiff * xDiff) + (yDiff * yDiff));

        return (int)(cost * 10);
    }

    void ComputeCostToNeighbor(IUnityPathNode current, IUnityPathNode neighbor, IUnityPathNode destination, IList<IUnityPathNode> unvisitedList)
    {
        var distance = ComputeDistance(current, neighbor);
        var gcost = distance + current.Gcost + neighbor.BaseCost;
        var hcost = ComputeDistance(neighbor, destination);

        if(gcost < neighbor.Gcost || !unvisitedList.Contains(neighbor))
        {
            neighbor.Gcost = gcost;
            neighbor.Hcost = hcost;

            neighbor.ParentNode = current;
        }
    }
}
