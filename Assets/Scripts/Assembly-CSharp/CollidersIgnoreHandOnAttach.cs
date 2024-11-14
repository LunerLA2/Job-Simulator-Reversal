using System;
using UnityEngine;

public class CollidersIgnoreHandOnAttach : MonoBehaviour
{
	[SerializeField]
	private AttachableObject attachableObject;

	[SerializeField]
	private GameObject[] objectsToChangeLayersOnAttach;

	[SerializeField]
	private bool assumeAttachedByDefault = true;

	private void Awake()
	{
		if (assumeAttachedByDefault)
		{
			SetLayers(13);
		}
	}

	private void OnEnable()
	{
		AttachableObject obj = attachableObject;
		obj.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(obj.OnAttach, new Action<AttachableObject, AttachablePoint>(Attached));
		AttachableObject obj2 = attachableObject;
		obj2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(obj2.OnDetach, new Action<AttachableObject, AttachablePoint>(Detached));
	}

	private void OnDisable()
	{
		AttachableObject obj = attachableObject;
		obj.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(obj.OnAttach, new Action<AttachableObject, AttachablePoint>(Attached));
		AttachableObject obj2 = attachableObject;
		obj2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(obj2.OnDetach, new Action<AttachableObject, AttachablePoint>(Detached));
	}

	private void Attached(AttachableObject obj, AttachablePoint point)
	{
		SetLayers(13);
	}

	private void Detached(AttachableObject obj, AttachablePoint point)
	{
		SetLayers(0);
	}

	private void SetLayers(int _layer)
	{
		for (int i = 0; i < objectsToChangeLayersOnAttach.Length; i++)
		{
			objectsToChangeLayersOnAttach[i].layer = _layer;
		}
	}
}
