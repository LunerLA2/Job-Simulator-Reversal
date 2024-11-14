using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public static class Net
	{
		public static Packet ReadPacket()
		{
			if (!Core.IsInitialized())
			{
				return null;
			}
			IntPtr intPtr = CAPI.ovr_Net_ReadPacket();
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			return new Packet(intPtr);
		}

		public static bool SendPacket(ulong userID, byte[] bytes, SendPolicy policy)
		{
			if (Core.IsInitialized())
			{
				return CAPI.ovr_Net_SendPacket(userID, (UIntPtr)(ulong)bytes.Length, bytes, policy);
			}
			return false;
		}

		public static void Connect(ulong userID)
		{
			if (Core.IsInitialized())
			{
				CAPI.ovr_Net_Connect(userID);
			}
		}

		public static void Accept(ulong userID)
		{
			if (Core.IsInitialized())
			{
				CAPI.ovr_Net_Accept(userID);
			}
		}

		public static void Close(ulong userID)
		{
			if (Core.IsInitialized())
			{
				CAPI.ovr_Net_Close(userID);
			}
		}

		public static bool IsConnected(ulong userID)
		{
			return Core.IsInitialized() && CAPI.ovr_Net_IsConnected(userID);
		}

		public static bool SendPacketToCurrentRoom(byte[] bytes, SendPolicy policy)
		{
			if (Core.IsInitialized())
			{
				return CAPI.ovr_Net_SendPacketToCurrentRoom((UIntPtr)(ulong)bytes.Length, bytes, policy);
			}
			return false;
		}

		public static bool AcceptForCurrentRoom()
		{
			if (Core.IsInitialized())
			{
				return CAPI.ovr_Net_AcceptForCurrentRoom();
			}
			return false;
		}

		public static void CloseForCurrentRoom()
		{
			if (Core.IsInitialized())
			{
				CAPI.ovr_Net_CloseForCurrentRoom();
			}
		}

		public static Request<PingResult> Ping(ulong userID)
		{
			if (Core.IsInitialized())
			{
				return new Request<PingResult>(CAPI.ovr_Net_Ping(userID));
			}
			return null;
		}

		public static void SetPeerConnectRequestCallback(Message<NetworkingPeer>.Callback callback)
		{
			Callback.SetNotificationCallback(Message.MessageType.Notification_Networking_PeerConnectRequest, callback);
		}

		public static void SetConnectionStateChangedCallback(Message<NetworkingPeer>.Callback callback)
		{
			Callback.SetNotificationCallback(Message.MessageType.Notification_Networking_ConnectionStateChange, callback);
		}
	}
}
