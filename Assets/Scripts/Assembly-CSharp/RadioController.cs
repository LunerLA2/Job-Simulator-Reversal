using System;
using System.Collections;
using OwlchemyVR;
using TMPro;
using UnityEngine;

public class RadioController : MonoBehaviour
{
	[SerializeField]
	private GrabbableHinge volumeKnob;

	[SerializeField]
	private GrabbableItem volumeGrabbable;

	private float volumeValue;

	[SerializeField]
	private GrabbableHinge channelKnob;

	[SerializeField]
	private GrabbableItem channelGrabbable;

	private float channelValue;

	[SerializeField]
	private Transform channelSliderMesh;

	[SerializeField]
	private float sliderMinPos = 0.09f;

	[SerializeField]
	private float sliderMaxPos = -0.09f;

	[SerializeField]
	private RadioChannelController[] channels;

	private RadioChannelController cloestChannel;

	[SerializeField]
	private AudioSourceHelper staticSource;

	[SerializeField]
	private TextMeshPro channelNumberText;

	[SerializeField]
	private TextMeshPro channelInfoText;

	private bool isVolumeGrabbed;

	private bool isChannelGrabbed;

	private void OnEnable()
	{
		GrabbableItem grabbableItem = channelGrabbable;
		grabbableItem.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnReleased, new Action<GrabbableItem>(ChannelRelease));
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = channelGrabbable;
		grabbableItem.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnReleased, new Action<GrabbableItem>(ChannelRelease));
	}

	private void ChannelRelease(GrabbableItem item)
	{
		StartCoroutine(channelReleaseRoutine());
	}

	private IEnumerator channelReleaseRoutine()
	{
		yield return new WaitForEndOfFrame();
		UpdateRadio();
	}

	private IEnumerator Start()
	{
		UpdateRadio();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		UpdateRadio();
	}

	private void Update()
	{
		if (channelGrabbable.IsCurrInHand || volumeGrabbable.IsCurrInHand)
		{
			UpdateRadio();
		}
	}

	private void UpdateRadio()
	{
		volumeValue = volumeKnob.NormalizedAngle;
		channelValue = channelKnob.NormalizedAngle;
		for (int i = 0; i < channels.Length; i++)
		{
			float num = channels[i].ChannelPosition - channelValue;
			if (num < 0f)
			{
				num *= -1f;
			}
			float num2 = 0f;
			if (num < 0.165f)
			{
				num2 = 1f - num * 6.06f;
				staticSource.SetVolume(0.8f - num2);
				channelInfoText.text = channels[i].ChannelInfo;
				channelNumberText.text = channels[i].ChannelNumber;
				channels[i].AudioSource.SetVolume((num2 + 0.25f) * volumeValue);
			}
			else
			{
				channels[i].AudioSource.SetVolume(0f);
			}
		}
		Vector3 localPosition = channelSliderMesh.localPosition;
		localPosition.x = Mathf.Lerp(sliderMinPos, sliderMaxPos, channelValue);
		channelSliderMesh.localPosition = localPosition;
	}
}
