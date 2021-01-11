using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Connectedness<T> where T : IConnectedNode<T>
{
    public int DetermineConnectedness(IConnectedNode<T>[,,] nodes)
    {
        int path = 0;
        path = ~path;
        return DetermineConnectedness(nodes, path);
    }
    public int DetermineConnectedness(IConnectedNode<T> [,,] nodes, LayerMask pathMask)
    {
        List<IConnectedNode<T>> nodesVisited = new List<IConnectedNode<T>>();
        int connectedGraphCounter = 0;

        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                for (int z = 0; z < nodes.GetLength(2); z++)
                {

                    IConnectedNode<T> node = nodes[x, y, z];
                    bool canPass = node.CanPass(pathMask);
                        Debug.Log(canPass);
                    if (!nodesVisited.Contains(node) && canPass)
                    {
                        connectedGraphCounter++;
                        DepthFirstSearchConnectedNeighbors(node, connectedGraphCounter, nodesVisited, pathMask);
                    }
                }
            }
        }

        return connectedGraphCounter;
    }

    void DepthFirstSearchConnectedNeighbors(IConnectedNode<T> node,int connectedGraphValue, IList<IConnectedNode<T>> nodesVisited, LayerMask pathMask)
    {
        Stack<IConnectedNode<T>> nodeStack = new Stack<IConnectedNode<T>>();

        nodeStack.Push(node);
        nodesVisited.Add(node);

        while(nodeStack.Count > 0)
        {
            int visitedNeighbors = 0;

            IConnectedNode<T> currentNode = nodeStack.Peek();
            if (currentNode.CanPass(pathMask))
                currentNode.ConnectedValue = connectedGraphValue;

            for (int i = 0; i < currentNode.Neighbors.Count; i++)
            {
                IConnectedNode<T> neighbor = currentNode.Neighbors[i];

                bool isVisited = nodesVisited.Contains(neighbor);
                if (!currentNode.CanPass(pathMask) || isVisited || neighbor == null)
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
