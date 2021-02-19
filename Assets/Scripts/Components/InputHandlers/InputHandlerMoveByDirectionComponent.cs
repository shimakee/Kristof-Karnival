using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(IDirectionMoverComponent))]
public class InputHandlerMoveByDirectionComponent : InputHandlerComponent, IInputHandlerComponent
{
    [SerializeField] bool normalizeDirection;
    [SerializeField] bool restrictDiagonal;

    Vector3 _inputDirection;
    IDirectionMoverComponent _mover;

    protected override void InitializeOnAwake()
    {
        _mover = GetComponent<IDirectionMoverComponent>();
    }

    public override void HandleAction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            _inputDirection = ctx.ReadValue<Vector2>();

            if (normalizeDirection)
                _inputDirection = _inputDirection.normalized;

            if (restrictDiagonal)
                _inputDirection = InputHandlerUtils.RemoveDiagonalInputDirection(_inputDirection);

            if(ZasY)
                _inputDirection = new Vector3(_inputDirection.x, 0, _inputDirection.y);

            Debug.Log(_inputDirection);
            _mover.MoveDirection(_inputDirection);


        }

        if (ctx.canceled)
        {
            _inputDirection = ctx.ReadValue<Vector2>().normalized;
            _mover.MoveDirection(_inputDirection);

        }

        Debug.Log($"direction from input {_inputDirection} magnitude {_inputDirection.magnitude}");
    }
}


