namespace Oculus.Platform
{
	public sealed class Request<T> : Request
	{
		public Request(ulong requestID)
			: base(requestID)
		{
		}

		public Request<T> OnComplete(Message<T>.Callback callback)
		{
			Callback.OnComplete(this, callback);
			return this;
		}
	}
	public class Request
	{
		public ulong RequestID { get; set; }

		public Request(ulong requestID)
		{
			RequestID = requestID;
		}

		public Request OnComplete(Message.Callback callback)
		{
			Callback.OnComplete(this, callback);
			return this;
		}

		public static void RunCallbacks(uint limit = 0)
		{
			if (limit == 0)
			{
				Callback.RunCallbacks();
			}
			else
			{
				Callback.RunLimitedCallbacks(limit);
			}
		}
	}
}
