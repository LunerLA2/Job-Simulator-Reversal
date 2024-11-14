using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class ShatterOnCollision : MonoBehaviour
{
	private const float MINIMUM_TIME_BEFORE_SHATTERING = 2f;

	public float breakThresh = 10f;

	public GameObject breakPrefab;

	public AudioClip breakSoundEffect;

	[SerializeField]
	private bool sendDestroyEvents;

	[SerializeField]
	private PickupableItem pickupableItem;

	[SerializeField]
	private bool dontShatterIfAttached;

	[SerializeField]
	private AttachableObject optionalAttachableObj;

	private float awakeTime;

	private bool hasShattered;

	public Action<ShatterOnCollision> OnShatter;

	private void Awake()
	{
		awakeTime = Time.realtimeSinceStartup;
	}

	private void OnCollisionEnter(Collision c)
	{
		if (hasShattered || Time.realtimeSinceStartup - awakeTime < 2f)
		{
			return;
		}
		if (dontShatterIfAttached)
		{
			if (optionalAttachableObj != null)
			{
				if (optionalAttachableObj.CurrentlyAttachedTo != null)
				{
					return;
				}
			}
			else
			{
				Debug.LogWarning("dontShatterIfAttached is used, but no optionalAttachableObj is defined: " + base.gameObject.name, base.gameObject);
			}
		}
		float sqrMagnitude = c.relativeVelocity.sqrMagnitude;
		if (pickupableItem.IsCurrInHand && pickupableItem.CurrInteractableHand != null)
		{
			sqrMagnitude = pickupableItem.CurrInteractableHand.GetCurrentVelocity().sqrMagnitude;
		}
		if (sqrMagnitude > breakThresh)
		{
			StartCoroutine(WaitUntilEndOfFrameThenBreak());
		}
	}

	private IEnumerator WaitUntilEndOfFrameThenBreak()
	{
		yield return new WaitForEndOfFrame();
		Break(false);
	}

	public void Break(bool avoidBreakOnImpact)
	{
		if (avoidBreakOnImpact || hasShattered)
		{
			return;
		}
		hasShattered = true;
		if (optionalAttachableObj != null)
		{
			if (optionalAttachableObj.CurrentlyAttachedTo != null)
			{
				Debug.Log("Emergency detach of shatterable object!");
				optionalAttachableObj.CurrentlyAttachedTo.Detach(optionalAttachableObj);
			}
			optionalAttachableObj.enabled = false;
		}
		GrabbableItem component = GetComponent<GrabbableItem>();
		if (component != null && component.IsCurrInHand && component.CurrInteractableHand != null)
		{
			component.CurrInteractableHand.ManuallyReleaseJoint();
		}
		if (breakSoundEffect != null)
		{
			AudioManager.Instance.Play(base.transform.position, breakSoundEffect, 1f, 1f);
		}
		if (sendDestroyEvents)
		{
			WorldItem component2 = GetComponent<WorldItem>();
			if (component2 != null)
			{
				GameEventsManager.Instance.ItemActionOccurred(component2.Data, "DESTROYED");
			}
		}
		hasShattered = true;
		if (OnShatter != null)
		{
			OnShatter(this);
		}
		if (breakPrefab != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(breakPrefab, base.transform.position, base.transform.rotation) as GameObject;
			gameObject.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
