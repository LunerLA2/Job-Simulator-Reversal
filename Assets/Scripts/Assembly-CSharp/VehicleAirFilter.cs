using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class VehicleAirFilter : BlowableItem
{
	private const float BLOW_CLEAN_AMOUNT = 1f;

	[SerializeField]
	private AttachableObject attachableObject;

	[SerializeField]
	private AttachablePointData partsChooserAttachPointData;

	[SerializeField]
	private RotateAtSpeed fanRotation;

	[SerializeField]
	private float maxFanSpeed = 1000f;

	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private Material dirtyMaterial;

	private Material cleanMaterial;

	[SerializeField]
	private WorldItem worldItem;

	[SerializeField]
	private WorldItemData cleanFilterData;

	[SerializeField]
	private WorldItemData dirtyFilterData;

	[SerializeField]
	private ParticleSystem blowParticles;

	[SerializeField]
	private int durationAttachedToBecomeDirty = 6;

	private float totalAttachedDuration;

	private bool isDirty;

	private float cumulativeAmount;

	private bool isSpinning;

	private Vector3 fanSpeed;

	private new void Awake()
	{
		cleanMaterial = meshRenderer.material;
		fanSpeed = Vector3.zero;
	}

	private void Update()
	{
		if (!isDirty && attachableObject.CurrentlyAttachedTo != null && attachableObject.CurrentlyAttachedTo.Data != partsChooserAttachPointData)
		{
			totalAttachedDuration += Time.deltaTime;
			if (totalAttachedDuration > (float)durationAttachedToBecomeDirty)
			{
				isDirty = true;
				worldItem.ManualSetData(dirtyFilterData);
				cumulativeAmount = 0f;
				meshRenderer.material = dirtyMaterial;
				totalAttachedDuration = 0f;
			}
		}
	}

	private void OnEnable()
	{
		AttachableObject obj = attachableObject;
		obj.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(obj.OnAttach, new Action<AttachableObject, AttachablePoint>(Attached));
		AttachableObject obj2 = attachableObject;
		obj2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(obj2.OnDetach, new Action<AttachableObject, AttachablePoint>(Detached));
		OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Combine(OnWasBlown, new Action<BlowableItem, float, HeadController>(OnBlow));
	}

	private void OnDisable()
	{
		AttachableObject obj = attachableObject;
		obj.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(obj.OnAttach, new Action<AttachableObject, AttachablePoint>(Attached));
		AttachableObject obj2 = attachableObject;
		obj2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(obj2.OnDetach, new Action<AttachableObject, AttachablePoint>(Detached));
		OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Remove(OnWasBlown, new Action<BlowableItem, float, HeadController>(OnBlow));
	}

	public void SetDirty()
	{
		meshRenderer.material = dirtyMaterial;
		isDirty = true;
		worldItem.ManualSetData(dirtyFilterData);
	}

	public void SetRotationStatus(bool rotate)
	{
		isSpinning = rotate;
		StartCoroutine(SetFanStatusInternal(isSpinning));
	}

	private void Attached(AttachableObject obj, AttachablePoint point)
	{
	}

	private void Detached(AttachableObject obj, AttachablePoint point)
	{
		SetRotationStatus(false);
	}

	private IEnumerator SetFanStatusInternal(bool spinning)
	{
		float lerpFrom = 0f;
		float lerpTo = maxFanSpeed;
		if (!spinning)
		{
			lerpFrom = maxFanSpeed;
			lerpTo = 0f;
		}
		float lerpAmt = Mathf.InverseLerp(lerpFrom, lerpTo, fanSpeed.y);
		while (isSpinning == spinning && lerpAmt < 1f)
		{
			lerpAmt += Time.deltaTime;
			fanSpeed.y = Mathf.Lerp(lerpFrom, lerpTo, lerpAmt);
			fanRotation.SetSpeed(fanSpeed);
			yield return null;
		}
	}

	private void OnBlow(BlowableItem blowableItem, float amount, HeadController headController)
	{
		if (!isDirty)
		{
			return;
		}
		cumulativeAmount += amount;
		if (cumulativeAmount >= 1f)
		{
			isDirty = false;
			worldItem.ManualSetData(cleanFilterData);
			meshRenderer.material = cleanMaterial;
			blowParticles.Stop();
			GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "CLEANED");
			return;
		}
		if (!blowParticles.isPlaying)
		{
			blowParticles.Play();
		}
		blowParticles.transform.rotation = headController.transform.rotation;
		float amount2 = Mathf.Clamp(cumulativeAmount / 1f, 0f, 1f);
		GameEventsManager.Instance.ItemActionOccurredWithAmount(worldItem.Data, "CLEANED_PERC_CHANGE", amount2);
	}
}
