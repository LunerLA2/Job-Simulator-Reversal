using UnityEngine;

namespace OwlchemyVR
{
	public class SteamVRHmdDevice : MonoBehaviour, IHmdDevice
	{
		private SteamVR_TrackedObject trackedObj;

		private PlayerController playerController;

		private void OnEnable()
		{
			playerController = GetComponentInParent<PlayerController>();
			trackedObj = playerController.Hmd.gameObject.GetComponent<SteamVR_TrackedObject>();
		}

		private void OnDisable()
		{
			if (trackedObj != null)
			{
				trackedObj.enabled = false;
			}
		}

		public HmdState GetSuggestedHmdState(HMD hmd)
		{
			return null;
		}

		private void Update()
		{
		}
	}
}
