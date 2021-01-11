using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(IMoverComponent))]
public class InputHandlerMoveComponent : InputHandlerComponent, IInputHandlerComponent
{
    bool _hasMovementInput;
    Vector2 _inputDirection;
    IMoverComponent _mover;

    protected override void InitializeOnAwake()
    {
        _mover = GetComponent<IMoverComponent>();
    }

    public override void HandleAction(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _hasMovementInput = true;

        }

        if (ctx.performed)
        {
            _inputDirection = ctx.ReadValue<Vector2>().normalized;
            _inputDirection = InputHandlerUtils.RemoveDiagonalInputDirection(_inputDirection);
            _mover.TargetPosition = _mover.CurrentPosition + (Vector3)_inputDirection;
            _mover.Direction = _inputDirection;
            
            Debug.Log($"performed {_inputDirection}");

        }

        if (ctx.canceled)
        {
            _inputDirection = ctx.ReadValue<Vector2>().normalized;
            _inputDirection = InputHandlerUtils.RemoveDiagonalInputDirection(_inputDirection);
            _mover.Direction = _inputDirection;
            _hasMovementInput = false;
        }
    }
}


