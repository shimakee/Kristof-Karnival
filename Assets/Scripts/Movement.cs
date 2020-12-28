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
    //public float HoldTime { get; set; }


    //Vector2 _direction;
    //float _pressTime;
    //bool isHeld;

    public Movement(float speed)
    {
        Speed = speed;
        //HoldTime = holdtime;
    }

    #region Public utility method
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
    public Vector2 RestrainDiagonalDirection(Vector2 direction)
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

    //public void ProcessMovement2D(Rigidbody2D rb, float fixedDeltaTime)
    //{
    //    if (isHeld)
    //        _pressTime += fixedDeltaTime;

    //    if (_pressTime >= HoldTime)
    //    {
    //        _pressTime = 0;
    //        Debug.Log($"Execute {_pressTime}");
    //        DesiredPosition = rb.position + _direction;
    //        Debug.Log($"Change desiredPosition {DesiredPosition}, direction {_direction}");
    //    }

    //    //Debug.Log($"moving {DesiredPosition}");
    //    MoveTowards2D(rb, DesiredPosition, fixedDeltaTime);
    //}

    //public void ProcessMovementInput(InputAction.CallbackContext ctx)
    //{
    //    if (ctx.started)
    //    {
    //        isHeld = true;
    //        Debug.Log($"started {isHeld}");
    //    }

    //    if (ctx.performed)
    //    {
    //        _direction = ctx.ReadValue<Vector2>().normalized;
    //        _direction = RestrainDiagonalDirection(_direction);
    //        Debug.Log($"performed {_direction}");

    //    }

    //    if (ctx.canceled)
    //    {
    //        isHeld = false;
    //        Debug.Log($"cancelled {isHeld}");
    //    }
    //}

    #endregion

    #region Private methods

    Vector2 Calculate(Vector2 direction, float deltaTime)
    {
        float horizontal = direction.x * Speed * deltaTime;
        float vertical = direction.y * Speed * deltaTime;


        return new Vector2(horizontal, vertical);
    }

    #endregion
}
