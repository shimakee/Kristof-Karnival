using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ForwardCheckComponent : MonoBehaviour, ICastCollisionChecker
{
    [Header("Cast height:")]
    [Range(0, 1)][SerializeField] float lengthToLower = 0;
    [Range(0, 1)][SerializeField] float separation = .4f;
    [Space(10)]

    [Header("Cast distance:")]
    [Range(0, 1)] [SerializeField] float castDistanceTop = .5f;
    [Range(0, 1)] [SerializeField] float castDistanceBottom = .2f;
    [Range(0, 1)] [SerializeField] float castDistanceUnder = .15f;
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
    public bool UnderChecker { get { return _isUnderHit; } }

    Vector3 _direction;
    Vector3 _topChecker;
    Vector3 _bottomChecker;
    bool _isTopHit;
    bool _isBottomHit;
    bool _isUnderHit;
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
        CastUnderChecker();

        //Debug.Log(TopChecker);
        //Debug.Log(BottomChecker);
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

    private void CastUnderChecker()
    {
        var pos = _rb.position;
        pos.y = _bottomChecker.y;
        _direction = _rb.transform.up * -1;

        //RaycastHit hit;
        //_isUnderHit = Physics.Raycast(pos, _direction, out hit, castDistanceBottom, mask);
        _isUnderHit = Physics.Raycast(pos, _direction, castDistanceUnder, mask, QueryTriggerInteraction.Ignore);

        if (drawCastLines)
        {
            var drawDirection = (_direction * castDistanceUnder) + pos;
            Debug.DrawLine(pos, drawDirection, Color.red);
        }
    }


    private bool RayCastInAngles(Vector3 origin, Vector3 direction, float distance)
    {
        for (int i = 0; i <= step; i++)
        {
            float stepAngle = (angle / step) * (i);

            Vector3 angleDirection = TransformVector(direction, stepAngle);
            Vector3 angleDirectionMirrorSide = TransformVector(direction, -stepAngle);

            //RaycastHit hit;
            //RaycastHit hitMirror;
            //bool isHit = Physics.Raycast(origin, angleDirection, out hit, distance, mask);
            //bool isHitMirrorSide = Physics.Raycast(origin, angleDirectionMirrorSide, out hitMirror, distance, mask);
            bool isHit = Physics.Raycast(origin, angleDirection, distance, mask, QueryTriggerInteraction.Ignore);
            bool isHitMirrorSide = Physics.Raycast(origin, angleDirectionMirrorSide, distance, mask, QueryTriggerInteraction.Ignore);

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


public interface ICastCollisionChecker
{
    bool TopChecker { get; }
    bool BottomChecker { get; }
    bool UnderChecker { get; }
}