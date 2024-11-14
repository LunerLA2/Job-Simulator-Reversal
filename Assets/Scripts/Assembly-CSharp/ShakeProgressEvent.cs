using System;
using UnityEngine.Events;

[Serializable]
public class ShakeProgressEvent : UnityEvent<ShakeController, float>
{
}
