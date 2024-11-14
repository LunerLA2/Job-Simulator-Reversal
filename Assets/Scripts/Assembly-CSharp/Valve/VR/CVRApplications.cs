using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public class CVRApplications
	{
		private IVRApplications FnTable;

		internal CVRApplications(IntPtr pInterface)
		{
			FnTable = (IVRApplications)Marshal.PtrToStructure(pInterface, typeof(IVRApplications));
		}

		public EVRApplicationError AddApplicationManifest(string pchApplicationManifestFullPath, bool bTemporary)
		{
			return FnTable.AddApplicationManifest(pchApplicationManifestFullPath, bTemporary);
		}

		public EVRApplicationError RemoveApplicationManifest(string pchApplicationManifestFullPath)
		{
			return FnTable.RemoveApplicationManifest(pchApplicationManifestFullPath);
		}

		public bool IsApplicationInstalled(string pchAppKey)
		{
			return FnTable.IsApplicationInstalled(pchAppKey);
		}

		public uint GetApplicationCount()
		{
			return FnTable.GetApplicationCount();
		}

		public EVRApplicationError GetApplicationKeyByIndex(uint unApplicationIndex, string pchAppKeyBuffer, uint unAppKeyBufferLen)
		{
			return FnTable.GetApplicationKeyByIndex(unApplicationIndex, pchAppKeyBuffer, unAppKeyBufferLen);
		}

		public EVRApplicationError GetApplicationKeyByProcessId(uint unProcessId, string pchAppKeyBuffer, uint unAppKeyBufferLen)
		{
			return FnTable.GetApplicationKeyByProcessId(unProcessId, pchAppKeyBuffer, unAppKeyBufferLen);
		}

		public EVRApplicationError LaunchApplication(string pchAppKey)
		{
			return FnTable.LaunchApplication(pchAppKey);
		}

		public EVRApplicationError LaunchDashboardOverlay(string pchAppKey)
		{
			return FnTable.LaunchDashboardOverlay(pchAppKey);
		}

		public bool CancelApplicationLaunch(string pchAppKey)
		{
			return FnTable.CancelApplicationLaunch(pchAppKey);
		}

		public EVRApplicationError IdentifyApplication(uint unProcessId, string pchAppKey)
		{
			return FnTable.IdentifyApplication(unProcessId, pchAppKey);
		}

		public uint GetApplicationProcessId(string pchAppKey)
		{
			return FnTable.GetApplicationProcessId(pchAppKey);
		}

		public string GetApplicationsErrorNameFromEnum(EVRApplicationError error)
		{
			IntPtr ptr = FnTable.GetApplicationsErrorNameFromEnum(error);
			return (string)Marshal.PtrToStructure(ptr, typeof(string));
		}

		public uint GetApplicationPropertyString(string pchAppKey, EVRApplicationProperty eProperty, string pchPropertyValueBuffer, uint unPropertyValueBufferLen, ref EVRApplicationError peError)
		{
			return FnTable.GetApplicationPropertyString(pchAppKey, eProperty, pchPropertyValueBuffer, unPropertyValueBufferLen, ref peError);
		}

		public bool GetApplicationPropertyBool(string pchAppKey, EVRApplicationProperty eProperty, ref EVRApplicationError peError)
		{
			return FnTable.GetApplicationPropertyBool(pchAppKey, eProperty, ref peError);
		}

		public ulong GetApplicationPropertyUint64(string pchAppKey, EVRApplicationProperty eProperty, ref EVRApplicationError peError)
		{
			return FnTable.GetApplicationPropertyUint64(pchAppKey, eProperty, ref peError);
		}

		public EVRApplicationError SetApplicationAutoLaunch(string pchAppKey, bool bAutoLaunch)
		{
			return FnTable.SetApplicationAutoLaunch(pchAppKey, bAutoLaunch);
		}

		public bool GetApplicationAutoLaunch(string pchAppKey)
		{
			return FnTable.GetApplicationAutoLaunch(pchAppKey);
		}

		public EVRApplicationError GetStartingApplication(string pchAppKeyBuffer, uint unAppKeyBufferLen)
		{
			return FnTable.GetStartingApplication(pchAppKeyBuffer, unAppKeyBufferLen);
		}

		public EVRApplicationTransitionState GetTransitionState()
		{
			return FnTable.GetTransitionState();
		}

		public EVRApplicationError PerformApplicationPrelaunchCheck(string pchAppKey)
		{
			return FnTable.PerformApplicationPrelaunchCheck(pchAppKey);
		}

		public string GetApplicationsTransitionStateNameFromEnum(EVRApplicationTransitionState state)
		{
			IntPtr ptr = FnTable.GetApplicationsTransitionStateNameFromEnum(state);
			return (string)Marshal.PtrToStructure(ptr, typeof(string));
		}

		public bool IsQuitUserPromptRequested()
		{
			return FnTable.IsQuitUserPromptRequested();
		}

		public EVRApplicationError LaunchInternalProcess(string pchBinaryPath, string pchArguments, string pchWorkingDirectory)
		{
			return FnTable.LaunchInternalProcess(pchBinaryPath, pchArguments, pchWorkingDirectory);
		}
	}
}
