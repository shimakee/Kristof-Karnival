using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachTransformTriggerComponent : MonoBehaviour
{
    [SerializeField] LayerMask mask;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        int objectLayer = other.gameObject.layer;
        var maskValue = (objectLayer > 0) ? 1 << objectLayer : 0;

        if ((maskValue & mask.value) == maskValue)
            other.gameObject.transform.parent = transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.transform.parent == transform)
            other.gameObject.transform.parent = null;
    }
}
