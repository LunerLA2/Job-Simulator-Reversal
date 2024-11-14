using System.Collections.Generic;
using UnityEngine;

public class OnStayTriggerRecorder : MonoBehaviour
{
	private List<Collider> collidersWithinTrigger = new List<Collider>();

	private List<bool> wasColliderOnStay = new List<bool>();

	private bool wasFixedUpdate;

	[SerializeField]
	private bool ignoreTriggers = true;

	[SerializeField]
	private bool ignoreIsKinematicOrNonRigidbodies;

	private void OnDisable()
	{
		collidersWithinTrigger.Clear();
		wasColliderOnStay.Clear();
		wasFixedUpdate = false;
	}

	private void FixedUpdate()
	{
		wasFixedUpdate = true;
		for (int i = 0; i < wasColliderOnStay.Count; i++)
		{
			wasColliderOnStay[i] = false;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if ((!ignoreTriggers || !other.isTrigger) && (!ignoreIsKinematicOrNonRigidbodies || (!(other.attachedRigidbody == null) && !other.attachedRigidbody.isKinematic)))
		{
			int num = collidersWithinTrigger.IndexOf(other);
			if (num >= 0)
			{
				wasColliderOnStay[num] = true;
				return;
			}
			collidersWithinTrigger.Add(other);
			wasColliderOnStay.Add(true);
		}
	}

	public List<Collider> GetCollidersWithinTrigger()
	{
		if (wasFixedUpdate)
		{
			RefreshTriggerList();
			wasFixedUpdate = false;
		}
		return collidersWithinTrigger;
	}

	private void Update()
	{
		if (wasFixedUpdate)
		{
			RefreshTriggerList();
			wasFixedUpdate = false;
		}
	}

	private void RefreshTriggerList()
	{
		for (int i = 0; i < collidersWithinTrigger.Count; i++)
		{
			if (collidersWithinTrigger[i] == null || !wasColliderOnStay[i])
			{
				collidersWithinTrigger.RemoveAt(i);
				wasColliderOnStay.RemoveAt(i);
				i--;
			}
		}
	}
}
