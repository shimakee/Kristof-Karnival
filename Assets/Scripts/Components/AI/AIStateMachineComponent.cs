using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IDirectionMoverComponent), typeof(IFieldOfView))]
public class AIStateMachineComponent : MonoBehaviour
{
    public int state = 0;
    public float speed = 5;
    public float steering = 5;
    //public ISteeringBehaviour[] behaviours;
    private IDirectionMoverComponent mover;
    private IFieldOfView fieldOfView;
    private ISteeringBehaviour seek;
    Vector3 _direction = Vector3.zero;

    private void Awake()
    {
        fieldOfView = GetComponent<IFieldOfView>();
        mover = GetComponent<IDirectionMoverComponent>();
        seek = new SeekBehaviour(speed, steering);
    }

    // Update is called once per frame
    void Update()
    {
        if(state == 1)
        {
            _direction += seek.CalculateSteeringForce(mover, fieldOfView) * Time.deltaTime;
        }
        mover.MoveDirection(_direction);
    }
}
