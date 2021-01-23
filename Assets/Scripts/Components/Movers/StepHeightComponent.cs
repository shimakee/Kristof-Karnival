using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ICastCollisionChecker), typeof(Rigidbody))]
public class StepHeightComponent : MonoBehaviour
{
    [Range(0, 20)][SerializeField] float upForceToStep = 15;

    ICastCollisionChecker _castCollisionCheck;
    Rigidbody _rb;
    private void Awake()
    {
        _castCollisionCheck = GetComponent<ICastCollisionChecker>();
        _rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        if (!_castCollisionCheck.TopChecker && _castCollisionCheck.BottomChecker)
            _rb.AddForce(Vector3.up * upForceToStep);

    }
}
