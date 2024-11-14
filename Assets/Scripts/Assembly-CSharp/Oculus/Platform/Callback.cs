using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Platform
{
	public static class Callback
	{
		private class RequestCallback
		{
			private Message.Callback messageCallback;

			public RequestCallback()
			{
			}

			public RequestCallback(Message.Callback callback)
			{
				messageCallback = callback;
			}

			public virtual void HandleMessage(Message msg)
			{
				if (messageCallback != null)
				{
					messageCallback(msg);
				}
			}
		}

		private sealed class RequestCallback<T> : RequestCallback
		{
			private Message<T>.Callback callback;

			public RequestCallback(Message<T>.Callback callback)
			{
				this.callback = callback;
			}

			public override void HandleMessage(Message msg)
			{
				if (callback != null)
				{
					if (!hasRegisteredRoomInviteNotificationHandler && msg.Type == Message.MessageType.Notification_Room_InviteAccepted)
					{
						pendingRoomInviteNotifications.Add(msg);
					}
					else if (msg is Message<T>)
					{
						callback((Message<T>)msg);
					}
					else
					{
						Debug.LogError("Unable to handle message: " + msg.GetType());
					}
				}
			}
		}

		private static Dictionary<ulong, RequestCallback> requestIDsToCallbacks = new Dictionary<ulong, RequestCallback>();

		private static Dictionary<Message.MessageType, RequestCallback> notificationCallbacks = new Dictionary<Message.MessageType, RequestCallback>();

		private static bool hasRegisteredRoomInviteNotificationHandler = false;

		private static List<Message> pendingRoomInviteNotifications = new List<Message>();

		internal static void SetNotificationCallback<T>(Message.MessageType type, Message<T>.Callback callback)
		{
			if (callback == null)
			{
				throw new Exception("Cannot provide a null notification callback.");
			}
			notificationCallbacks[type] = new RequestCallback<T>(callback);
			if (type == Message.MessageType.Notification_Room_InviteAccepted)
			{
				FlushRoomInviteNotificationQueue();
			}
		}

		internal static void SetNotificationCallback(Message.MessageType type, Message.Callback callback)
		{
			if (callback == null)
			{
				throw new Exception("Cannot provide a null notification callback.");
			}
			notificationCallbacks[type] = new RequestCallback(callback);
		}

		internal static void OnComplete<T>(Request<T> request, Message<T>.Callback callback)
		{
			requestIDsToCallbacks[request.RequestID] = new RequestCallback<T>(callback);
		}

		internal static void OnComplete(Request request, Message.Callback callback)
		{
			requestIDsToCallbacks[request.RequestID] = new RequestCallback(callback);
		}

		internal static void RunCallbacks()
		{
			while (true)
			{
				Message message = Message.PopMessage();
				if (message == null)
				{
					break;
				}
				HandleMessage(message);
			}
		}

		internal static void RunLimitedCallbacks(uint limit)
		{
			for (int i = 0; i < limit; i++)
			{
				Message message = Message.PopMessage();
				if (message == null)
				{
					break;
				}
				HandleMessage(message);
			}
		}

		private static void FlushRoomInviteNotificationQueue()
		{
			hasRegisteredRoomInviteNotificationHandler = true;
			foreach (Message pendingRoomInviteNotification in pendingRoomInviteNotifications)
			{
				HandleMessage(pendingRoomInviteNotification);
			}
			pendingRoomInviteNotifications.Clear();
		}

		private static void HandleMessage(Message msg)
		{
			RequestCallback value;
			if (requestIDsToCallbacks.TryGetValue(msg.RequestID, out value))
			{
				try
				{
					value.HandleMessage(msg);
					return;
				}
				finally
				{
					requestIDsToCallbacks.Remove(msg.RequestID);
				}
			}
			if (notificationCallbacks.TryGetValue(msg.Type, out value))
			{
				value.HandleMessage(msg);
			}
		}
	}
}
