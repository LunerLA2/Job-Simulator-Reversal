using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class AudioImpactController : MonoBehaviour
{
	private const float MIN_TIME_BETWEEN_IMPACT_SOUNDS = 0.1f;

	private WorldItem worldItem;

	private GrabbableItem grabbableItem;

	private float timeOfLastImpactSound;

	private int recordedImpactCurrentFrameCount = -1;

	private List<AudioImpactController> alreadyRecordedImpacts = new List<AudioImpactController>();

	public WorldItem WorldItem
	{
		get
		{
			return worldItem;
		}
	}

	private void Awake()
	{
		worldItem = GetComponent<WorldItem>();
		if (worldItem == null)
		{
			Debug.LogWarning("AudioImpactController needs a worldItemController:" + base.gameObject.name);
			base.enabled = false;
		}
		grabbableItem = GetComponent<GrabbableItem>();
	}

	private void Start()
	{
		timeOfLastImpactSound = Time.time + 0.1f;
	}

	public void RecordedCollisionAlready(AudioImpactController other)
	{
		if (recordedImpactCurrentFrameCount != Time.frameCount)
		{
			alreadyRecordedImpacts.Clear();
			recordedImpactCurrentFrameCount = Time.frameCount;
		}
		alreadyRecordedImpacts.Add(other);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!(Time.time - timeOfLastImpactSound > 0.1f))
		{
			return;
		}
		GameObject gameObject = collision.gameObject;
		Rigidbody rigidbody = collision.rigidbody;
		AudioImpactController component = gameObject.GetComponent<AudioImpactController>();
		WorldItem worldItem = null;
		SurfaceTypeController surfaceTypeController = null;
		WorldItemData worldItemData = null;
		SurfaceTypeData otherSurfaceTypeData = null;
		if (component != null)
		{
			if (alreadyRecordedImpacts.Count > 0)
			{
				if (recordedImpactCurrentFrameCount == Time.frameCount)
				{
					if (alreadyRecordedImpacts.Contains(component))
					{
						return;
					}
				}
				else
				{
					alreadyRecordedImpacts.Clear();
				}
			}
			component.RecordedCollisionAlready(this);
			worldItemData = component.WorldItem.Data;
			if (worldItemData != null)
			{
				otherSurfaceTypeData = worldItemData.SurfaceTypeData;
			}
		}
		else
		{
			worldItem = gameObject.GetComponent<WorldItem>();
			if (worldItem != null)
			{
				worldItemData = worldItem.Data;
				if (worldItemData != null)
				{
					otherSurfaceTypeData = worldItemData.SurfaceTypeData;
				}
			}
			else
			{
				surfaceTypeController = collision.collider.gameObject.GetComponent<SurfaceTypeController>();
				if (surfaceTypeController == null)
				{
					surfaceTypeController = gameObject.GetComponent<SurfaceTypeController>();
				}
				if (surfaceTypeController != null)
				{
					otherSurfaceTypeData = surfaceTypeController.Data;
				}
			}
		}
		WorldItemData impactWorldItem = null;
		if (this.worldItem != null)
		{
			impactWorldItem = this.worldItem.Data;
		}
		Vector3 contactPoint = ((collision.contacts.Length <= 0) ? base.transform.position : collision.contacts[0].point);
		Vector3 relativeVelocity = collision.relativeVelocity;
		GrabbableItem component2 = gameObject.GetComponent<GrabbableItem>();
		if (grabbableItem != null && grabbableItem.IsCurrInHand)
		{
			relativeVelocity -= grabbableItem.CurrInteractableHand.GrabbedItemCurrVelocity;
		}
		if (component2 != null && component2.IsCurrInHand)
		{
			relativeVelocity += component2.CurrInteractableHand.GrabbedItemCurrVelocity;
		}
		else if (rigidbody != null && rigidbody.isKinematic)
		{
			KinematicRigidbodyVelocityStore component3 = rigidbody.GetComponent<KinematicRigidbodyVelocityStore>();
			if (component3 != null)
			{
				relativeVelocity += component3.CurrVelocity;
			}
		}
		float magnitude = relativeVelocity.magnitude;
		if (magnitude > 0f)
		{
			SoundControlManager.ImpactSound(impactWorldItem, worldItemData, otherSurfaceTypeData, contactPoint, magnitude);
			timeOfLastImpactSound = Time.time;
		}
	}
}
