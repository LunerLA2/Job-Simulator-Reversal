using OwlchemyVR;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu]
public class AudioMixerSettings : ScriptableObject
{
	private const string SNAPSHOT_NAME_SUFFIX = "Snapshot";

	[SerializeField]
	private AudioMixer spatializerMixer;

	[SerializeField]
	private AudioMixer spatializerMixerPSVR;

	[SerializeField]
	private AudioMixerSnapshot defaultSnapshot;

	[SerializeField]
	private AudioMixerGroup defaultMixerGroup;

	[SerializeField]
	private AudioMixerSnapshot defaultSnapshotPSVR;

	[SerializeField]
	private AudioMixerGroup defaultMixerGroupPSVR;

	public AudioMixerGroup DefaultMixerGroup
	{
		get
		{
			return GetDefaultMixerGroupForPlatform();
		}
	}

	private AudioMixerGroup GetDefaultMixerGroupForPlatform()
	{
		if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.PSVR)
		{
			return defaultMixerGroupPSVR;
		}
		return defaultMixerGroup;
	}

	private AudioMixerSnapshot GetDefaultSnapshotForPlatform()
	{
		if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.PSVR)
		{
			return defaultSnapshotPSVR;
		}
		return defaultSnapshot;
	}

	public void SetSnapshotBySceneName(string sceneName)
	{
		SetNewSnapshotByName(sceneName + "Snapshot");
	}

	public void SetNewSnapshotByName(string snapshotName)
	{
		AudioMixerSnapshot audioMixerSnapshot = ((VRPlatform.GetCurrVRPlatformType() != VRPlatformTypes.PSVR) ? spatializerMixer.FindSnapshot(snapshotName) : spatializerMixerPSVR.FindSnapshot(snapshotName));
		if (audioMixerSnapshot == null)
		{
			audioMixerSnapshot = GetDefaultSnapshotForPlatform();
		}
		audioMixerSnapshot.TransitionTo(0f);
	}
}
