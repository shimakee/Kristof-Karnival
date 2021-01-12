using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputHandlerMousePositionComponent))]
public class InputHandlerInteractionComponent : InputHandlerComponent, IInputHandlerComponent
{
    [SerializeField] GridMap2DInherited gridMap;
    [SerializeField] GameObject something;
    InputHandlerMousePositionComponent _inputHandlerMousePosition;


    protected override void InitializeOnAwake()
    {
        _inputHandlerMousePosition = GetComponent<InputHandlerMousePositionComponent>();
    }
    public override void HandleAction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(_inputHandlerMousePosition.MousePosition);
            worldPos.z = 0;
            Vector2Int coord = gridMap.WorldToGrid(worldPos);

            var node = gridMap.GetGridObject(coord);
            Debug.Log("=======");
            Debug.Log(worldPos);
            Debug.Log(coord);
            Debug.Log(node.PathBlockMask);

            GameObject thing = Instantiate(something);
            thing.transform.position = node.WorldPosition;
        }
    }
}