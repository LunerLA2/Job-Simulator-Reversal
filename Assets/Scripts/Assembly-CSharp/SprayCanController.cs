using System;
using OwlchemyVR;
using UnityEngine;

public class SprayCanController : MonoBehaviour
{
	private const string animTriggerRustFade = "RustFadePlay";

	[SerializeField]
	private ParticleSystem sprayFx;

	private PickupableItem pickupableItem;

	[SerializeField]
	private Transform nozzlePoint;

	private void Awake()
	{
		pickupableItem = GetComponent<PickupableItem>();
	}

	private void OnEnable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbedUpdate = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbedUpdate, new Action<GrabbableItem>(GrabbedUpdate));
	}

	private void OnDisable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbedUpdate = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbedUpdate, new Action<GrabbableItem>(GrabbedUpdate));
	}

	private void GrabbedUpdate(GrabbableItem item)
	{
		if (item.CurrInteractableHand.IsSqueezedButton())
		{
			if (!sprayFx.isPlaying)
			{
				sprayFx.Play();
			}
			RaycastHit hitInfo;
			if (Physics.Raycast(nozzlePoint.position, nozzlePoint.forward, out hitInfo, 0.6f))
			{
				Debug.Log("Spray can hit: " + hitInfo.collider.name);
				Debug.Log("Hit Object needs to be named RustPatch");
				if (hitInfo.collider.name == "RustPatch")
				{
					hitInfo.collider.gameObject.GetComponent<Animator>().SetTrigger("RustFadePlay");
				}
			}
		}
		else if (sprayFx.isPlaying)
		{
			sprayFx.Stop();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(nozzlePoint.position, nozzlePoint.forward * 0.6f);
	}
}
