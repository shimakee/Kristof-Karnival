using System;
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

    public static Vector3 Seek(Vector3 targetPosition, Vector3 currentVelocity, IMoverComponent mover)
    {
        var desired = targetPosition - mover.CurrentPosition;
        //var desired = targetPosition - currentVelocity;
        desired = desired.normalized * mover.MaxSpeed;

        return Steer(desired, mover.CurrentVelocity, mover.MaxSteering);
        //return Steer(desired, currentVelocity, mover.MaxSteering);
    }

    public static Vector3 Flee(Vector3 targetPosition, Vector3 currentVelocity, IMoverComponent mover)
    {
        var desired = (targetPosition - mover.CurrentPosition) * -1;
        //var desired = mover.CurrentPosition - targetPosition;
        desired = desired.normalized * mover.MaxSpeed;

        return Steer(desired, currentVelocity, mover.MaxSteering);
    }
    public static Vector3 Arriving(IMoverComponent mover, Vector3 direction, Vector3 target, float distanceFromTarget = 0, float threshold = 0)
    {
        if (threshold < 0 || distanceFromTarget < 0)
            throw new ArgumentOutOfRangeException("Threshold or distanceFromTarget must have a value of >= 0");

        float arrivingDistanceToTarget = Vector3.Distance(mover.CurrentPosition, target);

        var distance = Vector3.Distance(target, mover.CurrentPosition) - distanceFromTarget;
        if (distance < threshold)
            distance = 0;
        float multiplier = (distance < arrivingDistanceToTarget) ? distance / arrivingDistanceToTarget : 1;

        return Vector3.ClampMagnitude(direction, mover.MaxSpeed * multiplier);
    }

    public static float ComputeWeightByDistanceFurtherHigher(float distance, float weight, float maxDistance, float minDistance)
    {
        if (distance > minDistance)
        {
            float multiplier = (distance > maxDistance) ? 1 : distance / maxDistance;

            return weight * multiplier;
        }

        return 0;
    }

    public static float ComputeWeightByDistanceFurtherLower(float distance, float weight, float maxDistance, float minDistance)
    {
        if (distance < minDistance)
            return weight;

        float multiplier = (distance > maxDistance) ? 0 : 1 - (distance / maxDistance);

        return weight * multiplier;
    }

    public static float ComputeWeightByAngleAlignmentHigher(float weight, Vector3 angleOne, Vector3 angleTwo)
    {
        float dotProduct = Vector3.Dot(angleOne, angleTwo);

        float multiplier = (dotProduct <= 0) ? 0 : dotProduct / 1;

        return weight * multiplier;
    }
}
