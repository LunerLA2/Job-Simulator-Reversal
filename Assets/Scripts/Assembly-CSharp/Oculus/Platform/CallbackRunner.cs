using System.Runtime.InteropServices;
using UnityEngine;

namespace Oculus.Platform
{
	public class CallbackRunner : MonoBehaviour
	{
		public bool IsPersistantBetweenSceneLoads = true;

		[DllImport("LibOVRPlatform64_1")]
		private static extern void ovr_UnityResetTestPlatform();

		private void Awake()
		{
			CallbackRunner callbackRunner = Object.FindObjectOfType<CallbackRunner>();
			if (callbackRunner != this)
			{
				Debug.LogWarning("You only need one instance of CallbackRunner");
			}
			if (IsPersistantBetweenSceneLoads)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		private void Update()
		{
			Request.RunCallbacks(0u);
		}

		private void OnDestroy()
		{
		}
	}
}
