using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitAround : MonoBehaviour
{
    [SerializeField] GameObject objectToCircleAround;
    [SerializeField] Vector3 Offset;
    [Range(0, 360)][SerializeField] float degreesPosition;
    [SerializeField] bool automatic;
    [SerializeField] float speed = 2;

    [SerializeField] bool ZAsY;
    //[SerializeField] float speed;
    [SerializeField] float radius;

    Vector3 _newPosition;
    float _time;
    // Start is called before the first frame update
    void Start()
    {
        _newPosition.x = objectToCircleAround.transform.position.x;
        _newPosition.y = objectToCircleAround.transform.position.y;
        _newPosition.z = objectToCircleAround.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime * speed;
    }

    private void LateUpdate()
    {
        _newPosition = objectToCircleAround.transform.position;

        if (!automatic)
        {
            _newPosition.x = _newPosition.x + Mathf.Cos(degreesPosition * Mathf.Deg2Rad) * radius;
            if (ZAsY)
                _newPosition.z = _newPosition.z + Mathf.Sin(degreesPosition * Mathf.Deg2Rad) * radius;
            else
                _newPosition.y = _newPosition.y + Mathf.Sin(degreesPosition * Mathf.Deg2Rad) * radius;
        }
        else
        {
            _newPosition.x = _newPosition.x + Mathf.Cos(_time * Mathf.Deg2Rad) * radius;
            if (ZAsY)
                _newPosition.z = _newPosition.z + Mathf.Sin(_time * Mathf.Deg2Rad) * radius;
            else
                _newPosition.y = _newPosition.y + Mathf.Sin(_time * Mathf.Deg2Rad) * radius;
        }

            transform.position = _newPosition + Offset;
    }
}
