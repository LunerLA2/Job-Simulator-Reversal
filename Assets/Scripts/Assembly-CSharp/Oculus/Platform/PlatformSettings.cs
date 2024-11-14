using UnityEngine;

namespace Oculus.Platform
{
	public sealed class PlatformSettings : ScriptableObject
	{
		[SerializeField]
		private string ovrAppID = string.Empty;

		[SerializeField]
		private string ovrMobileAppID = string.Empty;

		[SerializeField]
		private bool ovrUseStandalonePlatform = true;

		private static PlatformSettings instance;

		public static string AppID
		{
			get
			{
				return Instance.ovrAppID;
			}
			set
			{
				Instance.ovrAppID = value;
			}
		}

		public static string MobileAppID
		{
			get
			{
				return Instance.ovrMobileAppID;
			}
			set
			{
				Instance.ovrMobileAppID = value;
			}
		}

		public static bool UseStandalonePlatform
		{
			get
			{
				return Instance.ovrUseStandalonePlatform;
			}
			set
			{
				Instance.ovrUseStandalonePlatform = value;
			}
		}

		public static PlatformSettings Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Resources.Load<PlatformSettings>("OculusPlatformSettings");
					if (instance == null)
					{
						instance = ScriptableObject.CreateInstance<PlatformSettings>();
					}
				}
				return instance;
			}
			set
			{
				instance = value;
			}
		}
	}
}
