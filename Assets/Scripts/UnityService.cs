using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IUnityService
{
    //float DeltaTime { get; }
    float GetAxisRaw(string axisName);
    float GetDeltaTime();
}
public class UnityService : IUnityService
{

    //public float DeltaTime { get { return Time.deltaTime; } }
    public float GetAxisRaw(string axisName)
    {
        return Input.GetAxisRaw(axisName);
    }
    public float GetDeltaTime()
    {
        return Time.deltaTime;
    }
}
