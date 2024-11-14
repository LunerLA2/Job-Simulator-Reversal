using System;
using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public abstract class Message<T> : Message
	{
		public new delegate void Callback(Message<T> message);

		private T data;

		public T Data
		{
			get
			{
				return data;
			}
		}

		public Message(IntPtr c_message)
			: base(c_message)
		{
			if (!base.IsError)
			{
				data = GetDataFromMessage(c_message);
			}
		}

		protected abstract T GetDataFromMessage(IntPtr c_message);
	}
	public class Message
	{
		public enum MessageType : uint
		{
			Unknown = 0u,
			Achievements_AddCount = 65495601u,
			Achievements_AddFields = 346693929u,
			Achievements_GetAllDefinitions = 64177549u,
			Achievements_GetAllProgress = 1335877149u,
			Achievements_GetDefinitionsByName = 1653670332u,
			Achievements_GetNextAchievementDefinitionArrayPage = 712888917u,
			Achievements_GetNextAchievementProgressArrayPage = 792913703u,
			Achievements_GetProgressByName = 354837425u,
			Achievements_Unlock = 1497156573u,
			ApplicationLifecycle_GetRegisteredPIDs = 82169698u,
			ApplicationLifecycle_GetSessionKey = 984570141u,
			ApplicationLifecycle_RegisterSessionKey = 1303818232u,
			Application_GetVersion = 1751583246u,
			CloudStorage_Delete = 685393261u,
			CloudStorage_GetNextCloudStorageMetadataArrayPage = 1544004335u,
			CloudStorage_Load = 1082420033u,
			CloudStorage_LoadBucketMetadata = 1931977997u,
			CloudStorage_LoadConflictMetadata = 1146770162u,
			CloudStorage_LoadHandle = 845863478u,
			CloudStorage_LoadMetadata = 65446546u,
			CloudStorage_ResolveKeepLocal = 811109637u,
			CloudStorage_ResolveKeepRemote = 1965400838u,
			CloudStorage_Save = 1270570030u,
			Entitlement_GetIsViewerEntitled = 409688241u,
			IAP_ConsumePurchase = 532378329u,
			IAP_GetNextProductArrayPage = 467225263u,
			IAP_GetNextPurchaseArrayPage = 1196886677u,
			IAP_GetProductsBySKU = 2124073717u,
			IAP_GetViewerPurchases = 974095385u,
			IAP_LaunchCheckoutFlow = 1067126029u,
			Leaderboard_GetEntries = 1572030284u,
			Leaderboard_GetEntriesAfterRank = 406293487u,
			Leaderboard_GetNextEntries = 1310751961u,
			Leaderboard_GetPreviousEntries = 1224858304u,
			Leaderboard_WriteEntry = 293587198u,
			Matchmaking_Browse = 509948616u,
			Matchmaking_Browse2 = 1715641947u,
			Matchmaking_Cancel = 543705519u,
			Matchmaking_Cancel2 = 285117908u,
			Matchmaking_CreateAndEnqueueRoom = 1615617480u,
			Matchmaking_CreateAndEnqueueRoom2 = 693889755u,
			Matchmaking_CreateRoom = 54203178u,
			Matchmaking_CreateRoom2 = 1231922052u,
			Matchmaking_Enqueue = 1086418033u,
			Matchmaking_Enqueue2 = 303174325u,
			Matchmaking_EnqueueRoom = 1888108644u,
			Matchmaking_EnqueueRoom2 = 1428741028u,
			Matchmaking_GetAdminSnapshot = 1008820116u,
			Matchmaking_GetStats = 1123849272u,
			Matchmaking_JoinRoom = 1295177725u,
			Matchmaking_ReportResultInsecure = 439800205u,
			Matchmaking_StartMatch = 1154746693u,
			Notification_GetNextRoomInviteNotificationArrayPage = 102890359u,
			Notification_GetRoomInvites = 1871801234u,
			Notification_MarkAsRead = 1903319523u,
			Party_GetCurrent = 1200830304u,
			Room_CreateAndJoinPrivate = 1977017207u,
			Room_Get = 1704628152u,
			Room_GetCurrent = 161916164u,
			Room_GetCurrentForUser = 234887141u,
			Room_GetInvitableUsers = 506615698u,
			Room_GetModeratedRooms = 159645047u,
			Room_GetNextRoomArrayPage = 1317239238u,
			Room_InviteUser = 1093266451u,
			Room_Join = 382373641u,
			Room_KickUser = 1233344310u,
			Room_LaunchInvitableUserFlow = 843047539u,
			Room_Leave = 1916281973u,
			Room_SetDescription = 809796911u,
			Room_UpdateDataStore = 40779816u,
			Room_UpdateMembershipLockStatus = 923514796u,
			Room_UpdateOwner = 850803997u,
			Room_UpdatePrivateRoomJoinPolicy = 289473179u,
			User_Get = 1808768583u,
			User_GetAccessToken = 111696574u,
			User_GetLoggedInUser = 1131361373u,
			User_GetLoggedInUserFriends = 1484532365u,
			User_GetNextUserArrayPage = 645723971u,
			User_GetOrgScopedID = 418426907u,
			User_GetUserProof = 578880643u,
			Voip_SetSystemVoipSuppressed = 1161808298u,
			Notification_Matchmaking_MatchFound = 197393623u,
			Notification_Networking_ConnectionStateChange = 1577243802u,
			Notification_Networking_PeerConnectRequest = 1295114959u,
			Notification_Networking_PingResult = 1360343058u,
			Notification_Room_InviteAccepted = 1829794225u,
			Notification_Room_RoomUpdate = 1626094639u,
			Notification_Voip_ConnectRequest = 908343318u,
			Notification_Voip_StateChange = 888120928u,
			Notification_Voip_SystemVoipState = 1490179237u
		}

		public delegate void Callback(Message message);

		internal delegate Message ExtraMessageTypesHandler(IntPtr messageHandle, MessageType message_type);

		private MessageType type;

		private ulong requestID;

		private Error error;

		public MessageType Type
		{
			get
			{
				return type;
			}
		}

		public bool IsError
		{
			get
			{
				return error != null;
			}
		}

		public ulong RequestID
		{
			get
			{
				return requestID;
			}
		}

		internal static ExtraMessageTypesHandler HandleExtraMessageTypes { private get; set; }

		public Message(IntPtr c_message)
		{
			type = CAPI.ovr_Message_GetType(c_message);
			bool flag = CAPI.ovr_Message_IsError(c_message);
			requestID = CAPI.ovr_Message_GetRequestID(c_message);
			if (flag)
			{
				IntPtr obj = CAPI.ovr_Message_GetError(c_message);
				error = new Error(CAPI.ovr_Error_GetCode(obj), CAPI.ovr_Error_GetMessage(obj), CAPI.ovr_Error_GetHttpCode(obj));
			}
			else if (Core.LogMessages)
			{
				string text = CAPI.ovr_Message_GetString(c_message);
				if (text != null)
				{
					Debug.Log(text);
				}
				else
				{
					Debug.Log(string.Format("null message string {0}", c_message));
				}
			}
		}

		~Message()
		{
		}

		public virtual Error GetError()
		{
			return error;
		}

		public virtual PingResult GetPingResult()
		{
			return null;
		}

		public virtual NetworkingPeer GetNetworkingPeer()
		{
			return null;
		}

		public virtual AchievementDefinitionList GetAchievementDefinitions()
		{
			return null;
		}

		public virtual AchievementProgressList GetAchievementProgressList()
		{
			return null;
		}

		public virtual AchievementUpdate GetAchievementUpdate()
		{
			return null;
		}

		public virtual ApplicationVersion GetApplicationVersion()
		{
			return null;
		}

		public virtual CloudStorageConflictMetadata GetCloudStorageConflictMetadata()
		{
			return null;
		}

		public virtual CloudStorageData GetCloudStorageData()
		{
			return null;
		}

		public virtual CloudStorageMetadata GetCloudStorageMetadata()
		{
			return null;
		}

		public virtual CloudStorageMetadataList GetCloudStorageMetadataList()
		{
			return null;
		}

		public virtual CloudStorageUpdateResponse GetCloudStorageUpdateResponse()
		{
			return null;
		}

		public virtual InstalledApplicationList GetInstalledApplicationList()
		{
			return null;
		}

		public virtual bool GetLeaderboardDidUpdate()
		{
			return false;
		}

		public virtual LeaderboardEntryList GetLeaderboardEntryList()
		{
			return null;
		}

		public virtual LivestreamingStatus GetLivestreamingStatus()
		{
			return null;
		}

		public virtual MatchmakingAdminSnapshot GetMatchmakingAdminSnapshot()
		{
			return null;
		}

		public virtual MatchmakingBrowseResult GetMatchmakingBrowseResult()
		{
			return null;
		}

		public virtual MatchmakingEnqueueResult GetMatchmakingEnqueueResult()
		{
			return null;
		}

		public virtual MatchmakingEnqueueResultAndRoom GetMatchmakingEnqueueResultAndRoom()
		{
			return null;
		}

		public virtual MatchmakingStats GetMatchmakingStats()
		{
			return null;
		}

		public virtual OrgScopedID GetOrgScopedID()
		{
			return null;
		}

		public virtual Party GetParty()
		{
			return null;
		}

		public virtual PidList GetPidList()
		{
			return null;
		}

		public virtual ProductList GetProductList()
		{
			return null;
		}

		public virtual Purchase GetPurchase()
		{
			return null;
		}

		public virtual PurchaseList GetPurchaseList()
		{
			return null;
		}

		public virtual Room GetRoom()
		{
			return null;
		}

		public virtual RoomInviteNotificationList GetRoomInviteNotificationList()
		{
			return null;
		}

		public virtual RoomList GetRoomList()
		{
			return null;
		}

		public virtual string GetString()
		{
			return null;
		}

		public virtual SystemVoipState GetSystemVoipState()
		{
			return null;
		}

		public virtual User GetUser()
		{
			return null;
		}

		public virtual UserList GetUserList()
		{
			return null;
		}

		public virtual UserProof GetUserProof()
		{
			return null;
		}

		internal static Message ParseMessageHandle(IntPtr messageHandle)
		{
			if (messageHandle.ToInt64() == 0L)
			{
				return null;
			}
			Message message = null;
			MessageType messageType = CAPI.ovr_Message_GetType(messageHandle);
			switch (messageType)
			{
			case MessageType.Achievements_GetAllDefinitions:
			case MessageType.Achievements_GetNextAchievementDefinitionArrayPage:
			case MessageType.Achievements_GetDefinitionsByName:
				message = new MessageWithAchievementDefinitions(messageHandle);
				break;
			case MessageType.Achievements_GetProgressByName:
			case MessageType.Achievements_GetNextAchievementProgressArrayPage:
			case MessageType.Achievements_GetAllProgress:
				message = new MessageWithAchievementProgressList(messageHandle);
				break;
			case MessageType.Achievements_AddCount:
			case MessageType.Achievements_AddFields:
			case MessageType.Achievements_Unlock:
				message = new MessageWithAchievementUpdate(messageHandle);
				break;
			case MessageType.Application_GetVersion:
				message = new MessageWithApplicationVersion(messageHandle);
				break;
			case MessageType.CloudStorage_LoadConflictMetadata:
				message = new MessageWithCloudStorageConflictMetadata(messageHandle);
				break;
			case MessageType.CloudStorage_LoadHandle:
			case MessageType.CloudStorage_Load:
				message = new MessageWithCloudStorageData(messageHandle);
				break;
			case MessageType.CloudStorage_LoadMetadata:
				message = new MessageWithCloudStorageMetadataUnderLocal(messageHandle);
				break;
			case MessageType.CloudStorage_GetNextCloudStorageMetadataArrayPage:
			case MessageType.CloudStorage_LoadBucketMetadata:
				message = new MessageWithCloudStorageMetadataList(messageHandle);
				break;
			case MessageType.CloudStorage_Delete:
			case MessageType.CloudStorage_ResolveKeepLocal:
			case MessageType.CloudStorage_Save:
			case MessageType.CloudStorage_ResolveKeepRemote:
				message = new MessageWithCloudStorageUpdateResponse(messageHandle);
				break;
			case MessageType.Matchmaking_Cancel2:
			case MessageType.Entitlement_GetIsViewerEntitled:
			case MessageType.Matchmaking_ReportResultInsecure:
			case MessageType.IAP_ConsumePurchase:
			case MessageType.Matchmaking_Cancel:
			case MessageType.Room_LaunchInvitableUserFlow:
			case MessageType.Room_UpdateOwner:
			case MessageType.Matchmaking_StartMatch:
			case MessageType.ApplicationLifecycle_RegisterSessionKey:
			case MessageType.Notification_MarkAsRead:
				message = new Message(messageHandle);
				break;
			case MessageType.Leaderboard_GetEntriesAfterRank:
			case MessageType.Leaderboard_GetPreviousEntries:
			case MessageType.Leaderboard_GetNextEntries:
			case MessageType.Leaderboard_GetEntries:
				message = new MessageWithLeaderboardEntryList(messageHandle);
				break;
			case MessageType.Leaderboard_WriteEntry:
				message = new MessageWithLeaderboardDidUpdate(messageHandle);
				break;
			case MessageType.Matchmaking_GetAdminSnapshot:
				message = new MessageWithMatchmakingAdminSnapshot(messageHandle);
				break;
			case MessageType.Matchmaking_Browse:
			case MessageType.Matchmaking_Browse2:
				message = new MessageWithMatchmakingBrowseResult(messageHandle);
				break;
			case MessageType.Matchmaking_Enqueue2:
			case MessageType.Matchmaking_Enqueue:
			case MessageType.Matchmaking_EnqueueRoom2:
			case MessageType.Matchmaking_EnqueueRoom:
				message = new MessageWithMatchmakingEnqueueResult(messageHandle);
				break;
			case MessageType.Matchmaking_CreateAndEnqueueRoom2:
			case MessageType.Matchmaking_CreateAndEnqueueRoom:
				message = new MessageWithMatchmakingEnqueueResultAndRoom(messageHandle);
				break;
			case MessageType.Matchmaking_GetStats:
				message = new MessageWithMatchmakingStatsUnderMatchmakingStats(messageHandle);
				break;
			case MessageType.User_GetOrgScopedID:
				message = new MessageWithOrgScopedID(messageHandle);
				break;
			case MessageType.Party_GetCurrent:
				message = new MessageWithPartyUnderCurrentParty(messageHandle);
				break;
			case MessageType.ApplicationLifecycle_GetRegisteredPIDs:
				message = new MessageWithPidList(messageHandle);
				break;
			case MessageType.IAP_GetNextProductArrayPage:
			case MessageType.IAP_GetProductsBySKU:
				message = new MessageWithProductList(messageHandle);
				break;
			case MessageType.IAP_LaunchCheckoutFlow:
				message = new MessageWithPurchase(messageHandle);
				break;
			case MessageType.IAP_GetViewerPurchases:
			case MessageType.IAP_GetNextPurchaseArrayPage:
				message = new MessageWithPurchaseList(messageHandle);
				break;
			case MessageType.Room_Get:
				message = new MessageWithRoom(messageHandle);
				break;
			case MessageType.Room_GetCurrent:
			case MessageType.Room_GetCurrentForUser:
				message = new MessageWithRoomUnderCurrentRoom(messageHandle);
				break;
			case MessageType.Room_UpdateDataStore:
			case MessageType.Matchmaking_CreateRoom:
			case MessageType.Room_UpdatePrivateRoomJoinPolicy:
			case MessageType.Room_Join:
			case MessageType.Room_SetDescription:
			case MessageType.Room_UpdateMembershipLockStatus:
			case MessageType.Room_InviteUser:
			case MessageType.Matchmaking_CreateRoom2:
			case MessageType.Room_KickUser:
			case MessageType.Matchmaking_JoinRoom:
			case MessageType.Notification_Room_RoomUpdate:
			case MessageType.Room_Leave:
			case MessageType.Room_CreateAndJoinPrivate:
				message = new MessageWithRoomUnderViewerRoom(messageHandle);
				break;
			case MessageType.Room_GetModeratedRooms:
			case MessageType.Room_GetNextRoomArrayPage:
				message = new MessageWithRoomList(messageHandle);
				break;
			case MessageType.Notification_GetNextRoomInviteNotificationArrayPage:
			case MessageType.Notification_GetRoomInvites:
				message = new MessageWithRoomInviteNotificationList(messageHandle);
				break;
			case MessageType.User_GetAccessToken:
			case MessageType.ApplicationLifecycle_GetSessionKey:
			case MessageType.Notification_Room_InviteAccepted:
				message = new MessageWithString(messageHandle);
				break;
			case MessageType.Voip_SetSystemVoipSuppressed:
				message = new MessageWithSystemVoipState(messageHandle);
				break;
			case MessageType.User_GetLoggedInUser:
			case MessageType.User_Get:
				message = new MessageWithUser(messageHandle);
				break;
			case MessageType.Room_GetInvitableUsers:
			case MessageType.User_GetNextUserArrayPage:
			case MessageType.User_GetLoggedInUserFriends:
				message = new MessageWithUserList(messageHandle);
				break;
			case MessageType.User_GetUserProof:
				message = new MessageWithUserProof(messageHandle);
				break;
			case MessageType.Notification_Networking_PeerConnectRequest:
			case MessageType.Notification_Networking_ConnectionStateChange:
				message = new MessageWithNetworkingPeer(messageHandle);
				break;
			case MessageType.Notification_Networking_PingResult:
				message = new MessageWithPingResult(messageHandle);
				break;
			case MessageType.Notification_Matchmaking_MatchFound:
				message = new MessageWithMatchmakingNotification(messageHandle);
				break;
			case MessageType.Notification_Voip_StateChange:
			case MessageType.Notification_Voip_ConnectRequest:
				message = new MessageWithNetworkingPeer(messageHandle);
				break;
			case MessageType.Notification_Voip_SystemVoipState:
				message = new MessageWithSystemVoipState(messageHandle);
				break;
			default:
				message = PlatformInternal.ParseMessageHandle(messageHandle, messageType);
				if (message == null)
				{
					Debug.LogError(string.Format("Unrecognized message type {0}\n", messageType));
				}
				break;
			}
			return message;
		}

		public static Message PopMessage()
		{
			if (!Core.IsInitialized())
			{
				return null;
			}
			IntPtr intPtr = CAPI.ovr_PopMessage();
			Message result = ParseMessageHandle(intPtr);
			CAPI.ovr_FreeMessage(intPtr);
			return result;
		}
	}
}
