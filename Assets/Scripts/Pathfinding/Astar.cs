using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar : IPathfinding
{
    public List<Node> FindPath(Node origin, Node destination)
    {
        List<Node> unvisitedList = new List<Node>();
        HashSet<Node> visitedList = new HashSet<Node>();

        unvisitedList.Add(origin);

        while (unvisitedList.Count > 0)
        {
            Node currentNode = FindLowestCost(unvisitedList);
            visitedList.Add(currentNode);
            unvisitedList.Remove(currentNode);
            EvaluateNeighbors(currentNode, destination, visitedList, unvisitedList);
        }

        return GetNodesPath(origin, destination);
    }

    List<Node> GetNodesPath(Node origin, Node destination)
    {
        if (destination == null || origin == null)
            Debug.LogError("Argument cannot be null");

        List<Node> path = new List<Node>();
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

    Node FindLowestCost(IList<Node> nodes)
    {
        if (nodes == null || nodes.Count <= 0)
            return null;

        Node lowestCost = null;
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

    void EvaluateNeighbors(Node current,Node destination, HashSet<Node> visitedList, List<Node> unvisitedList)
    {
        for (int i = 0; i < current.Neighbors.Count; i++)
        {
            var neighbor = current.Neighbors[i];

            if (!neighbor.CanPass || visitedList.Contains(neighbor))
                continue;

            ComputeCostToNeighbor(current, neighbor, destination, unvisitedList);

            if (!unvisitedList.Contains(neighbor))
                unvisitedList.Add(neighbor);
        }
    }

    int ComputeDistance(Node first, Node second)
    {
        var xDiff = first.Coordinates.x - second.Coordinates.x;
        var yDiff = first.Coordinates.y - second.Coordinates.y;

        var cost = Mathf.Sqrt((xDiff * xDiff) + (yDiff * yDiff));

        return (int)(cost * 10);
    }

    void ComputeCostToNeighbor(Node current, Node neighbor, Node destination, List<Node> unvisitedList)
    {
        var distance = ComputeDistance(current, neighbor);
        var gcost = distance + current.Gcost + neighbor.baseCost;
        var hcost = ComputeDistance(neighbor, destination);

        if(gcost < neighbor.Gcost || !unvisitedList.Contains(neighbor))
        {
            neighbor.Gcost = gcost;
            neighbor.Hcost = hcost;

            neighbor.ParentNode = current;
        }
    }
}
