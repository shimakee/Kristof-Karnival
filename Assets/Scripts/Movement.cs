using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;


public class Movement
{
    public float Speed { get; }
    public Vector3 DesiredPosition { get; set; }
    public Movement(float speed)
    {
        Speed = speed;
    }

    Vector2 Calculate(Vector2 direction, float deltaTime)
    {
        float horizontal = direction.x * Speed * deltaTime;
        float vertical = direction.y * Speed * deltaTime;


        return new Vector2(horizontal, vertical);
    }

    public void MovePosition2D(Rigidbody2D rb, Vector2 position)
    {
            rb.MovePosition(position);
    }

    public void AddForce2D(Rigidbody2D rb, Vector2 direction, float deltaTime)
    {
        rb.AddForce(Calculate(direction, deltaTime));
    }

    public void AssignVelocity2D(Rigidbody2D rb, Vector2 direction)
    {
        rb.velocity = direction * Speed;
    }

    public void MoveTowards2D(Rigidbody2D rb, Vector2 destination, float deltaTime)
    {
        rb.position = Vector2.MoveTowards(rb.position, destination, Speed * deltaTime);
    }

    public void MoveToDesiredPosition2D(Rigidbody2D rb, float deltaTime)
    {
        if (DesiredPosition == null)
            Debug.LogError("Desired position cannot be null.");

        rb.position = Vector2.MoveTowards(rb.position, DesiredPosition, Speed * deltaTime);
    }
}
