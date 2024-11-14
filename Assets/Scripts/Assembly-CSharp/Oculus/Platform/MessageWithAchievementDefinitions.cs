using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithAchievementDefinitions : Message<AchievementDefinitionList>
	{
		public MessageWithAchievementDefinitions(IntPtr c_message)
			: base(c_message)
		{
		}

		public override AchievementDefinitionList GetAchievementDefinitions()
		{
			return base.Data;
		}

		protected override AchievementDefinitionList GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr a = CAPI.ovr_Message_GetAchievementDefinitionArray(obj);
			return new AchievementDefinitionList(a);
		}
	}
}
