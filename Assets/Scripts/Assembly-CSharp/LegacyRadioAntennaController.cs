using OwlchemyVR;
using TMPro;
using UnityEngine;

public class LegacyRadioAntennaController : MonoBehaviour
{
	private const int musicChannel = 2;

	private const float volumeToActivateEvent = 0.5f;

	[SerializeField]
	private GrabbableHinge volumeKnob;

	private float radioVolumeKnob = 1f;

	[SerializeField]
	private Transform channelSlider;

	[SerializeField]
	private float sliderLowX;

	[SerializeField]
	private float sliderHighX;

	[SerializeField]
	private Transform antennaBase;

	[SerializeField]
	private Transform anteenaTop;

	private float antennaValue;

	private float staticStrength;

	[SerializeField]
	private RadioChannelController[] channels = new RadioChannelController[4];

	[SerializeField]
	private TextMeshPro channelNumberText;

	[SerializeField]
	private TextMeshPro channelInfoText;

	private float[] channelSweetSpots = new float[4] { 0.125f, 0.375f, 0.625f, 0.875f };

	[SerializeField]
	private AudioSourceHelper staticSource;

	[SerializeField]
	private WorldItem worldItem;

	private bool isActivated;

	private int lastClosestChannel = -1;

	private void Update()
	{
		float num = radioVolumeKnob;
		radioVolumeKnob = volumeKnob.NormalizedAngle;
		antennaValue = Quaternion.LookRotation(Vector3.up, anteenaTop.position - antennaBase.position).eulerAngles.y / 360f;
		int num2 = 0;
		float num3 = 1f;
		for (int i = 0; i < channelSweetSpots.Length; i++)
		{
			float num4 = antennaValue - channelSweetSpots[i];
			if (num4 < 0f)
			{
				num4 *= -1f;
			}
			if (num3 > num4)
			{
				num3 = num4;
				num2 = i;
			}
		}
		if (lastClosestChannel != num2)
		{
			if (num2 == 2)
			{
				GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "OPENED");
			}
			else
			{
				GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "CLOSED");
			}
			lastClosestChannel = num2;
		}
		staticSource.SetVolume(num3 * 10f * radioVolumeKnob);
		SetChannelVolume(num2, 1f - num3 * 10f);
		SetChannelInfo(num2);
		channelSlider.transform.localPosition = new Vector3(Mathf.Lerp(sliderLowX, sliderHighX, antennaValue), channelSlider.localPosition.y, channelSlider.localPosition.z);
		if (num == radioVolumeKnob)
		{
			return;
		}
		if (!isActivated)
		{
			if (radioVolumeKnob >= 0.5f)
			{
				GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "ACTIVATED");
				isActivated = true;
			}
		}
		else if (radioVolumeKnob < 0.5f)
		{
			GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "DEACTIVATED");
			isActivated = false;
		}
	}

	private void SetChannelVolume(int n, float volume)
	{
		for (int i = 0; i < channels.Length; i++)
		{
			channels[i].AudioSource.SetVolume(0f);
			if (i == n)
			{
				channels[i].AudioSource.SetVolume(volume * radioVolumeKnob * 0.25f);
			}
		}
	}

	private void SetChannelInfo(int channelIndex)
	{
		channelInfoText.text = channels[channelIndex].ChannelInfo;
		channelNumberText.text = channels[channelIndex].ChannelNumber;
	}
}
