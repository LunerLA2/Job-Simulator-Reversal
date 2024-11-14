using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithAchievementProgressList : Message<AchievementProgressList>
	{
		public MessageWithAchievementProgressList(IntPtr c_message)
			: base(c_message)
		{
		}

		public override AchievementProgressList GetAchievementProgressList()
		{
			return base.Data;
		}

		protected override AchievementProgressList GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr a = CAPI.ovr_Message_GetAchievementProgressArray(obj);
			return new AchievementProgressList(a);
		}
	}
}
