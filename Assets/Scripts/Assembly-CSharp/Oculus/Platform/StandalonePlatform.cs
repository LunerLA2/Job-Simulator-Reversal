using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Oculus.Platform
{
	public sealed class StandalonePlatform
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void UnityLogDelegate(IntPtr tag, IntPtr msg);

		private void CPPLogCallback(IntPtr tag, IntPtr message)
		{
			Debug.Log(string.Format("{0}: {1}", Marshal.PtrToStringAnsi(tag), Marshal.PtrToStringAnsi(message)));
		}

		public bool InitializeInEditor()
		{
			if (string.IsNullOrEmpty(StandalonePlatformSettings.OculusPlatformAccessToken))
			{
				throw new UnityException("Update your access token by selecting 'Oculus Platform' -> 'Edit Settings'");
			}
			return Initialize(StandalonePlatformSettings.OculusPlatformAccessToken);
		}

		public bool Initialize(string accessToken)
		{
			CAPI.ovr_UnityResetTestPlatform();
			CAPI.ovr_UnityInitWrapperStandalone(accessToken, IntPtr.Zero);
			return true;
		}
	}
}
