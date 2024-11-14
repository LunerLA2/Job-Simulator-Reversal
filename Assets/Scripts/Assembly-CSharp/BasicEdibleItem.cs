using OwlchemyVR;
using UnityEngine;

public class BasicEdibleItem : EdibleItem
{
	[SerializeField]
	private ParticleSystem crumbParticle;

	[SerializeField]
	private int totalBitesRequired = 1;

	[SerializeField]
	private Transform movableColliderTransform;

	[SerializeField]
	private SelectedChangeOutlineController outline;

	[SerializeField]
	private GameObject[] visualProgression;

	[SerializeField]
	private Transform[] colliderOrientProgression;

	[SerializeField]
	private AttachableObject optionalAttachableObject;

	[SerializeField]
	private float[] thicknessProgression;

	[SerializeField]
	private bool dontDestroyWhenEaten;

	[SerializeField]
	private bool dontDetachOnBite;

	private int numBitesTaken;

	public bool IsFullyConsumed
	{
		get
		{
			return numBitesTaken >= totalBitesRequired;
		}
	}

	public int NumBitesTaken
	{
		get
		{
			return numBitesTaken;
		}
	}

	public override void Awake()
	{
		base.Awake();
		if (optionalAttachableObject == null)
		{
			optionalAttachableObject = GetComponent<AttachableObject>();
		}
	}

	private void Start()
	{
		SetNumberOfBitesTaken(0);
	}

	public override BiteResultInfo TakeBiteAndGetResult(HeadController head)
	{
		if (numBitesTaken >= totalBitesRequired)
		{
			return new BiteResultInfo(false, null, null);
		}
		AttachableObject component = GetComponent<AttachableObject>();
		if (component != null && component.CurrentlyAttachedTo != null)
		{
			if (component.CurrentlyAttachedTo.IsRefilling)
			{
				return new BiteResultInfo(false, null, null);
			}
			if (!dontDetachOnBite)
			{
				component.Detach();
			}
		}
		SetNumberOfBitesTaken(numBitesTaken + 1);
		if (crumbParticle != null)
		{
			crumbParticle.Play();
		}
		BiteTakenEvent();
		if (numBitesTaken >= totalBitesRequired)
		{
			if (crumbParticle != null)
			{
				crumbParticle.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
				Object.Destroy(crumbParticle.gameObject, 2f);
			}
			ItemConsumedEvent();
			if (!dontDestroyWhenEaten)
			{
				if (base.PickupableItem.CurrInteractableHand != null)
				{
					base.PickupableItem.CurrInteractableHand.ManuallyReleaseJoint();
				}
				Object.Destroy(base.gameObject, 0.001f);
			}
			return new BiteResultInfo(true, null, null);
		}
		if (base.PickupableItem.IsCurrInHand)
		{
			Quaternion rotation = Quaternion.LookRotation(-(base.transform.position - head.transform.position).normalized);
			base.PickupableItem.CurrInteractableHand.ReorientCurrItemInHand(base.transform.position, rotation);
		}
		return new BiteResultInfo(false, null, null);
	}

	public override void SetNumberOfBitesTaken(int num)
	{
		if (base.PickupableItem.CurrInteractableHand != null)
		{
			base.PickupableItem.CurrInteractableHand.UnIgnoreOtherHandGrabbleCollidersAndClearInOtherHand();
		}
		for (int i = 0; i < visualProgression.Length; i++)
		{
			visualProgression[i].SetActive(num == i);
		}
		if (movableColliderTransform != null)
		{
			for (int j = 0; j < colliderOrientProgression.Length; j++)
			{
				if (num == j)
				{
					movableColliderTransform.localPosition = colliderOrientProgression[j].localPosition;
					movableColliderTransform.localRotation = colliderOrientProgression[j].localRotation;
					movableColliderTransform.localScale = colliderOrientProgression[j].localScale;
				}
			}
		}
		if (optionalAttachableObject != null && num > 0 && thicknessProgression != null && thicknessProgression.Length > 0)
		{
			optionalAttachableObject.SetThickness(thicknessProgression[Mathf.Min(num - 1, thicknessProgression.Length - 1)]);
		}
		numBitesTaken = num;
		outline.ForceRefreshMeshes();
		if (base.PickupableItem.CurrInteractableHand != null)
		{
			base.PickupableItem.CurrInteractableHand.PhysicsIgnoreGrabbableInTheOtherHand();
		}
	}
}
