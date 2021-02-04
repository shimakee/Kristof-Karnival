using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekBehaviour : ISteeringBehaviour
{
    public float SpeedForce;
    public float SteeringForce;

    public SeekBehaviour(float speed, float steering)
    {
        SpeedForce = speed;
        SteeringForce = steering;
    }
    public Vector3 CalculateSteeringForce(IMoverComponent mover, IFieldOfView fieldOfView)
    {
        if(fieldOfView.GameObjectsInView.Count > 0)
        {
            var target = fieldOfView.GameObjectsInView[0];
            var targetPos = target.transform.position;
            var desired = targetPos - mover.CurrentPosition;
            desired = desired.normalized * SpeedForce;

            var steering = desired - mover.CurrentVelocity;
            steering = Vector3.ClampMagnitude(steering, SteeringForce);

            Debug.Log(target.name);
            Debug.Log(steering);

            return steering;
        }

        return Vector3.zero;
    }
}
