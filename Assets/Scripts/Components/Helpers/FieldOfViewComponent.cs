﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FieldOfViewComponent : MonoBehaviour, IFieldOfView
{
    public float Radius { get { return viewRadius; } }
    public float Angle { get { return viewAngle; } }
    public List<GameObject> GameObjectsInView { get; private set; }
    public List<GameObject> GameObjectsInSurroundings { get; private set; }


    [Range(.1f, 100)] [SerializeField] float viewRadius;
    [Range(0, 180)] [SerializeField] float viewAngle;
    [SerializeField] LayerMask mask;
    [SerializeField] bool enableDraw;

    SphereCollider _sphereTrigger;
    Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        //_sphereTrigger = gameObject.AddComponent<SphereCollider>();
        //_sphereTrigger.isTrigger = true;
        //_sphereTrigger.radius = radius;

        GameObjectsInSurroundings = new List<GameObject>();
        GameObjectsInView = new List<GameObject>();
    }

    private void FixedUpdate()
    {
        CheckSurroundings();
    }

    private void CheckSurroundings()
    {
        Collider[] hit = Physics.OverlapSphere(_rb.position, Radius);

        GameObjectsInSurroundings.Clear();
        GameObjectsInView.Clear();

        for (int i = 0; i < hit.Length; i++)
        {
            var collider = hit[i];
            int gameObjectLayer = collider.gameObject.layer;
            int layerMask = (gameObjectLayer > 0) ? 1 << gameObjectLayer : gameObjectLayer;

            bool isInLayerMask = (mask.value & layerMask) == layerMask;
            if (!GameObjectsInSurroundings.Contains(collider.gameObject) && isInLayerMask && collider.gameObject != this.gameObject)
                GameObjectsInSurroundings.Add(collider.gameObject);
        }

        foreach (var item in GameObjectsInSurroundings)
        {
            if (IsInView(item))
            {
                if (!GameObjectsInView.Contains(item))
                    GameObjectsInView.Add(item);
            }
            else
            {
                if (GameObjectsInView.Contains(item))
                    GameObjectsInView.Remove(item);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (enableDraw)
        {
            Gizmos.color = Color.blue;

            if(_rb != null)
            {
                var rightSide = TransformVector(_rb.transform.forward * Radius, Angle) + _rb.position;
                var leftSide = TransformVector(_rb.transform.forward * Radius, -Angle) + _rb.position;
                rightSide.y = _rb.position.y;
                leftSide.y = _rb.position.y;

                //draw field of vision
                Gizmos.DrawLine(_rb.position, rightSide);
                Gizmos.DrawLine(_rb.position, leftSide);

                Gizmos.DrawWireSphere(_rb.position, Radius);
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(_rb.position, (_rb.transform.forward * Radius) + _rb.position);

                //draw a line to each item seen
                foreach (var item in GameObjectsInSurroundings)
                {
                    Color color = Color.red;

                    if (GameObjectsInView.Contains(item))
                        color = Color.green;

                    Gizmos.color = color;
                    Gizmos.DrawLine(_rb.position, item.transform.position);
                }
            }

        }
    }

    private bool IsInView(GameObject gameObject)
    {
        Vector3 adjustedGameObjectPosition = gameObject.transform.position - _rb.position;
        float angle = Vector3.Angle(_rb.transform.forward, adjustedGameObjectPosition);

        if (angle > Angle)
            return false;
        else
        {
            RaycastHit hitInfo;
            var isHit = Physics.Raycast(_rb.position, adjustedGameObjectPosition, out hitInfo, Mathf.Infinity, mask.value, QueryTriggerInteraction.Ignore);

            if (!isHit)
                return false;

            if (hitInfo.collider.gameObject != gameObject)
                return false;

            return true;
        }
    }

    protected Vector3 TransformVector(Vector3 vector, float angle) //TODO: make based on angle
    {
        float cosin = Mathf.Cos(Mathf.Deg2Rad * angle);
        float sin = Mathf.Sin(Mathf.Deg2Rad * angle);

        float x = (vector.x * cosin) + (vector.z * -sin);
        float z = (vector.x * sin) + (vector.z * cosin);

        return new Vector3(x, vector.y, z);
    }
}
