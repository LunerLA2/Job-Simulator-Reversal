using System;
using UnityEngine;

namespace OwlchemyVR
{
	[RequireComponent(typeof(InteractableItem))]
	public class GrabbableItem : MonoBehaviour
	{
		[SerializeField]
		private float breakForceMultiplier = 1f;

		private InteractableItem interactableItem;

		protected Rigidbody r;

		private InteractionHandController currInteractableHand;

		public Action<GrabbableItem> OnGrabbed;

		public Action<GrabbableItem> OnReleased;

		public Action<GrabbableItem> OnGrabbedUpdate;

		public Action<GrabbableItem> OnStartedUsing;

		public Action<GrabbableItem> OnStoppedUsing;

		public Action<GrabbableItem> OnReleasedWasNotSwappedBetweenHands;

		private bool isBeingUsed;

		private bool cachedUseGravity;

		public float BreakForceMultiplier
		{
			get
			{
				return breakForceMultiplier;
			}
		}

		public InteractableItem InteractableItem
		{
			get
			{
				return interactableItem;
			}
		}

		public Rigidbody Rigidbody
		{
			get
			{
				return r;
			}
		}

		public InteractionHandController CurrInteractableHand
		{
			get
			{
				return currInteractableHand;
			}
		}

		public bool IsCurrInHand
		{
			get
			{
				return currInteractableHand != null;
			}
		}

		public bool IsBeingUsed
		{
			get
			{
				return isBeingUsed;
			}
		}

		public bool CachedUseGravity
		{
			get
			{
				return cachedUseGravity;
			}
		}

		public virtual void Awake()
		{
			UpdateRigidbody();
			interactableItem = GetComponent<InteractableItem>();
			if (Rigidbody != null)
			{
				cachedUseGravity = Rigidbody.useGravity;
			}
		}

		public virtual void Start()
		{
		}

		public void UpdateRigidbody()
		{
			if (r == null)
			{
				r = GetComponent<Rigidbody>();
			}
		}

		public virtual void InHandUpdate()
		{
			if (OnGrabbedUpdate != null)
			{
				OnGrabbedUpdate(this);
			}
		}

		public virtual void Grab(InteractionHandController interactableHand)
		{
			if (!IsCurrInHand)
			{
				currInteractableHand = interactableHand;
				if (OnGrabbed != null)
				{
					OnGrabbed(this);
				}
				if (interactableItem.WorldItemData != null)
				{
					GameEventsManager.Instance.ItemActionOccurred(interactableItem.WorldItemData, "GRABBED");
				}
				if (Rigidbody != null)
				{
					cachedUseGravity = Rigidbody.useGravity;
					Rigidbody.useGravity = false;
				}
				interactableItem.ForceUnSelectAll();
			}
			else
			{
				Debug.LogWarning("Can not grab something when something is already in your hand");
			}
		}

		public void StartUsing()
		{
			if (IsCurrInHand && !isBeingUsed)
			{
				isBeingUsed = true;
				if (OnStartedUsing != null)
				{
					OnStartedUsing(this);
				}
			}
		}

		public void StopUsing()
		{
			if (isBeingUsed)
			{
				isBeingUsed = false;
				if (OnStoppedUsing != null)
				{
					OnStoppedUsing(this);
				}
			}
		}

		public virtual bool Release(InteractionHandController interactableHand, Vector3 applyVelocity, Vector3 applyAngVelocity, bool wasSwappedBetweenHands = false)
		{
			StopUsing();
			if (IsCurrInHand)
			{
				if (Rigidbody != null)
				{
					Rigidbody.useGravity = cachedUseGravity;
				}
				if (OnReleased != null)
				{
					OnReleased(this);
				}
				if (!wasSwappedBetweenHands && OnReleasedWasNotSwappedBetweenHands != null)
				{
					OnReleasedWasNotSwappedBetweenHands(this);
				}
				if (interactableItem.WorldItemData != null)
				{
					GameEventsManager.Instance.ItemActionOccurred(interactableItem.WorldItemData, "RELEASED");
				}
				currInteractableHand = null;
			}
			else
			{
				Debug.LogWarning("Can not release something that is not in a hand");
			}
			return true;
		}
	}
}
