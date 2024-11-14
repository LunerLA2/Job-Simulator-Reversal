using System.Collections.Generic;
using UnityEngine;

public class Slicer : MonoBehaviour
{
	private class PendingSlice
	{
		public readonly Vector3 point;

		public readonly ISliceable target;

		public PendingSlice(Vector3 _point, ISliceable _target)
		{
			point = _point;
			target = _target;
		}
	}

	private int knifeUsesLeft;

	private bool knifeDisabled;

	private bool knifeHasBeenBroken;

	public Transform planeDefiner1;

	public Transform planeDefiner2;

	public Transform planeDefiner3;

	public MeshRenderer editorVisualization;

	private readonly Queue<PendingSlice> pendingSlices = new Queue<PendingSlice>();

	private List<GameObject> suppressUntilContactCeases = new List<GameObject>();

	private Vector3 positionInWorldSpace
	{
		get
		{
			return (planeDefiner1.position + planeDefiner2.position + planeDefiner3.position) / 3f;
		}
	}

	private Vector3 normalInWorldSpace
	{
		get
		{
			Vector3 position = planeDefiner1.position;
			Vector3 position2 = planeDefiner2.position;
			Vector3 position3 = planeDefiner3.position;
			Vector3 result = default(Vector3);
			result.x = position.y * (position2.z - position3.z) + position2.y * (position3.z - position.z) + position3.y * (position.z - position2.z);
			result.y = position.z * (position2.x - position3.x) + position2.z * (position3.x - position.x) + position3.z * (position.x - position2.x);
			result.z = position.x * (position2.y - position3.y) + position2.x * (position3.y - position.y) + position3.x * (position.y - position2.y);
			return result;
		}
	}

	private void Start()
	{
		if (editorVisualization != null)
		{
			editorVisualization.enabled = false;
		}
		bool flag = true;
		flag = planeDefiner1 != null;
		flag &= planeDefiner2 != null;
		if (flag & (planeDefiner3 != null))
		{
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!suppressUntilContactCeases.Contains(other.gameObject))
		{
			ISliceable sliceable = other.GetComponent(typeof(ISliceable)) as ISliceable;
			if (sliceable != null)
			{
				Vector3 point = other.ClosestPointOnBounds(positionInWorldSpace);
				pendingSlices.Enqueue(new PendingSlice(point, sliceable));
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		ContactCeased(other.gameObject);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!suppressUntilContactCeases.Contains(other.gameObject))
		{
			ISliceable sliceable = other.gameObject.GetComponent(typeof(ISliceable)) as ISliceable;
			Debug.Log("Item found for slicing: " + other.gameObject.name);
			if (sliceable != null)
			{
				Debug.Log("Slicable found on " + other.gameObject.name);
				Vector3 point = other.contacts[0].point;
				pendingSlices.Enqueue(new PendingSlice(point, sliceable));
			}
		}
	}

	private void OnCollisionExit(Collision other)
	{
		ContactCeased(other.gameObject);
	}

	private void ContactCeased(GameObject other)
	{
		if (suppressUntilContactCeases.Contains(other))
		{
			suppressUntilContactCeases.Remove(other);
		}
	}

	private void LateUpdate()
	{
		while (pendingSlices.Count > 0)
		{
			PendingSlice pendingSlice = pendingSlices.Dequeue();
			GameObject[] array = pendingSlice.target.Slice(pendingSlice.point, normalInWorldSpace);
			knifeUsesLeft--;
			if (knifeUsesLeft < 0)
			{
				knifeDisabled = true;
			}
			if (array.Length > 1)
			{
				suppressUntilContactCeases.AddRange(array);
			}
			if (knifeDisabled)
			{
				if (knifeHasBeenBroken)
				{
					break;
				}
				base.transform.parent.GetComponent<BreakKnife>().Break();
				knifeHasBeenBroken = true;
			}
		}
	}
}
