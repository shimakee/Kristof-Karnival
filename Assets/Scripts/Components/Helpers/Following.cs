using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Following : MonoBehaviour
{

    [SerializeField] GameObject Target;
    [SerializeField] Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        FollowTarget(Target.transform.position);
    }

    void FollowTarget(Vector3 targetPosition)
    {
        transform.position = targetPosition + offset;
    }
}
