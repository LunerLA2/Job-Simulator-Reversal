using System;
using OwlchemyVR;
using OwlchemyVR2;
using UnityEngine;

public class FidgetSpinner : MonoBehaviour
{
	public IndirectGrabbableSpinner spinner;

	private Rigidbody rb;

	public GrabbableItem grabbableSpinner;

	public PickupableItem pickupableFidget;

	public Transform middleCollider;

	public Transform middleColliderInteractable;

	public Transform[] colliderTransforms;

	public MeshRenderer SpinnerRenderer;

	private Vector3 down = new Vector3(0f, -1f, 0f);

	private bool doFixedUpdateFix;

	private RaycastHit[] collisionHits = new RaycastHit[10];

	private RaycastHit[] stackingHits = new RaycastHit[1];

	private Vector3 smallSizeInteractable = new Vector3(0.1f, 0.1f, 0.1f);

	private Vector3 regularSizeInteractable = new Vector3(1f, 1f, 1f);

	private Quaternion prevRot;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		pickupableFidget = GetComponent<PickupableItem>();
	}

	protected virtual void OnEnable()
	{
		PickupableItem pickupableItem = pickupableFidget;
		pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(pickupableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem pickupableItem2 = pickupableFidget;
		pickupableItem2.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Combine(pickupableItem2.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(Released));
	}

	protected virtual void OnDisable()
	{
		PickupableItem pickupableItem = pickupableFidget;
		pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(pickupableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem pickupableItem2 = pickupableFidget;
		pickupableItem2.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Remove(pickupableItem2.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(Released));
	}

	private void Grabbed(GrabbableItem item)
	{
		EnableSpinnerGrabbable(true);
	}

	private void Released(GrabbableItem item)
	{
		EnableSpinnerGrabbable(false);
		StopHaptics();
	}

	private void EnableSpinnerGrabbable(bool doEnable)
	{
		grabbableSpinner.enabled = doEnable;
		middleColliderInteractable.localScale = ((!doEnable) ? regularSizeInteractable : smallSizeInteractable);
	}

	private void Update()
	{
		float overallSpinSpeed = spinner.overallSpinSpeed;
		rb.angularDrag = AngularDragOverTime(overallSpinSpeed);
		RaycastForStacking();
		if (overallSpinSpeed > 0.001f || overallSpinSpeed < -0.001f)
		{
			RaycastForCollisions();
			UpdateHaptics();
		}
		prevRot = base.transform.rotation;
	}

	private void UpdateHaptics()
	{
		float angularVelocityMagnitude = GetAngularVelocityMagnitude();
		if (pickupableFidget.IsCurrInHand)
		{
			pickupableFidget.CurrInteractableHand.HapticsController.BeginBasicHaptic((float)Mathf.Clamp(60 * Mathf.FloorToInt(angularVelocityMagnitude), 20, 300) * Mathf.Clamp(Mathf.Abs(spinner.overallSpinSpeed) / 20f, 0f, 1f), 0.02f);
		}
	}

	public void StopHaptics()
	{
		pickupableFidget.CurrInteractableHand.HapticsController.ManuallyClearHaptics();
	}

	private float GetAngularVelocityMagnitude()
	{
		Quaternion rotation = base.transform.rotation;
		Quaternion quaternion = rotation * Quaternion.Inverse(prevRot);
		float num = 2f * Mathf.Acos(quaternion.w);
		float x = quaternion.x / Mathf.Sqrt(1f - quaternion.w * quaternion.w);
		float y = quaternion.y / Mathf.Sqrt(1f - quaternion.w * quaternion.w);
		float z = quaternion.z / Mathf.Sqrt(1f - quaternion.w * quaternion.w);
		return (new Vector3(x, y, z) * num * (1f / Time.deltaTime)).sqrMagnitude;
	}

	private void RaycastForStacking()
	{
		Ray ray = new Ray(middleCollider.position, down);
		if (Physics.RaycastNonAlloc(ray, stackingHits, 0.01f, LayerMaskHelper.OnlyIncluding(LayerMask.NameToLayer("Interactable")), QueryTriggerInteraction.Collide) > 0)
		{
			doFixedUpdateFix = true;
		}
		else
		{
			doFixedUpdateFix = false;
		}
	}

	private void FixedUpdate()
	{
		if (doFixedUpdateFix)
		{
			rb.drag = 500f;
		}
		else
		{
			rb.drag = 0f;
		}
	}

	private float AngularDragOverTime(float drag)
	{
		drag = Mathf.Pow(Mathf.Abs(drag), 2f);
		if (drag < 50f && drag > 0f)
		{
			drag *= drag / 50f;
		}
		return Mathf.Max(drag, 0.05f);
	}

	private void RaycastForCollisions()
	{
		bool flag = false;
		for (int i = 0; i < colliderTransforms.Length; i++)
		{
			Ray ray = new Ray(colliderTransforms[i].position, colliderTransforms[i].forward);
			if (Physics.RaycastNonAlloc(ray, collisionHits, 0.02f, LayerMaskHelper.EverythingBut(), QueryTriggerInteraction.Ignore) > 0)
			{
				flag = true;
			}
		}
		if (flag)
		{
			float overallSpinSpeed = spinner.overallSpinSpeed;
			overallSpinSpeed = ((!(overallSpinSpeed > 20f) && !(overallSpinSpeed < -20f)) ? ((!(overallSpinSpeed > 10f) && !(overallSpinSpeed < -10f)) ? ((!(overallSpinSpeed > 5f) && !(overallSpinSpeed < -5f)) ? (overallSpinSpeed - overallSpinSpeed * 0.9f) : (overallSpinSpeed - overallSpinSpeed * 0.04f)) : (overallSpinSpeed - overallSpinSpeed * 0.0275f)) : (overallSpinSpeed - overallSpinSpeed * 0.015f));
			spinner.overallSpinSpeed = overallSpinSpeed;
		}
	}

	public void SetupArt(Material material, Color color)
	{
		GetComponent<MeshRenderer>().material = material;
		SpinnerRenderer.materials[0].SetColor("_DiffColor", color);
	}
}
