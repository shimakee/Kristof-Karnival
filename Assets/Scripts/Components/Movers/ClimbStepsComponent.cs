using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ICastCollisionChecker), typeof(Rigidbody))]
public class ClimbStepsComponent : MonoBehaviour
{
    [Range(0, 200)][SerializeField] float upForceToStep = 15;

    ICastCollisionChecker _castCollisionCheck;
    Rigidbody _rb;
    private void Awake()
    {
        _castCollisionCheck = GetComponent<ICastCollisionChecker>();
        _rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (!_castCollisionCheck.TopChecker && _castCollisionCheck.BottomChecker)
            _rb.AddForce(Vector3.up * upForceToStep);
            //_rb.velocity += Vector3.up * upForceToStep;
    }
}
