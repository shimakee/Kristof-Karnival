using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathfinding
{
    List<Node> FindPath(Node origin, Node destination);
}
