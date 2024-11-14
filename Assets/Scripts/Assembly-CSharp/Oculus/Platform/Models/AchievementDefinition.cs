using System;

namespace Oculus.Platform.Models
{
	public class AchievementDefinition
	{
		public readonly AchievementType Type;

		public readonly string Name;

		public readonly uint BitfieldLength;

		public readonly ulong Target;

		public AchievementDefinition(IntPtr o)
		{
			Type = CAPI.ovr_AchievementDefinition_GetType(o);
			Name = CAPI.ovr_AchievementDefinition_GetName(o);
			BitfieldLength = CAPI.ovr_AchievementDefinition_GetBitfieldLength(o);
			Target = CAPI.ovr_AchievementDefinition_GetTarget(o);
		}
	}
}
