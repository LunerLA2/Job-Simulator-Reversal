using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class CapDespawner : MonoBehaviour
{
	private void Start()
	{
		GrabbableItem component = GetComponent<GrabbableItem>();
		component.OnReleased = (Action<GrabbableItem>)Delegate.Combine(component.OnReleased, new Action<GrabbableItem>(CapReleased));
	}

	private void CapReleased(GrabbableItem cap)
	{
		cap.OnReleased = (Action<GrabbableItem>)Delegate.Remove(cap.OnReleased, new Action<GrabbableItem>(CapReleased));
		StartCoroutine(DespawnCap(cap));
	}

	private IEnumerator DespawnCap(GrabbableItem cap)
	{
		yield return new WaitForSeconds(2f);
		bool wasDespawned2 = false;
		while (!wasDespawned2)
		{
			bool canBeDespawned = true;
			if (cap.GetComponent<TrinketSpringController>() != null)
			{
				canBeDespawned = false;
			}
			StasisFieldItem stasis = cap.GetComponent<StasisFieldItem>();
			if (stasis != null && stasis.IsInStasis)
			{
				canBeDespawned = false;
			}
			if (cap.IsCurrInHand)
			{
				canBeDespawned = false;
			}
			if (canBeDespawned)
			{
				cap.enabled = false;
				float scale = 1f;
				while (scale > 0f)
				{
					scale -= Time.deltaTime;
					cap.transform.localScale = cap.transform.localScale * scale;
					yield return null;
				}
				wasDespawned2 = true;
				break;
			}
			yield return new WaitForSeconds(3f);
		}
	}
}
