using Oculus.Platform;
using OwlchemyVR;
using UnityEngine;

public class LoadGameWhenReady : MonoBehaviour
{
	private bool startedLoad;

	public bool isThisSceneOfficeDemoLoader;

	private bool readyToContinue;

	[SerializeField]
	private GameObject[] oculusEntitlementErrorObjects;

	private void Start()
	{
		if (VRPlatform.GetCurrVRPlatformType() != VRPlatformTypes.Oculus)
		{
			return;
		}
		Core.Initialize("1069133196442024");
		Entitlements.IsUserEntitledToApplication().OnComplete(delegate(Message msg)
		{
			if (msg.IsError)
			{
				ShowEntitlementError();
			}
			else
			{
				readyToContinue = true;
			}
		});
	}

	private void ShowEntitlementError()
	{
		Debug.Log("Showing Entitlement Error");
		for (int i = 0; i < oculusEntitlementErrorObjects.Length; i++)
		{
			Object.Instantiate(oculusEntitlementErrorObjects[i]);
		}
	}

	private void Update()
	{
		if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.SteamVR)
		{
			SteamVR instance = SteamVR.instance;
		}
		if (startedLoad || !PlaySpaceSizeSelector.isComplete)
		{
			return;
		}
		if (VRPlatform.GetCurrVRPlatformType() != VRPlatformTypes.Oculus)
		{
			readyToContinue = true;
		}
		if (readyToContinue)
		{
			Debug.Log("Continuing with loading museum");
			if (isThisSceneOfficeDemoLoader)
			{
				Debug.Log("Load Office");
				TempBuildSettingsHolder.CurrentBuildType = TempBuildSettingsHolder.DemoType.Office5minDemo;
				LevelLoader.Instance.LoadSceneManualWithoutFade("Office");
			}
			else
			{
				Debug.Log("Load Museum");
				TempBuildSettingsHolder.CurrentBuildType = TempBuildSettingsHolder.DemoType.NoDemo;
				LevelLoader.Instance.LoadSceneManualWithoutFade("Museum");
			}
			startedLoad = true;
		}
	}
}
