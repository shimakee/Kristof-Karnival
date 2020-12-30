using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GridConnectedness
{
    //public Node[,] Map { get { return _map; } }

    Node[,] _map;
    bool _allowDiagonalConnection;
    Dictionary<int, HashSet<int>> _equivalencyList;


    public void DetermineConnectedness(Node[,] map, bool allowDiagonal)
    {
        _map = map;
        _allowDiagonalConnection = allowDiagonal;
        _equivalencyList = new Dictionary<int, HashSet<int>>();

        DetermineConnectedness(_map.GetLength(0), _map.GetLength(1));
    }
    void DetermineConnectedness(int column, int row)
    {
        int counter = 0;

        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                var current = _map[x, y];
                //if (current.ConnectedValue == 0)
                //    continue;

                //evaluate neighbors
                counter = EvaluateConnectedNeighbors(counter, current);
            }
        }

        ReduceEquivalencyList(_equivalencyList);

        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                var current = _map[x, y];

                current.ConnectedValue = LowestValueEquivalent(current.ConnectedValue, _equivalencyList);
            }
        }
    }

    int EvaluateConnectedNeighbors(int counter, Node current)
    {
        int separationCounter = 0;
        HashSet<int> neighborsValue = new HashSet<int>();
        for (int x = -1; x < 1; x++)
        {
            for (int y = -1; y < 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                bool isMoveDiagonal = (x > 0 || x < 0) && (y > 0 || y < 0);
                if (!_allowDiagonalConnection && isMoveDiagonal)
                    continue;

                int xCoordinate = current.Coordinates.x + x;
                int yCoordinate = current.Coordinates.y + y;
                bool isBeyondMap = xCoordinate < 0 || xCoordinate >= _map.GetLength(0) ||
                                    yCoordinate < 0 || yCoordinate >= _map.GetLength(1);
                if (isBeyondMap)
                {
                    separationCounter++;
                    continue;
                }

                Node neighbor = _map[xCoordinate, yCoordinate];

                if (neighbor.ConnectedValue <= 0)
                {
                    separationCounter++;
                    continue;
                }

                neighborsValue.Add(neighbor.ConnectedValue);
            }
        }

        bool hasConflict = neighborsValue.Count > 1;

        if (current.ConnectedValue == 0 && (!hasConflict || !_allowDiagonalConnection))
            return counter;

        if (hasConflict)
        {
            //resolve conflict //TODO: better to use heap since its already sorted
            int lowestInt = 0;
            var arrayInt = neighborsValue.ToArray();

            for (int i = 0; i < arrayInt.Length; i++)
            {
                if (i == 0)
                    lowestInt = arrayInt[i];

                if (arrayInt[i] < lowestInt)
                    lowestInt = arrayInt[i];
            }

            if (current.ConnectedValue != 0)
                current.ConnectedValue = lowestInt;

            if (!_equivalencyList.ContainsKey(lowestInt))
                _equivalencyList[lowestInt] = new HashSet<int>();

            _equivalencyList[lowestInt].UnionWith(neighborsValue);
        }
        else if (neighborsValue.Count == 1)
        {
            current.ConnectedValue = neighborsValue.First();
        }

        if ((separationCounter == 3 && _allowDiagonalConnection) || (separationCounter == 2 && !_allowDiagonalConnection))
        {
            counter++;
            current.ConnectedValue = counter;

            if (!_equivalencyList.ContainsKey(counter))
                _equivalencyList[counter] = new HashSet<int>();

            _equivalencyList[counter].Add(counter);
        }

        return counter;
    }

    int LowestValueEquivalent(int counter, Dictionary<int, HashSet<int>> list)
    {

        var lowestEquivalent = counter;

        for (int i = list.Count - 1; i >= 0; i--)
        {
            var item = list.ElementAt(i);
            if (item.Value.Contains(lowestEquivalent))
                lowestEquivalent = item.Key;
        }

        return lowestEquivalent;
    }

    void ReduceEquivalencyList(Dictionary<int, HashSet<int>> list)
    {
        for (int index = 0; index < list.Count; index++)
        {
            var item = list.ElementAt(index);

            for (int i = list.Count - 1; i >= 0; i--)
            {
                var toCheck = list.ElementAt(i);

                if (item.Value.Contains(toCheck.Key) && item.Value != toCheck.Value)
                {
                    list[item.Key].UnionWith(list[toCheck.Key]);
                    list.Remove(toCheck.Key);
                }
            }
        }
    }
}
