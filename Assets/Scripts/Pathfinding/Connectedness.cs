using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Connectedness
{

    //create value counter

    //pick a node => make it current
    //if value is 0 - move on to next node
    //if value > 0 
    //evaluate current node with its neighbors
    //if separated increment (neighbors value is 0) value counter ++
    //if conflict - make note of conflict in equivalency list
    //assign connected lowest value

    //add current to stack
    //pick a neighbor
    Node[,] _map;
    bool allowDiagonalConnections;
    
}
