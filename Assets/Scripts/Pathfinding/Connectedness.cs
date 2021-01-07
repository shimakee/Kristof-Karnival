using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Connectedness
{
    public void DetermineConnectedness(Node[,] nodes)
    {
        List<Node> nodesVisited = new List<Node>();
        int connectedGraphCounter = 0;

        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                var node = nodes[x, y];

                if(!nodesVisited.Contains(node) && node.CanPass)
                {
                    connectedGraphCounter++;
                    DepthFirstSearchConnectedNeighbors(node, connectedGraphCounter, nodesVisited);
                }
            }
        }
    }

    void DepthFirstSearchConnectedNeighbors(Node node,int connectedGraphValue, List<Node> nodesVisited)
    {
        Stack<Node> nodeStack = new Stack<Node>();

        nodeStack.Push(node);
        nodesVisited.Add(node);

        while(nodeStack.Count > 0)
        {
            Node currentNode = nodeStack.Peek();
            int visitedNeighbors = 0;

            if(currentNode.CanPass)
                currentNode.ConnectedValue = connectedGraphValue;

            for (int i = 0; i < currentNode.Neighbors.Count; i++)
            {
                Node neighbor = currentNode.Neighbors[i];

                bool isVisited = nodesVisited.Contains(neighbor);
                if (!neighbor.CanPass || isVisited)
                    visitedNeighbors++;
                else
                    nodeStack.Push(neighbor);

                if(!isVisited)
                    nodesVisited.Add(neighbor);
            }

            bool isAllNeighborsVisited = visitedNeighbors == currentNode.Neighbors.Count;
            if (isAllNeighborsVisited)
                nodeStack.Pop();
        }
    }
}
