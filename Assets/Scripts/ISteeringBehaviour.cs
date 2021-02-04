﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISteeringBehaviour
{
    Vector3 CalculateSteeringForce(IMoverComponent mover, IFieldOfView fieldOfView);
}
