using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class LocalPositionChangedEvent : UnityEvent<Vector3>
{
}
