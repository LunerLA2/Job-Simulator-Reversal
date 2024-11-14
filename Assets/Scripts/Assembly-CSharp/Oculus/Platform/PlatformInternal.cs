using System;

namespace Oculus.Platform
{
	public static class PlatformInternal
	{
		public enum MessageTypeInternal : uint
		{
			Application_GetInstalledApplications = 1376744524u,
			GraphAPI_Get = 822018158u,
			GraphAPI_Post = 1990567876u,
			HTTP_Get = 1874211363u,
			HTTP_GetToFile = 1317133401u,
			HTTP_Post = 1798743375u,
			Livestreaming_GetStatus = 1218079125u,
			Party_Create = 450042703u,
			Party_GatherInApplication = 1921499523u,
			Party_Get = 1586058173u,
			Party_GetCurrentForUser = 1489764138u,
			Party_Invite = 901104867u,
			Party_Join = 1744993395u,
			Party_Leave = 848430801u,
			Room_CreateOrUpdateAndJoinNamed = 2089683601u,
			Room_GetNamedRooms = 125660812u,
			Room_GetSocialRooms = 1636310390u,
			User_NewEntitledTestUser = 292822787u,
			User_NewTestUser = 921194380u,
			User_NewTestUserFriends = 517416647u
		}

		public static void CrashApplication()
		{
			CAPI.ovr_CrashApplication();
		}

		internal static Message ParseMessageHandle(IntPtr messageHandle, Message.MessageType messageType)
		{
			Message result = null;
			switch ((MessageTypeInternal)messageType)
			{
			case MessageTypeInternal.Party_Leave:
				result = new Message(messageHandle);
				break;
			case MessageTypeInternal.Application_GetInstalledApplications:
				result = new MessageWithInstalledApplicationList(messageHandle);
				break;
			case MessageTypeInternal.Livestreaming_GetStatus:
				result = new MessageWithLivestreamingStatus(messageHandle);
				break;
			case MessageTypeInternal.Party_Get:
				result = new MessageWithParty(messageHandle);
				break;
			case MessageTypeInternal.Party_GetCurrentForUser:
				result = new MessageWithPartyUnderCurrentParty(messageHandle);
				break;
			case MessageTypeInternal.Room_CreateOrUpdateAndJoinNamed:
				result = new MessageWithRoomUnderViewerRoom(messageHandle);
				break;
			case MessageTypeInternal.Room_GetNamedRooms:
			case MessageTypeInternal.Room_GetSocialRooms:
				result = new MessageWithRoomList(messageHandle);
				break;
			case MessageTypeInternal.User_NewEntitledTestUser:
			case MessageTypeInternal.User_NewTestUserFriends:
			case MessageTypeInternal.GraphAPI_Get:
			case MessageTypeInternal.User_NewTestUser:
			case MessageTypeInternal.HTTP_GetToFile:
			case MessageTypeInternal.HTTP_Post:
			case MessageTypeInternal.HTTP_Get:
			case MessageTypeInternal.GraphAPI_Post:
				result = new MessageWithString(messageHandle);
				break;
			}
			return result;
		}
	}
}
