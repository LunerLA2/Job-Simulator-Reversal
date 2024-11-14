using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public static class Voip
	{
		public static void Start(ulong userID)
		{
			if (Core.IsInitialized())
			{
				CAPI.ovr_Voip_Start(userID);
			}
		}

		public static void Accept(ulong userID)
		{
			if (Core.IsInitialized())
			{
				CAPI.ovr_Voip_Accept(userID);
			}
		}

		public static void Stop(ulong userID)
		{
			if (Core.IsInitialized())
			{
				CAPI.ovr_Voip_Stop(userID);
			}
		}

		public static void SetVoipConnectRequestCallback(Message<NetworkingPeer>.Callback callback)
		{
			if (Core.IsInitialized())
			{
				Callback.SetNotificationCallback(Message.MessageType.Notification_Voip_ConnectRequest, callback);
			}
		}

		public static void SetVoipStateChangeCallback(Message<NetworkingPeer>.Callback callback)
		{
			if (Core.IsInitialized())
			{
				Callback.SetNotificationCallback(Message.MessageType.Notification_Voip_StateChange, callback);
			}
		}

		public static void SetMicrophoneFilterCallback(CAPI.FilterCallback callback)
		{
			if (Core.IsInitialized())
			{
				CAPI.ovr_Voip_SetMicrophoneFilterCallbackWithFixedSizeBuffer(callback, (UIntPtr)480uL);
			}
		}

		public static void SetMicrophoneMuted(VoipMuteState state)
		{
			if (Core.IsInitialized())
			{
				CAPI.ovr_Voip_SetMicrophoneMuted(state);
			}
		}

		public static VoipMuteState GetSystemVoipMicrophoneMuted()
		{
			if (Core.IsInitialized())
			{
				return CAPI.ovr_Voip_GetSystemVoipMicrophoneMuted();
			}
			return VoipMuteState.Unknown;
		}

		public static SystemVoipStatus GetSystemVoipStatus()
		{
			if (Core.IsInitialized())
			{
				return CAPI.ovr_Voip_GetSystemVoipStatus();
			}
			return SystemVoipStatus.Unknown;
		}

		public static Request<SystemVoipState> SetSystemVoipSuppressed(bool suppressed)
		{
			if (Core.IsInitialized())
			{
				return new Request<SystemVoipState>(CAPI.ovr_Voip_SetSystemVoipSuppressed(suppressed));
			}
			return null;
		}

		public static void SetSystemVoipStateNotificationCallback(Message<SystemVoipState>.Callback callback)
		{
			if (Core.IsInitialized())
			{
				Callback.SetNotificationCallback(Message.MessageType.Notification_Voip_SystemVoipState, callback);
			}
		}
	}
}
