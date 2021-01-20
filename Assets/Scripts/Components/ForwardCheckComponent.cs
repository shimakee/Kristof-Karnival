﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ForwardCheckComponent : MonoBehaviour, IForwardCollisionChecker
{
    [Header("Cast height:")]
    [SerializeField] float lengthToLower;
    [SerializeField] float separation;
    [Space(10)]

    [Header("Cast distance:")]
    [SerializeField] float castDistanceTop;
    [SerializeField] float castDistanceBottom;
    [Space(10)]

    [Header("Cast dtails:")]
    [Range(0, 360)][SerializeField] float angle;
    [Range(1, 10)][SerializeField] int step;
    [SerializeField] LayerMask mask;
    [Space(10)]

    [Header("Debuging:")]
    [SerializeField] bool drawCastLines = true;

    public bool TopChecker { get { return _isTopHit; } }
    public bool BottomChecker { get { return _isBottomHit; } }

    Vector3 _direction;
    Vector3 _topChecker;
    Vector3 _bottomChecker;
    bool _isTopHit;
    bool _isBottomHit;
    Rigidbody _rb;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _direction = _rb.transform.forward;
    }

    private void FixedUpdate()
    {

        CastBottomChecker();
        CastTopChecker();

        Debug.Log(TopChecker);
        Debug.Log(BottomChecker);
        
    }

    private void CastTopChecker()
    {
        _topChecker = _rb.position;
        _topChecker.y = _topChecker.y - lengthToLower;

        _direction = _rb.transform.forward;

        _isTopHit = RayCastInAngles(_topChecker, _direction, castDistanceTop);
    }

    private void CastBottomChecker()
    {
        _bottomChecker = _rb.position;
        _bottomChecker.y = _bottomChecker.y - (lengthToLower + separation);

        _direction = _rb.transform.forward;

        _isBottomHit = RayCastInAngles(_bottomChecker, _direction, castDistanceBottom);
    }


    private bool RayCastInAngles(Vector3 origin, Vector3 direction, float distance)
    {
        for (int i = 0; i <= step; i++)
        {
            float stepAngle = (angle / step) * (i);

            Vector3 angleDirection = TransformVector(direction, stepAngle);
            Vector3 angleDirectionMirrorSide = TransformVector(direction, -stepAngle);

            RaycastHit hit;
            RaycastHit hitMirror;
            bool isHit = Physics.Raycast(origin, angleDirection, out hit, distance, mask);
            bool isHitMirrorSide = Physics.Raycast(origin, angleDirectionMirrorSide, out hitMirror, distance, mask);

            if (isHit || isHitMirrorSide)
                return true;


            if (drawCastLines)
            {
                var drawDirection = (angleDirection * distance) + origin;
                var drawDirectionReverse = (angleDirectionMirrorSide * distance) + origin;

                Color color = (i == 0) ? Color.blue : Color.green;
                Debug.DrawLine(origin, drawDirection, color);
                Debug.DrawLine(origin, drawDirectionReverse, color);
            }
        }

        return false;
    }


    private Vector3 TransformVector(Vector3 vector, float angle) //TODO: make based on angle
    {
        float cosin = Mathf.Cos(Mathf.Deg2Rad * angle);
        float sin = Mathf.Sin(Mathf.Deg2Rad * angle);

        float x = (vector.x * cosin) + (vector.z * -sin);
        float z = (vector.x * sin) + (vector.z * cosin);

        return new Vector3(x, vector.y, z);
    }
}


public interface IForwardCollisionChecker
{
    bool TopChecker { get; }
    bool BottomChecker { get; }
}