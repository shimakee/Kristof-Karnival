using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.UI;


public class Movement
{
    public float Speed { get; set; }
    public Vector3 DesiredPosition { get; set; }
    public Vector3 Direction { get; set; }
    public Vector3 LastDirectionFacing { get; set;}

    public Movement(float speed)
    {
        Speed = speed;
    }

    #region Public utility method
    public void MovePosition(Rigidbody rb, Vector3 position)
    {
        rb.MovePosition(position);
    }
    public void MovePosition2D(Rigidbody2D rb, Vector2 position)
    {
            rb.MovePosition(position);
    }
    public void AddForce(Rigidbody rb, Vector3 direction, float deltaTime)
    {
        rb.AddForce(Calculate(direction, deltaTime));
    }

    public void AddForce2D(Rigidbody2D rb, Vector2 direction, float deltaTime)
    {
        rb.AddForce(Calculate2D(direction, deltaTime));
    }
    public void AssignVelocity(Rigidbody rb, Vector3 direction)
    {
        rb.velocity = direction * Speed;
    }

    public void AssignVelocity2D(Rigidbody2D rb, Vector2 direction)
    {
        rb.velocity = direction * Speed;
    }
    public void MoveTowards(Rigidbody rb, Vector3 destination, float deltaTime)
    {
        rb.MovePosition(Vector3.MoveTowards(rb.position, destination, deltaTime * Speed));
    }

    public void MoveTowards2D(Rigidbody2D rb, Vector2 destination, float deltaTime)
    {
        //rb.position = Vector2.MoveTowards(rb.position, destination, Speed * deltaTime);
        rb.MovePosition(Vector2.MoveTowards(rb.position, destination, deltaTime * Speed));
    }

    public Vector3 SwitchZandY(Vector3 direction)
    {
        return new Vector3(direction.x, direction.z, direction.y);
    }

    public void RotateForwardDirectionInYAxis(Rigidbody rb, Vector3 direction)
    {
        //direction.y = rb.transform.position.y;

        var angle = Vector3.SignedAngle(rb.transform.forward, direction.normalized, rb.position);

        rb.transform.Rotate(0f, angle, 0f);
    }
    #endregion

    #region Private methods

    Vector2 Calculate2D(Vector2 direction, float deltaTime)
    {
        Vector2 newDirection = direction * Speed * deltaTime;
        
        return newDirection;
    }

    Vector3 Calculate(Vector3 direction, float deltaTime)
    {
        Vector3 newDirection = direction * Speed * deltaTime;
        
        return newDirection;
    }

    #endregion
}
