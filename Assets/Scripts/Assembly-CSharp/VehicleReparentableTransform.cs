using System.Collections.Generic;
using UnityEngine;

public class VehicleReparentableTransform
{
	public Transform TransformToReparent;

	private Transform intendedParent;

	private Vector3 intendedLocalPosition;

	private Quaternion intendedLocalRotation;

	private Vector3 intendedLocalScale;

	private bool isInReparentedState;

	private Transform temporaryParent;

	private List<Collider> tempColliderList = new List<Collider>();

	public VehicleReparentableTransform(Transform sourceTransform, Transform _temporaryParent)
	{
		TransformToReparent = sourceTransform;
		intendedParent = sourceTransform.parent;
		intendedLocalPosition = sourceTransform.localPosition;
		intendedLocalRotation = sourceTransform.localRotation;
		intendedLocalScale = sourceTransform.localScale;
		isInReparentedState = false;
		temporaryParent = _temporaryParent;
		Collider[] componentsInChildren = sourceTransform.GetComponentsInChildren<Collider>();
		tempColliderList.AddRange(componentsInChildren);
	}

	public void SetColliderState(bool state)
	{
		for (int i = 0; i < tempColliderList.Count; i++)
		{
			tempColliderList[i].enabled = state;
		}
	}

	public void SetReparentedState(bool isReparented)
	{
		if (isReparented != isInReparentedState)
		{
			isInReparentedState = isReparented;
			if (isReparented)
			{
				TransformToReparent.gameObject.SetActive(false);
				TransformToReparent.SetParent(temporaryParent, false);
				return;
			}
			TransformToReparent.SetParent(intendedParent, false);
			TransformToReparent.localPosition = intendedLocalPosition;
			TransformToReparent.localRotation = intendedLocalRotation;
			TransformToReparent.localScale = intendedLocalScale;
			TransformToReparent.gameObject.SetActive(true);
		}
	}
}
