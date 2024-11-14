using System;
using UnityEngine;

namespace Oculus.Platform
{
	public sealed class Core
	{
		private static bool IsPlatformInitialized;

		public static bool LogMessages;

		public static bool IsInitialized()
		{
			return IsPlatformInitialized;
		}

		internal static void ForceInitialized()
		{
			IsPlatformInitialized = true;
		}

		public static void Initialize(string appId = null)
		{
			bool flag = UnityEngine.Application.platform == RuntimePlatform.WindowsEditor && !PlatformSettings.UseStandalonePlatform;
			if (!UnityEngine.Application.isEditor || flag)
			{
				string appIDFromConfig = GetAppIDFromConfig(flag);
				if (string.IsNullOrEmpty(appId))
				{
					if (string.IsNullOrEmpty(appIDFromConfig))
					{
						throw new UnityException("Update your app id by selecting 'Oculus Platform' -> 'Edit Settings'");
					}
					appId = appIDFromConfig;
				}
				else if (!string.IsNullOrEmpty(appIDFromConfig))
				{
					Debug.LogWarningFormat("The 'Oculus App Id ({0})' field in 'Oculus Platform/Edit Settings' is clobbering appId ({1}) that you passed in to Platform.Core.Init.  You should only specify this in one place.  We recommend the menu location.", appIDFromConfig, appId);
				}
			}
			if (flag)
			{
				WindowsPlatform windowsPlatform = new WindowsPlatform();
				IsPlatformInitialized = windowsPlatform.Initialize(appId);
			}
			else if (UnityEngine.Application.isEditor)
			{
				StandalonePlatform standalonePlatform = new StandalonePlatform();
				IsPlatformInitialized = standalonePlatform.InitializeInEditor();
			}
			else if (UnityEngine.Application.platform == RuntimePlatform.Android)
			{
				AndroidPlatform androidPlatform = new AndroidPlatform();
				IsPlatformInitialized = androidPlatform.Initialize(appId);
			}
			else
			{
				if (UnityEngine.Application.platform != RuntimePlatform.WindowsPlayer)
				{
					throw new NotImplementedException("Oculus platform is not implemented on this platform yet.");
				}
				WindowsPlatform windowsPlatform2 = new WindowsPlatform();
				IsPlatformInitialized = windowsPlatform2.Initialize(appId);
			}
			if (!IsPlatformInitialized)
			{
				throw new UnityException("Oculus Platform failed to initialize.");
			}
			if (LogMessages)
			{
				Debug.LogWarning("Oculus.Platform.Core.LogMessages is set to true. This will cause extra heap allocations, and should not be used outside of testing and debugging.");
			}
			new GameObject("Oculus.Platform.CallbackRunner").AddComponent<CallbackRunner>();
		}

		private static string GetAppIDFromConfig(bool forceWindows)
		{
			if (UnityEngine.Application.platform == RuntimePlatform.Android)
			{
				return PlatformSettings.MobileAppID;
			}
			if (UnityEngine.Application.platform == RuntimePlatform.WindowsPlayer || forceWindows)
			{
				return PlatformSettings.AppID;
			}
			return null;
		}
	}
}
