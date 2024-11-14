using OwlchemyVR;
using UnityEngine;

public class SoundControlManager
{
	private const float MIN_IMPACT_VELOCITY = 0.5f;

	private const float MIN_REGULAR_IMPACT_VELOCITY = 2f;

	private const float HIGH_IMPACT_VELOCITY_VOLUME = 5f;

	public const string PLAYER_PREF_SHOW_DEBUG_IMPACT_AUDIO_MESSAGES = "ShowDebugImpactAudioMessages";

	private static bool wasInit;

	private static bool showDebugMessages;

	private static bool overrideAllImpactAudioWithImpactAudioData;

	private static ImpactAudioData overrideImpactAudioData;

	public static void SetImpactAudioDataOverride(ImpactAudioData impactAudioData)
	{
		overrideImpactAudioData = impactAudioData;
		overrideAllImpactAudioWithImpactAudioData = overrideImpactAudioData != null;
	}

	public static void ImpactSound(WorldItemData impactWorldItem, WorldItemData otherWorldItemData, SurfaceTypeData otherSurfaceTypeData, Vector3 contactPoint, float impactVelocity)
	{
		if (!(impactVelocity >= 0.5f))
		{
			return;
		}
		ImpactAudioData.ImpactAudioTypes impactAudioType = ImpactAudioData.ImpactAudioTypes.SlowImpact;
		if (impactVelocity > 2f)
		{
			impactAudioType = ImpactAudioData.ImpactAudioTypes.Impact;
		}
		ImpactAudioData appropriateImpactAudioData = GameSettings.Instance.ImpactAudioMatrix.GetAppropriateImpactAudioData(impactAudioType, impactWorldItem, otherWorldItemData, otherSurfaceTypeData);
		if (overrideAllImpactAudioWithImpactAudioData)
		{
			appropriateImpactAudioData = overrideImpactAudioData;
		}
		if (appropriateImpactAudioData != null)
		{
			float volume = Mathf.Min((impactVelocity - 0.5f) / 4.5f, 1f);
			AudioClip audioClipByType = appropriateImpactAudioData.GetAudioClipByType(impactAudioType);
			if (audioClipByType != null)
			{
				float pitch = Random.Range(0.9f, 1.1f);
				AudioManager.Instance.Play(contactPoint, audioClipByType, volume, pitch);
				return;
			}
			Debug.LogWarning(string.Concat("Missing Impact Audio:", impactWorldItem, "vs ", otherWorldItemData, ",", otherSurfaceTypeData));
		}
	}

	private static string GetDetailsOnItemVsItem(WorldItemData impactWorldItem, WorldItemData otherWorldItemData, SurfaceTypeData otherSurfaceTypeData)
	{
		string empty = string.Empty;
		if (impactWorldItem != null)
		{
			empty = empty + "WorldItemData(" + impactWorldItem.ItemName + ")";
			empty = ((!(impactWorldItem.SurfaceTypeData != null)) ? (empty + "-SurfaceType(Missing) vs ") : (empty + "-SurfaceType(" + impactWorldItem.SurfaceTypeData.SurfaceTypeName + ") vs "));
		}
		else
		{
			empty += "Unknown vs ";
		}
		if (otherWorldItemData != null)
		{
			empty = empty + "WorldItemData(" + otherWorldItemData.ItemName + ")";
			if (otherWorldItemData.SurfaceTypeData != null)
			{
				return empty + "-SurfaceType(" + otherWorldItemData.SurfaceTypeData.SurfaceTypeName + ")";
			}
			return empty + "-SurfaceType(Missing)";
		}
		if (otherSurfaceTypeData != null)
		{
			return empty + "SurfaceType(" + otherSurfaceTypeData.SurfaceTypeName + ")";
		}
		return empty + "Unknown";
	}
}
