using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IDirectionMoverComponent))]
public class CollisionAvoidance : MonoBehaviour
{
    [SerializeField] LayerMask collisionDetectionMask;
    [SerializeField] float distanceAhead;
    [SerializeField] float collisionAvoidanceWeight;
    [SerializeField] float angle;
    [SerializeField] float maxTravelSpeed;

    Vector3 _direction;
    IDirectionMoverComponent _mover;
    // Start is called before the first frame update
    void Awake()
    {
        _direction = Vector3.zero;

        _mover = GetComponent<IDirectionMoverComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        _direction += CheckForCollision(distanceAhead, angle) * Time.deltaTime * collisionAvoidanceWeight;
        _direction = Vector3.ClampMagnitude(_direction, maxTravelSpeed);

        _mover.MoveDirection(_direction);
        Debug.DrawLine(_mover.CurrentPosition, _mover.CurrentPosition + _direction);
    }

    #region Collision avoidance
    private Vector3 CheckForCollision(float distance, float angle)
    {
        Vector3 direction = _mover.CurrentVelocity;
        //Vector3 direction = _mover.LastDirectionFacing;
        RaycastHit hitInfo;
        //bool isHit = Physics.SphereCast(_mover.CurrentPosition, 1, direction, out hitInfo, distance, collisionDetectionMask);
        bool isHit = Physics.Raycast(_mover.CurrentPosition, direction, out hitInfo, distance, collisionDetectionMask);

        if (isHit)
        {
            var directionAvoidance = hitInfo.point - hitInfo.collider.transform.position;
            var dynamicLength = _mover.CurrentVelocity.magnitude / maxTravelSpeed;
            directionAvoidance = (directionAvoidance.normalized * distanceAhead) * dynamicLength;

            direction = directionAvoidance;
        }

        //Debug.DrawLine(_mover.CurrentPosition, _mover.CurrentPosition + direction);
        return direction.normalized * maxTravelSpeed;
        //return Seek(direction.normalized * maxTravelSpeed + _mover.CurrentPosition);
    }

    private Vector3 TransformVector(Vector3 vector, float angle) //TODO: make based on angle
    {
        float cosin = Mathf.Cos(Mathf.Deg2Rad * angle);
        float sin = Mathf.Sin(Mathf.Deg2Rad * angle);

        float x = (vector.x * cosin) + (vector.z * -sin);
        float z = (vector.x * sin) + (vector.z * cosin);

        return new Vector3(x, vector.y, z);
    }

    #endregion
}
