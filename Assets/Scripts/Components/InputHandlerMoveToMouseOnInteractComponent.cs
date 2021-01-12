using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputHandlerMousePositionComponent), typeof(IMoverComponent))]
public class InputHandlerMoveToMouseOnInteractComponent : InputHandlerComponent, IInputHandlerComponent
{
    InputHandlerMousePositionComponent _inputHandlerMousePosition;
    IMoverComponent _mover;

    
    protected override void InitializeOnAwake()
    {
        _inputHandlerMousePosition = GetComponent<InputHandlerMousePositionComponent>();
        _mover = GetComponent<IMoverComponent>();
    }
    public override void HandleAction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(_inputHandlerMousePosition.MousePosition);
            _mover.TargetPosition = worldPos;
        }
    }

}
