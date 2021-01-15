using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputHandlerMousePositionComponent : InputHandlerComponent, IInputHandlerComponent
{
    public Vector2 MousePosition { get { return _mousePosition; } }
    
    private Vector2 _mousePosition;
    
    public override void HandleAction(InputAction.CallbackContext ctx)
    {
        _mousePosition = ctx.ReadValue<Vector2>();
    }

    protected override void InitializeOnAwake()
    {
    }
}
