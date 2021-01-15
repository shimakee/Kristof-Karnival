using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboarding : MonoBehaviour
{
    [SerializeField] GameObject target;

    [SerializeField] bool clampX;
    [SerializeField] bool clampY;
    [SerializeField] bool clampZ;

    private void LateUpdate()
    {
        transform.LookAt(target.transform.position);

        var x = transform.rotation.eulerAngles.x;
        var y = transform.rotation.eulerAngles.y;
        var z = transform.rotation.eulerAngles.z;

        if (clampX)
            x = 0;
        if (clampY)
            y = 0;
        if (clampZ)
            z = 0;

        transform.rotation = Quaternion.Euler(x,y,z);
    }
}
