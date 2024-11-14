using System;
using OwlchemyVR;
using UnityEngine;

[RequireComponent(typeof(BlowableItem))]
[RequireComponent(typeof(PickupableItem))]
public class PartyHornController : MonoBehaviour
{
	[SerializeField]
	private float inflateSpeed = 2f;

	[SerializeField]
	private float deflateSpeed = 2.5f;

	[SerializeField]
	private float hornStartThreshold = 0.95f;

	[SerializeField]
	private float hornStopThreshold = 0.75f;

	[SerializeField]
	private AudioSourceHelper airAudio;

	[SerializeField]
	private AudioSourceHelper hornAudio;

	[SerializeField]
	private AudioSourceHelper paperAudio;

	[SerializeField]
	private Transform[] ribbonJoints;

	[SerializeField]
	private Transform[] ribbonBones;

	private BlowableItem blowable;

	private PickupableItem pickupable;

	private float[] initialJointAngles;

	private Vector3[] initialBoneScales;

	private float inflation;

	private bool isBlowing;

	private void Awake()
	{
		blowable = GetComponent<BlowableItem>();
		pickupable = GetComponent<PickupableItem>();
		initialJointAngles = new float[ribbonJoints.Length];
		initialBoneScales = new Vector3[ribbonBones.Length];
		for (int i = 0; i < ribbonJoints.Length; i++)
		{
			float num;
			for (num = ribbonJoints[i].localEulerAngles.x; num > 180f; num -= 360f)
			{
			}
			for (; num < -180f; num += 360f)
			{
			}
			initialJointAngles[i] = num;
			initialBoneScales[i] = ribbonBones[i].localScale;
		}
	}

	private void OnEnable()
	{
		BlowableItem blowableItem = blowable;
		blowableItem.OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Combine(blowableItem.OnWasBlown, new Action<BlowableItem, float, HeadController>(Blown));
	}

	private void OnDisable()
	{
		BlowableItem blowableItem = blowable;
		blowableItem.OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Remove(blowableItem.OnWasBlown, new Action<BlowableItem, float, HeadController>(Blown));
	}

	private void Update()
	{
		float num = inflation;
		if (isBlowing)
		{
			num = Mathf.Min(1f, inflation + inflateSpeed * Time.deltaTime);
			isBlowing = false;
			if (!airAudio.IsPlaying)
			{
				airAudio.Play();
			}
		}
		else
		{
			if (inflation > 0f)
			{
				num = Mathf.Max(0f, inflation - deflateSpeed * Time.deltaTime);
			}
			if (airAudio.IsPlaying)
			{
				airAudio.Stop();
			}
		}
		if (inflation < hornStartThreshold && num >= hornStartThreshold)
		{
			if (!hornAudio.IsPlaying)
			{
				hornAudio.Play();
			}
		}
		else if (inflation >= hornStopThreshold && num < hornStopThreshold && hornAudio.IsPlaying)
		{
			hornAudio.Stop();
		}
		if (inflation != num)
		{
			if (!paperAudio.IsPlaying)
			{
				paperAudio.Play();
			}
			UpdateRibbon();
		}
		else if (paperAudio.IsPlaying)
		{
			paperAudio.Stop();
		}
		inflation = num;
	}

	private void UpdateRibbon()
	{
		for (int i = 0; i < ribbonJoints.Length; i++)
		{
			Transform transform = ribbonJoints[i];
			Transform transform2 = ribbonBones[i];
			float t = inflation * (float)ribbonJoints.Length - (float)i;
			transform.localEulerAngles = new Vector3(Mathf.Lerp(initialJointAngles[i], 0f, t), 0f, 0f);
			transform2.localScale = Vector3.Lerp(initialBoneScales[i], Vector3.one, t);
		}
	}

	private void Blown(BlowableItem blowable, float amount, HeadController headController)
	{
		if (pickupable.IsCurrInHand)
		{
			isBlowing = true;
		}
	}
}
