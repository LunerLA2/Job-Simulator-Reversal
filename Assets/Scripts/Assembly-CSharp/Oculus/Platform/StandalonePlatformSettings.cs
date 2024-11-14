using UnityEngine;

namespace Oculus.Platform
{
	public sealed class StandalonePlatformSettings : ScriptableObject
	{
		private const string OculusPlatformAccessTokenKey = "OculusPlatformAccessToken";

		public static string OculusPlatformAccessToken
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}
	}
}
