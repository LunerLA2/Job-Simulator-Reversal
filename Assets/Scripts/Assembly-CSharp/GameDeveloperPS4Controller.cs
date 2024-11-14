using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class GameDeveloperPS4Controller : MonoBehaviour
{
	[SerializeField]
	private AttachablePoint discAttachpoint;

	[SerializeField]
	private WorldItemData discWorldItemToAccept;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private Transform discAnimateToPoint;

	[SerializeField]
	private AudioClip discEjectSound;

	private AttachableObject currentDisc;

	[SerializeField]
	private PageData pageToSpawnMasterDiscOn;

	[SerializeField]
	private PageData pageToRevealSuctionOn;

	[SerializeField]
	private GameObject suctionMaster;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents shredRegion;

	[SerializeField]
	private Transform killAxis;

	[SerializeField]
	private float shredDuration;

	[SerializeField]
	private AudioSourceHelper bladesAudio;

	[SerializeField]
	private AudioClip shredSound;

	[SerializeField]
	private ParticleSystem shredSparks;

	private bool suctionAllowed;

	private List<PickupableItem> itemsBeingShredded = new List<PickupableItem>();

	private void OnEnable()
	{
		discAttachpoint.OnObjectWasAttached += DiscWasInserted;
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = shredRegion;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ObjectEnteredShredRegion));
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageStarted = (Action<PageStatusController>)Delegate.Combine(instance.OnPageStarted, new Action<PageStatusController>(PageStarted));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnPageShown = (Action<PageStatusController>)Delegate.Combine(instance2.OnPageShown, new Action<PageStatusController>(PageShown));
		JobBoardManager instance3 = JobBoardManager.instance;
		instance3.OnPageComplete = (Action<PageStatusController>)Delegate.Combine(instance3.OnPageComplete, new Action<PageStatusController>(PageCompleted));
	}

	private void OnDisable()
	{
		discAttachpoint.OnObjectWasAttached -= DiscWasInserted;
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = shredRegion;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ObjectEnteredShredRegion));
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageStarted = (Action<PageStatusController>)Delegate.Remove(instance.OnPageStarted, new Action<PageStatusController>(PageStarted));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnPageShown = (Action<PageStatusController>)Delegate.Remove(instance2.OnPageShown, new Action<PageStatusController>(PageShown));
		JobBoardManager instance3 = JobBoardManager.instance;
		instance3.OnPageComplete = (Action<PageStatusController>)Delegate.Remove(instance3.OnPageComplete, new Action<PageStatusController>(PageCompleted));
	}

	private void Awake()
	{
		discAttachpoint.gameObject.SetActive(false);
		suctionMaster.SetActive(false);
	}

	private void ObjectEnteredShredRegion(Rigidbody rb)
	{
		if (suctionAllowed)
		{
			PickupableItem componentInParent = rb.GetComponentInParent<PickupableItem>();
			if (componentInParent != null && !itemsBeingShredded.Contains(componentInParent))
			{
				StartCoroutine(ShredAsync(componentInParent));
			}
		}
	}

	private IEnumerator ShredAsync(PickupableItem pickupable)
	{
		if (!shredSparks.isPlaying)
		{
			shredSparks.Play();
		}
		itemsBeingShredded.Add(pickupable);
		if (pickupable.IsCurrInHand)
		{
			pickupable.CurrInteractableHand.TryRelease();
		}
		pickupable.enabled = false;
		Rigidbody[] componentsInChildren = pickupable.GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody rb in componentsInChildren)
		{
			rb.isKinematic = true;
		}
		Transform tr = pickupable.transform;
		if (shredSound != null)
		{
			AudioManager.Instance.Play(bladesAudio.transform, shredSound, 1f, 1f);
		}
		Vector3 startPosition = tr.position;
		Vector3 targetPosition = killAxis.transform.position;
		float shredTime = 0f;
		float timeToNextJitter = 0f;
		while (shredTime < shredDuration)
		{
			float progress = shredTime / shredDuration;
			tr.localScale = Vector3.one * Mathf.Lerp(1f, 0.2f, progress);
			tr.position = Vector3.Lerp(startPosition, targetPosition, progress);
			if (timeToNextJitter <= 0f)
			{
				tr.Rotate(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * 10f);
				timeToNextJitter = 0.02f;
			}
			else
			{
				timeToNextJitter -= Time.deltaTime;
			}
			shredTime += Time.deltaTime;
			yield return null;
		}
		itemsBeingShredded.Remove(pickupable);
		if (itemsBeingShredded.Count == 0 && shredSparks.isPlaying)
		{
			shredSparks.Stop();
		}
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
		WorldItem worldItem = pickupable.InteractableItem.WorldItem;
		if (worldItem != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(worldItem.Data, myWorldItem.Data, "ADDED_TO");
			GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "DESTROYED");
		}
		GameDeveloperDataStorage.Instance.RegisterBossObject(pickupable);
	}

	private void PageStarted(PageStatusController page)
	{
		if (page.Data == pageToSpawnMasterDiscOn)
		{
			StartCoroutine(ForceOutDisc());
		}
	}

	private IEnumerator ForceOutDisc()
	{
		if (discAttachpoint.NumAttachedObjects > 0)
		{
			while (discAttachpoint.AttachedObjects.Count > 0)
			{
				AttachableObject obj = discAttachpoint.AttachedObjects[0];
				obj.Detach();
				UnityEngine.Object.Destroy(obj.gameObject);
			}
		}
		Vector3 initialPosition = discAttachpoint.transform.position;
		discAttachpoint.transform.position = discAnimateToPoint.position;
		discAttachpoint.gameObject.SetActive(true);
		discAttachpoint.RefillOneItem();
		float t = 0f;
		while (t < 1f)
		{
			discAttachpoint.transform.position = Vector3.Lerp(discAnimateToPoint.position, initialPosition, t);
			t += Time.deltaTime;
			yield return null;
		}
		discAttachpoint.transform.position = initialPosition;
	}

	private void PageShown(PageStatusController page)
	{
		if (page.Data == pageToRevealSuctionOn)
		{
			suctionAllowed = true;
			suctionMaster.SetActive(true);
		}
	}

	private void PageCompleted(PageStatusController page)
	{
		if (page.Data == pageToRevealSuctionOn)
		{
			suctionAllowed = false;
			StartCoroutine(DisableSuctionAsync());
		}
	}

	private IEnumerator DisableSuctionAsync()
	{
		yield return new WaitForSeconds(1f);
		suctionMaster.SetActive(false);
	}

	private void DiscWasInserted(AttachablePoint slot, AttachableObject disc)
	{
		if (currentDisc == null)
		{
			currentDisc = disc;
			disc.PickupableItem.enabled = false;
			StartCoroutine(ProcessDiscAsync(disc));
		}
	}

	private IEnumerator ProcessDiscAsync(AttachableObject disc)
	{
		Vector3 initialPosition = disc.transform.position;
		float t2 = 0f;
		while (t2 < 1f)
		{
			disc.transform.position = Vector3.Lerp(initialPosition, discAnimateToPoint.position, t2);
			t2 += Time.deltaTime;
			yield return null;
		}
		disc.transform.position = discAnimateToPoint.position;
		if (disc.PickupableItem.InteractableItem.WorldItemData == discWorldItemToAccept)
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
			yield break;
		}
		yield return new WaitForSeconds(0.5f);
		t2 = 0f;
		while (t2 < 1f)
		{
			disc.transform.position = Vector3.Lerp(discAnimateToPoint.position, initialPosition, t2);
			t2 += Time.deltaTime;
			yield return null;
		}
		disc.transform.position = initialPosition;
		disc.PickupableItem.enabled = true;
		currentDisc = null;
	}
}
