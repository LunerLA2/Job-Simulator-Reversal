using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class AttachablePoint : MonoBehaviour
{
	public enum RefillSelectionMode
	{
		Cycle = 0,
		Random = 1
	}

	[SerializeField]
	protected RigidbodyEnterExitTriggerEvents inRangeTrigger;

	[SerializeField]
	protected AttachablePointData data;

	[SerializeField]
	public float detachDistance;

	[SerializeField]
	public float detachMovementRatio = 1f;

	[SerializeField]
	public bool omniDirectionDetach;

	[SerializeField]
	public bool retainDistanceOnAttach;

	[SerializeField]
	public MeshRenderer[] highlight;

	[SerializeField]
	public ParticleSystem particleOnAttach;

	[SerializeField]
	public ParticleSystem particleOnDetach;

	[SerializeField]
	public PickupableItem pickupableItem;

	[SerializeField]
	public Collider optionalAutoConnectArea;

	[SerializeField]
	public bool onlyAllowDetachWhenCurrInHand;

	[SerializeField]
	public bool refill;

	[SerializeField]
	public float delayBeforeRefilling;

	[SerializeField]
	public GameObject[] refillPrefabs;

	[SerializeField]
	protected int initialCapacity;

	[SerializeField]
	public RefillSelectionMode refillSelectionMethod;

	[SerializeField]
	public float refillDuration = 0.25f;

	[SerializeField]
	public bool growRefillingObject;

	[SerializeField]
	private bool refillWhenObjectReleasedFromHand;

	[SerializeField]
	private float durationDroppedBeforeRefilled = 5f;

	[SerializeField]
	private bool ignoreInRangesWhenFull;

	[SerializeField]
	private bool dontConsiderInRangeWhenDetached;

	protected bool isRefilling;

	protected bool hasGeneratedRefillObject;

	protected AttachableObject refillObj;

	protected int refillObjectIndex;

	private bool firstSpawnCompleted;

	private Collider col;

	private CachedInteractionState[] oldItemInteractions;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents snapInstantlyTrigger;

	private bool isHighlighting;

	private bool isWaitingForReleaseTimer;

	private AttachableObject lastSnappedObjectThatIsStillInAutoSnapRange;

	protected List<AttachableObject> objectsInRange = new List<AttachableObject>();

	protected List<AttachableObject> attachedObjects = new List<AttachableObject>();

	public List<AttachableObject> AttachedObjects
	{
		get
		{
			return attachedObjects;
		}
	}

	public int NumAttachedObjects
	{
		get
		{
			return attachedObjects.Count;
		}
	}

	public bool IsRefilling
	{
		get
		{
			return isRefilling;
		}
	}

	public int InitialCapacity
	{
		get
		{
			return initialCapacity;
		}
	}

	public float SqrDetachDistance
	{
		get
		{
			return detachDistance * detachDistance;
		}
	}

	public RigidbodyEnterExitTriggerEvents InRangeTrigger
	{
		get
		{
			return inRangeTrigger;
		}
	}

	public Collider OptionalAutoConnectArea
	{
		get
		{
			return optionalAutoConnectArea;
		}
	}

	public PickupableItem PickupableItem
	{
		get
		{
			return pickupableItem;
		}
	}

	public bool OnlyAllowDetachWhenCurrInHand
	{
		get
		{
			return onlyAllowDetachWhenCurrInHand;
		}
	}

	public AttachablePointData Data
	{
		get
		{
			return data;
		}
	}

	public virtual bool IsBusy
	{
		get
		{
			return NumDetachingObjects > 0;
		}
	}

	public virtual bool IsOccupied
	{
		get
		{
			return attachedObjects.Count > 0;
		}
	}

	public virtual bool HasContents
	{
		get
		{
			return attachedObjects.Count > 0;
		}
	}

	public int NumDetachingObjects
	{
		get
		{
			int num = 0;
			for (int i = 0; i < attachedObjects.Count; i++)
			{
				if (attachedObjects[i].PickupableItem != null && attachedObjects[i].PickupableItem.IsCurrInHand)
				{
					num++;
				}
			}
			return num;
		}
	}

	public event Action<AttachableObject> OnObjectEnteredRange;

	public event Action<AttachableObject> OnObjectExitedRange;

	public event Action<AttachablePoint, AttachableObject> OnObjectWasAttached;

	public event Action<AttachablePoint, AttachableObject> OnObjectWasDetached;

	public event Action<AttachablePoint, AttachableObject> OnObjectWasRefilled;

	public void EditorSetInRangeTrigger(RigidbodyEnterExitTriggerEvents t)
	{
		inRangeTrigger = t;
	}

	public virtual bool CanAcceptItem(WorldItemData item)
	{
		return !IsOccupied;
	}

	protected virtual void Awake()
	{
		SetHighlightState(false, true);
		col = GetComponent<Collider>();
	}

	protected virtual void OnEnable()
	{
		if (pickupableItem != null)
		{
			PickupableItem obj = pickupableItem;
			obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(ItemGrabbed));
			PickupableItem obj2 = pickupableItem;
			obj2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj2.OnReleased, new Action<GrabbableItem>(ItemReleased));
		}
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = inRangeTrigger;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnteredRange));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = inRangeTrigger;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExitedRange));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents3 = inRangeTrigger;
		rigidbodyEnterExitTriggerEvents3.OnUnknownRigidbodyDestroyedInsideOfTrigger = (Action)Delegate.Combine(rigidbodyEnterExitTriggerEvents3.OnUnknownRigidbodyDestroyedInsideOfTrigger, new Action(SomeRigidbodyExitedRange));
		col.enabled = true;
		if (snapInstantlyTrigger != null)
		{
			RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents4 = snapInstantlyTrigger;
			rigidbodyEnterExitTriggerEvents4.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents4.OnRigidbodyEnterTrigger, new Action<Rigidbody>(SnapInstantlyTriggerEntered));
			RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents5 = snapInstantlyTrigger;
			rigidbodyEnterExitTriggerEvents5.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents5.OnRigidbodyExitTrigger, new Action<Rigidbody>(SnapInstantlyTriggerExited));
		}
		if (isRefilling)
		{
			if (!hasGeneratedRefillObject)
			{
				CreateRefillObjectAndCheckForSuccess();
			}
			if (refillObj != null)
			{
				FinishRefill();
			}
		}
		else if (firstSpawnCompleted && refillWhenObjectReleasedFromHand && (isWaitingForReleaseTimer || !IsOccupied || refillObj == null))
		{
			StartCoroutine(RefillAsync(true));
		}
	}

	protected virtual void OnDisable()
	{
		if (pickupableItem != null)
		{
			PickupableItem obj = pickupableItem;
			obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(ItemGrabbed));
			PickupableItem obj2 = pickupableItem;
			obj2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj2.OnReleased, new Action<GrabbableItem>(ItemReleased));
		}
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = inRangeTrigger;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnteredRange));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = inRangeTrigger;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExitedRange));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents3 = inRangeTrigger;
		rigidbodyEnterExitTriggerEvents3.OnUnknownRigidbodyDestroyedInsideOfTrigger = (Action)Delegate.Remove(rigidbodyEnterExitTriggerEvents3.OnUnknownRigidbodyDestroyedInsideOfTrigger, new Action(SomeRigidbodyExitedRange));
		if (snapInstantlyTrigger != null)
		{
			RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents4 = snapInstantlyTrigger;
			rigidbodyEnterExitTriggerEvents4.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents4.OnRigidbodyEnterTrigger, new Action<Rigidbody>(SnapInstantlyTriggerEntered));
			RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents5 = snapInstantlyTrigger;
			rigidbodyEnterExitTriggerEvents5.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents5.OnRigidbodyExitTrigger, new Action<Rigidbody>(SnapInstantlyTriggerExited));
		}
		if (refillWhenObjectReleasedFromHand && refillObj != null && isRefilling)
		{
			FinishRefill();
		}
		if (isRefilling && hasGeneratedRefillObject && refillObj != null)
		{
			FinishRefill();
		}
		SetHighlightState(false);
		List<AttachableObject> list = objectsInRange;
		for (int i = 0; i < list.Count; i++)
		{
			ObjectExitedRange(list[i]);
		}
	}

	private void Start()
	{
		StartCoroutine(InitialRefillAsync());
		if (ignoreInRangesWhenFull)
		{
			StartCoroutine(WaitAndDisregardInRanges());
		}
	}

	private IEnumerator WaitAndDisregardInRanges()
	{
		yield return null;
		yield return null;
		yield return null;
		for (int i = 0; i < attachedObjects.Count; i++)
		{
			attachedObjects[i].ManuallyClearInRanges();
		}
	}

	public void ForceSetNextRefillObjectIndex(int index)
	{
		if (index != -1)
		{
			refillObjectIndex = index;
		}
	}

	private void SnapInstantlyTriggerEntered(Rigidbody r)
	{
		AttachableObject component = r.GetComponent<AttachableObject>();
		bool flag = true;
		if (pickupableItem != null)
		{
			flag = pickupableItem.Rigidbody == null || pickupableItem.Rigidbody != r;
		}
		if (component != null && lastSnappedObjectThatIsStillInAutoSnapRange != component && !attachedObjects.Contains(component) && flag)
		{
			component.GotInRangeOfInstantSnapAttachablePoint(this);
		}
	}

	private void SnapInstantlyTriggerExited(Rigidbody r)
	{
		AttachableObject component = r.GetComponent<AttachableObject>();
		bool flag = true;
		if (pickupableItem != null)
		{
			flag = pickupableItem.Rigidbody == null || pickupableItem.Rigidbody != r;
		}
		if (component != null && lastSnappedObjectThatIsStillInAutoSnapRange == component && !attachedObjects.Contains(component) && flag)
		{
			lastSnappedObjectThatIsStillInAutoSnapRange = null;
		}
	}

	public Coroutine RefillOneItem()
	{
		return StartCoroutine(RefillAsync(false));
	}

	public void RefillOneItemImmediate()
	{
		StartCoroutine(RefillAsync(true));
	}

	private IEnumerator InitialRefillAsync()
	{
		yield return null;
		for (int i = 0; i < initialCapacity; i++)
		{
			yield return StartCoroutine(RefillAsync(true));
		}
		firstSpawnCompleted = true;
	}

	public AttachableObject GetAttachedObject(int i)
	{
		if (i >= 0 && i < attachedObjects.Count)
		{
			return attachedObjects[i];
		}
		return null;
	}

	private void ItemGrabbed(GrabbableItem grabbableItem)
	{
		if (!HasContents)
		{
			return;
		}
		for (int i = 0; i < attachedObjects.Count; i++)
		{
			AttachableObject attachableObject = attachedObjects[i];
			if (attachableObject != null)
			{
				attachableObject.AttachPointGrabbed(this);
			}
		}
	}

	private void ItemReleased(GrabbableItem grabbableItem)
	{
		if (!HasContents)
		{
			return;
		}
		for (int i = 0; i < attachedObjects.Count; i++)
		{
			AttachableObject attachableObject = attachedObjects[i];
			if (attachableObject != null)
			{
				attachableObject.AttachPointReleased(this);
			}
		}
	}

	public virtual void Attach(AttachableObject o, int index = -1, bool suppressEvents = false, bool suppressEffects = false)
	{
		if (isRefilling && o != refillObj)
		{
			return;
		}
		if (index == -1)
		{
			index = attachedObjects.Count;
		}
		attachedObjects.Insert(index, o);
		ObjectExitedRange(o);
		if (ignoreInRangesWhenFull)
		{
			AttachableObject[] array = objectsInRange.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null)
				{
					ObjectExitedRange(array[i]);
					array[i].ManuallyClearInRanges();
				}
			}
			o.ManuallyClearInRanges();
		}
		if (!suppressEffects)
		{
			PlayAttachParticles(o);
			AudioClip soundWhenSomethingAttached = data.SoundWhenSomethingAttached;
			if (soundWhenSomethingAttached != null)
			{
				AudioManager.Instance.Play(base.transform, soundWhenSomethingAttached, 1f, 1f);
			}
		}
		if (!suppressEvents && this.OnObjectWasAttached != null)
		{
			this.OnObjectWasAttached(this, o);
		}
		lastSnappedObjectThatIsStillInAutoSnapRange = o;
	}

	public virtual void Detach(AttachableObject o, bool suppressEvents = false, bool suppressEffects = false)
	{
		if (isRefilling)
		{
			return;
		}
		attachedObjects.Remove(o);
		if (!suppressEffects)
		{
			PlayDetachParticles(o);
			AudioClip soundWhenSomethingDetached = data.SoundWhenSomethingDetached;
			if (soundWhenSomethingDetached != null)
			{
				AudioManager.Instance.Play(base.transform, soundWhenSomethingDetached, 1f, 1f);
			}
		}
		if (!suppressEvents && this.OnObjectWasDetached != null)
		{
			this.OnObjectWasDetached(this, o);
		}
		if (refill && refillPrefabs.Length > 0)
		{
			if (refillWhenObjectReleasedFromHand)
			{
				PickupableItem component = o.GetComponent<PickupableItem>();
				if (component != null && !component.IsCurrInHand)
				{
					StartCoroutine(RefillAsync(false));
				}
			}
			else
			{
				StartCoroutine(RefillAsync(false));
			}
		}
		lastSnappedObjectThatIsStillInAutoSnapRange = o;
		ObjectExitedRange(o);
		if (!dontConsiderInRangeWhenDetached)
		{
			RigidbodyEnteredRange(o.PickupableItem.Rigidbody);
		}
	}

	private void RigidbodyEnteredRange(Rigidbody rb)
	{
		if (rb == null || (ignoreInRangesWhenFull && IsOccupied))
		{
			return;
		}
		AttachableObject component = rb.GetComponent<AttachableObject>();
		if (!(component != null) || component.PickupableItem == PickupableItem || !base.isActiveAndEnabled || IsBusy || !CanAcceptItem(component.PickupableItem.InteractableItem.WorldItemData))
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < component.ValidAttachablePointDatas.Length; i++)
		{
			if (component.ValidAttachablePointDatas[i] == data)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			ObjectEnteredRange(component);
		}
	}

	private void RigidbodyExitedRange(Rigidbody rb)
	{
		if (!(rb == null))
		{
			AttachableObject component = rb.GetComponent<AttachableObject>();
			if (component != null && !(component.PickupableItem == PickupableItem))
			{
				ObjectExitedRange(component);
			}
		}
	}

	private void SomeRigidbodyExitedRange()
	{
		for (int i = 0; i < objectsInRange.Count; i++)
		{
			if (objectsInRange[i] == null)
			{
				objectsInRange.RemoveAt(i);
				i--;
			}
			else if (objectsInRange[i].PickupableItem.Rigidbody == null)
			{
				ObjectExitedRange(objectsInRange[i]);
			}
		}
	}

	private void ObjectEnteredRange(AttachableObject o)
	{
		o.GotInRangeOfAttachablePoint(this);
		if (!objectsInRange.Contains(o))
		{
			objectsInRange.Add(o);
			if (this.OnObjectEnteredRange != null)
			{
				this.OnObjectEnteredRange(o);
			}
		}
	}

	public void ObjectExitedRange(AttachableObject o)
	{
		o.LeftRangeOfAttachablePoint(this);
		if (objectsInRange.Contains(o))
		{
			objectsInRange.Remove(o);
			if (this.OnObjectExitedRange != null)
			{
				this.OnObjectExitedRange(o);
			}
			if (o == lastSnappedObjectThatIsStillInAutoSnapRange || objectsInRange.Count == 0)
			{
				lastSnappedObjectThatIsStillInAutoSnapRange = null;
			}
		}
		for (int i = 0; i < objectsInRange.Count; i++)
		{
			if (objectsInRange[i] == null)
			{
				objectsInRange.RemoveAt(i);
				i--;
			}
		}
	}

	protected void SetHighlightState(bool s, bool force = false)
	{
		if (isHighlighting == s && !force)
		{
			return;
		}
		isHighlighting = s;
		if (highlight == null)
		{
			return;
		}
		for (int i = 0; i < highlight.Length; i++)
		{
			highlight[i].enabled = s;
		}
		if (!(snapInstantlyTrigger != null))
		{
			return;
		}
		snapInstantlyTrigger.enabled = isHighlighting;
		if (!isHighlighting && attachedObjects.Count == 0)
		{
			if (base.gameObject.name == "SnapPoint 3")
			{
				Debug.Log("Nothing is attached and the highlight is off, set lastSnappedToNull");
			}
			lastSnappedObjectThatIsStillInAutoSnapRange = null;
		}
	}

	private void Update()
	{
		bool s = false;
		if (!IsOccupied && !IsBusy && !IsRefilling)
		{
			for (int i = 0; i < objectsInRange.Count; i++)
			{
				if (objectsInRange[i] != null && objectsInRange[i].PickupableItem.IsCurrInHand && objectsInRange[i].CurrentlyAttachedTo == null && objectsInRange[i].enabled && objectsInRange[i].CachedLastClosestAttachpoint == this)
				{
					s = true;
					break;
				}
			}
		}
		SetHighlightState(s);
	}

	public virtual Vector3 GetPoint()
	{
		return GetPoint(Vector3.zero);
	}

	public virtual Vector3 GetPoint(Vector3 relativeTo)
	{
		return base.transform.position;
	}

	protected virtual void PlayAttachParticles(AttachableObject attached)
	{
		if (particleOnAttach != null)
		{
			particleOnAttach.transform.position = GetPoint(attached.transform.position);
			particleOnAttach.Play();
		}
		if (particleOnDetach != null)
		{
			particleOnDetach.Stop();
		}
	}

	protected virtual void PlayDetachParticles(AttachableObject detached)
	{
		if (particleOnDetach != null)
		{
			particleOnDetach.transform.position = GetPoint(detached.transform.position);
			particleOnDetach.Play();
		}
		if (particleOnAttach != null)
		{
			particleOnAttach.Stop();
		}
	}

	protected virtual IEnumerator RefillObjectAsync(bool immediate)
	{
		if (!immediate)
		{
			AudioClip clip = data.SoundWhenSomethingRefilled;
			if (clip != null)
			{
				AudioManager.Instance.Play(base.transform, clip, 1f, 1f);
			}
			float refillProgress = 0f;
			while (refillProgress < 1f)
			{
				if (growRefillingObject)
				{
					refillObj.transform.localScale = Vector3.one * Mathf.Max(0.1f, refillProgress);
					refillObj.ForceRealign();
				}
				refillProgress = Mathf.Min(refillProgress + Time.deltaTime / refillDuration, 1f);
				yield return null;
			}
		}
		refillObj.transform.localScale = Vector3.one;
		refillObj.ForceRealign();
	}

	private IEnumerator RefillAsync(bool immediate)
	{
		hasGeneratedRefillObject = false;
		isRefilling = true;
		col.enabled = false;
		if (!immediate && delayBeforeRefilling > 0f)
		{
			float timer = Time.time + delayBeforeRefilling;
			while (Time.time < timer)
			{
				if (IsOccupied)
				{
					if (refillWhenObjectReleasedFromHand)
					{
						RemoveTrackEventsFromObject(refillObj);
						refillObj = attachedObjects[0];
						AddTrackEventsToObject(refillObj);
					}
					col.enabled = true;
					isRefilling = false;
					yield break;
				}
				yield return null;
			}
		}
		if (CreateRefillObjectAndCheckForSuccess())
		{
			yield return StartCoroutine(RefillObjectAsync(immediate));
			FinishRefill();
		}
	}

	private bool CreateRefillObjectAndCheckForSuccess()
	{
		if (refillPrefabs.Length == 0)
		{
			col.enabled = true;
			isRefilling = false;
			hasGeneratedRefillObject = true;
			return false;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(refillPrefabs[refillObjectIndex]);
		gameObject.RemoveCloneFromName();
		BasePrefabSpawner component = gameObject.GetComponent<BasePrefabSpawner>();
		if (component != null)
		{
			gameObject = component.LastSpawnedPrefabGO;
		}
		if (refillObj != null && refillWhenObjectReleasedFromHand)
		{
			RemoveTrackEventsFromObject(refillObj);
		}
		refillObj = gameObject.GetComponent<AttachableObject>();
		if (refillObj == null)
		{
			Debug.LogError("Refill object must have AttachableObject component.");
			col.enabled = true;
			isRefilling = false;
			hasGeneratedRefillObject = true;
			return false;
		}
		refillObj.AttachTo(this, 0, true, true);
		oldItemInteractions = CacheItemInteractions();
		DisableItemInteractions();
		if (this.OnObjectWasRefilled != null)
		{
			this.OnObjectWasRefilled(this, refillObj);
		}
		hasGeneratedRefillObject = true;
		return true;
	}

	protected virtual void FinishRefill()
	{
		refillObj.transform.localScale = Vector3.one;
		SetItemInteractions(oldItemInteractions);
		if (refillPrefabs.Length > 1)
		{
			if (refillSelectionMethod == RefillSelectionMode.Cycle)
			{
				refillObjectIndex = (refillObjectIndex + 1) % refillPrefabs.Length;
			}
			else if (refillSelectionMethod == RefillSelectionMode.Random)
			{
				int num = refillObjectIndex;
				while (num == refillObjectIndex)
				{
					refillObjectIndex = UnityEngine.Random.Range(0, refillPrefabs.Length);
				}
			}
		}
		if (refillWhenObjectReleasedFromHand)
		{
			AddTrackEventsToObject(refillObj);
		}
		col.enabled = true;
		isRefilling = false;
	}

	private void AddTrackEventsToObject(AttachableObject obj)
	{
		PickupableItem obj2 = obj.PickupableItem;
		obj2.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj2.OnGrabbed, new Action<GrabbableItem>(TrackedRefillObjectGrabbed));
		PickupableItem obj3 = obj.PickupableItem;
		obj3.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Combine(obj3.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(TrackedRefillObjectReleased));
	}

	private void RemoveTrackEventsFromObject(AttachableObject obj)
	{
		PickupableItem obj2 = obj.PickupableItem;
		obj2.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj2.OnGrabbed, new Action<GrabbableItem>(TrackedRefillObjectGrabbed));
		PickupableItem obj3 = obj.PickupableItem;
		obj3.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Remove(obj3.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(TrackedRefillObjectReleased));
	}

	private void TrackedRefillObjectGrabbed(GrabbableItem grabbable)
	{
	}

	private void TrackedRefillObjectReleased(GrabbableItem grabbable)
	{
		if (refillObj.CurrentlyAttachedTo != this)
		{
			isWaitingForReleaseTimer = true;
			if (base.isActiveAndEnabled)
			{
				StartCoroutine(TrackedObjectReleasedAsync());
			}
		}
	}

	private IEnumerator TrackedObjectReleasedAsync()
	{
		yield return new WaitForEndOfFrame();
		float timer = 0f;
		while (timer < durationDroppedBeforeRefilled)
		{
			timer += Time.deltaTime;
			if (refillObj.PickupableItem.IsCurrInHand)
			{
				break;
			}
			yield return null;
		}
		isWaitingForReleaseTimer = false;
		if (timer >= durationDroppedBeforeRefilled)
		{
			StartCoroutine(RefillAsync(false));
		}
	}

	private CachedInteractionState[] CacheItemInteractions()
	{
		CachedInteractionState[] array = new CachedInteractionState[attachedObjects.Count];
		for (int i = 0; i < attachedObjects.Count; i++)
		{
			bool isEnabled = IsPickupEnabled(attachedObjects[i]);
			array[i] = new CachedInteractionState(attachedObjects[i], isEnabled);
		}
		return array;
	}

	protected void DisableItemInteractions()
	{
		for (int num = attachedObjects.Count - 1; num >= 0; num--)
		{
			SetPickupEnabled(attachedObjects[num], false);
		}
	}

	protected void SetItemInteractions()
	{
		SetItemInteractions(CacheItemInteractions());
	}

	protected virtual void SetItemInteractions(CachedInteractionState[] oldItemInteractions)
	{
		for (int num = oldItemInteractions.Length - 1; num >= 0; num--)
		{
			for (int num2 = attachedObjects.Count - 1; num2 >= 0; num2--)
			{
				if (attachedObjects[num2] == oldItemInteractions[num].AttachableObject)
				{
					SetPickupEnabled(attachedObjects[num2], oldItemInteractions[num].IsInteractable);
				}
			}
		}
	}

	protected bool IsPickupEnabled(AttachableObject o)
	{
		PickupableItem pickupableItem = o.PickupableItem;
		if (pickupableItem != null)
		{
			return pickupableItem.enabled;
		}
		return false;
	}

	protected void SetPickupEnabled(AttachableObject o, bool enablePickup)
	{
		PickupableItem pickupableItem = o.PickupableItem;
		if (pickupableItem != null)
		{
			pickupableItem.enabled = enablePickup;
		}
	}

	private void OnDrawGizmos()
	{
		if (IsOccupied)
		{
			Gizmos.color = new Color(0.5f, 1f, 0.5f, 0.5f);
		}
		else
		{
			Gizmos.color = new Color(0.5f, 0.25f, 0.5f, 0.5f);
		}
		Gizmos.DrawSphere(GetPoint(), 0.01f);
	}
}
