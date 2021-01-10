using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Connectedness<T> where T : IConnectedNode<T>
{
    public int DetermineConnectedness(IConnectedNode<T> [,] nodes)
    {
        List<IConnectedNode<T>> nodesVisited = new List<IConnectedNode<T>>();
        int connectedGraphCounter = 0;

        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                IConnectedNode<T> node = nodes[x, y];

                if (!nodesVisited.Contains(node) && node.CanPass)
                {
                    connectedGraphCounter++;
                    DepthFirstSearchConnectedNeighbors(node, connectedGraphCounter, nodesVisited);
                }
            }
        }

        return connectedGraphCounter;
    }

    void DepthFirstSearchConnectedNeighbors(IConnectedNode<T> node,int connectedGraphValue, IList<IConnectedNode<T>> nodesVisited)
    {
        Stack<IConnectedNode<T>> nodeStack = new Stack<IConnectedNode<T>>();

        nodeStack.Push(node);
        nodesVisited.Add(node);

        while(nodeStack.Count > 0)
        {
            int visitedNeighbors = 0;

            IConnectedNode<T> currentNode = nodeStack.Peek();
            if (currentNode.CanPass)
                currentNode.ConnectedValue = connectedGraphValue;

            for (int i = 0; i < currentNode.Neighbors.Count; i++)
            {
                IConnectedNode<T> neighbor = currentNode.Neighbors[i];

                bool isVisited = nodesVisited.Contains(neighbor);
                if (!neighbor.CanPass || isVisited)
                    visitedNeighbors++;
                else
                {
                    nodesVisited.Add(neighbor);
                    nodeStack.Push(neighbor);
                }
            }

            bool isAllNeighborsVisited = visitedNeighbors == currentNode.Neighbors.Count;
            if (isAllNeighborsVisited)
                nodeStack.Pop();
        }
    }
}
