using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(AudioListener))]
public class SteamVR_Ears : MonoBehaviour
{
	public SteamVR_Camera vrcam;

	private bool usingSpeakers;

	private Quaternion offset;

	private void OnNewPosesApplied(params object[] args)
	{
		Transform origin = vrcam.origin;
		Quaternion quaternion = ((!(origin != null)) ? Quaternion.identity : origin.rotation);
		base.transform.rotation = quaternion * offset;
	}

	private void OnEnable()
	{
		usingSpeakers = false;
		CVRSettings settings = OpenVR.Settings;
		if (settings != null)
		{
			EVRSettingsError peError = EVRSettingsError.None;
			if (settings.GetBool("steamvr", "usingSpeakers", false, ref peError))
			{
				usingSpeakers = true;
				float @float = settings.GetFloat("steamvr", "speakersForwardYawOffsetDegrees", 0f, ref peError);
				offset = Quaternion.Euler(0f, @float, 0f);
			}
		}
		if (usingSpeakers)
		{
			SteamVR_Utils.Event.Listen("new_poses_applied", OnNewPosesApplied);
		}
	}

	private void OnDisable()
	{
		if (usingSpeakers)
		{
			SteamVR_Utils.Event.Remove("new_poses_applied", OnNewPosesApplied);
		}
	}
}
