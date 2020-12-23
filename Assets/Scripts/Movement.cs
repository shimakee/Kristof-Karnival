using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    public float Speed;

    public Movement(float speed)
    {
        Speed = speed;
    }

   public Vector2 Calculate(float x, float y, float deltaTime)
    {
        float horizontal = x * Speed * deltaTime;
        float vertical = y * Speed * deltaTime;


        return new Vector2(horizontal, vertical);
    }
}
