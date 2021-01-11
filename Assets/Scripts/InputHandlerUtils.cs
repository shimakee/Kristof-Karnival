using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandlerUtils
{
    public static Vector2 RemoveDiagonalInputDirection(Vector2 direction)
    {
        if (direction.magnitude < .2)
            return Vector2.zero;
        bool isAxisDominantX = (direction.y < .7 || direction.y > -.7) && (direction.x >= 0.7 || direction.x <= -.7);
        bool isAxisDominantY = (direction.x < .7 || direction.x > -.7) && (direction.y >= 0.7 || direction.y <= -.7);

        if (isAxisDominantY)
        {
            direction.x = 0;

            if (direction.y >= .7)
                direction.y = 1;
            else if (direction.y <= -.7)
                direction.y = -1;
            else
                direction.y = 0;
        }
        else if (isAxisDominantX)
        {
            direction.y = 0;

            if (direction.x >= .7)
                direction.x = 1;
            else if (direction.x <= -.7)
                direction.x = -1;
            else
                direction.x = 0;
        }
        else
        {
            direction = Vector2.zero;
        }

        return direction;
    }
}
