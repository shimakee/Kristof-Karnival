using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar : MonoBehaviour
{
    public List<TrialNode> findPath(TrialNode origin, TrialNode destination)
    {
        List<TrialNode> unvisitedList = new List<TrialNode>();
        HashSet<TrialNode> visitedList = new HashSet<TrialNode>();

        unvisitedList.Add(origin);

        while (unvisitedList.Count > 0)
        {
            //TrialNode currentNode = UnvisitedList[0];
            //get lowest cost
            TrialNode currentNode = FindLowestCost(unvisitedList);
            //visit current node
            visitedList.Add(currentNode);
            unvisitedList.Remove(currentNode);
            //check neighbors
            EvaluateNeighbors(currentNode, destination, visitedList, unvisitedList);
        }

        return GetNodesPath(origin, destination);
    }

    List<TrialNode> GetNodesPath(TrialNode origin, TrialNode destination)
    {
        if (destination == null || origin == null)
            Debug.LogError("Argument cannot be null");

        List<TrialNode> path = new List<TrialNode>();
        var currentNode = destination;

        while (currentNode != origin || currentNode.Coordinates != origin.Coordinates)
        {
            if (!path.Contains(currentNode))
                path.Add(currentNode);

            if (currentNode.ParentNode == null)
                break;

            currentNode = currentNode.ParentNode;

        } 

        path.Reverse();

        return path;
    }

    TrialNode FindLowestCost(IList<TrialNode> nodes)
    {
        if (nodes == null || nodes.Count <= 0)
            return null;

        TrialNode lowestCost = null;
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

    void EvaluateNeighbors(TrialNode current,TrialNode destination, HashSet<TrialNode> visitedList, List<TrialNode> unvisitedList)
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

    int ComputeDistance(TrialNode first, TrialNode second)
    {
        var xDiff = first.Coordinates.x - second.Coordinates.x;
        var yDiff = first.Coordinates.y - second.Coordinates.y;

        var cost = Mathf.Sqrt((xDiff * xDiff) + (yDiff * yDiff));

        return (int)(cost * 10);
    }

    void ComputeCostToNeighbor(TrialNode current, TrialNode neighbor, TrialNode destination, List<TrialNode> unvisitedList)
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
