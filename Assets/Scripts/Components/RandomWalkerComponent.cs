using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: require mover component based on velocity? or target? or direction?
[RequireComponent(typeof(IDirectionMoverComponent))]
public class RandomWalkerComponent : MonoBehaviour
{
    [Range(0, 360)] [SerializeField] float maxDirectionAngleRange;
    [Range(.1f, 20f)][SerializeField] float radius = 1;
    [Range(.1f, 20f)] [SerializeField] float distanceAheadToCheck = 1;
    [SerializeField] bool enableDebugDraw = false;
    [Range(.1f, 20f)] [SerializeField] float timeInterval = 1;

    IDirectionMoverComponent _mover;

    Vector3 _centerPoint;
    Vector3 _destination;
    float _time;

    private void Awake()
    {
        _mover = GetComponent<IDirectionMoverComponent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnDrawGizmos()
    {
        if (enableDebugDraw)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_centerPoint, .2f);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_centerPoint, radius);

            Gizmos.color = Color.green;
            if (_mover != null)
                Gizmos.DrawLine(_mover.CurrentPosition, _destination);
        }
    }

    private void FixedUpdate()
    {
        _time += Time.fixedDeltaTime;

        if(_time > timeInterval)
        {
            _time = 0;
            _centerPoint = _mover.CurrentPosition + (_mover.LastDirectionFacing.normalized * distanceAheadToCheck);
            _destination = GetPointWithinACircle(_centerPoint, radius);

            Debug.DrawLine(_mover.CurrentPosition, _destination);
            //_mover.Move(_destination);
            _mover.MoveDirection(_destination);
        }
    }

    Vector3 GetPointWithinACircle(Vector3 center, float radius)
    {
        float randomAngle = Random.Range(0, maxDirectionAngleRange);

        float x =center.x + Mathf.Cos(randomAngle * Mathf.Deg2Rad) * radius;
        float z =center.z + Mathf.Sin(randomAngle * Mathf.Deg2Rad) * radius;

        return new Vector3(x, center.y, z);
    }
}
