using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IDirectionMoverComponent))]
public class SteeringBehaviour : MonoBehaviour
{

    [SerializeField] GameObject target;

    IDirectionMoverComponent _rb;
    private void FixedUpdate()
    {
        //_rb.AddForce()
    }


}
