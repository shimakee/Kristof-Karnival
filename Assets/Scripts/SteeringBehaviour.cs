using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour
{

    public static Vector3 Steer(Vector3 desiredDirection, Vector3 currentVelocity, float maxSteeringForce)
    {
        var steering = desiredDirection - currentVelocity;
        steering = Vector3.ClampMagnitude(steering, maxSteeringForce);

        return steering;
    }

    public static Vector3 Seek(Vector3 targetPosition, IMoverComponent mover)
    {
        var desired = targetPosition - mover.CurrentPosition;
        desired = desired.normalized * mover.MaxSpeed;

        return Steer(desired, mover.CurrentVelocity, mover.MaxSteering);
    }

    //private Vector3 Flee(Vector3 targetPosition)
    //{
    //    var desired = (targetPosition - _mover.CurrentPosition) * -1;
    //    desired = desired.normalized * maxTravelSpeed;

    //    //var steering = desired - _direction;
    //    //steering = Vector3.ClampMagnitude(steering, maxSteeringForce);

    //    //Debug.DrawLine(_mover.CurrentPosition, steering + _mover.CurrentPosition);

    //    return Steer(desired);
    //}
}
