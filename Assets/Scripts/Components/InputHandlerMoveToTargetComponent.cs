using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputHandlerMousePositionComponent), typeof(ITargetMoverComponent))]
public class InputHandlerMoveToTargetComponent : InputHandlerComponent, IInputHandlerComponent
{
    [SerializeField] bool isOrthographic;
    InputHandlerMousePositionComponent _inputHandlerMousePosition;
    ITargetMoverComponent _mover;
    
    protected override void InitializeOnAwake()
    {
        _inputHandlerMousePosition = GetComponent<InputHandlerMousePositionComponent>();
        _mover = GetComponent<ITargetMoverComponent>();
    }
    public override void HandleAction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (isOrthographic)
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(_inputHandlerMousePosition.MousePosition);
                _mover.TargetPosition = worldPos;
                Debug.Log(worldPos);
            }
        }
    }

}
