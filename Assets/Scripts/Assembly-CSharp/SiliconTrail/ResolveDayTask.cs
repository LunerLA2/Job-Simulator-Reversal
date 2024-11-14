using System.Collections;

namespace SiliconTrail
{
	public class ResolveDayTask : Task
	{
		protected override IEnumerator Execute()
		{
			campaign.ResolveDay();
			program.UpdateStatsDisplay();
			PartyMember[] newDeadPartyMembers = campaign.GetNewDeadPartyMembers();
			foreach (PartyMember newDeadPartyMember in newDeadPartyMembers)
			{
				program.QueueTask(new InfoTask
				{
					GraphicName = "Death",
					InfoText = newDeadPartyMember.Name + " was deleted."
				});
			}
			if (campaign.NumLivingPartyMembers == 0)
			{
				EndingEvent ev = new EndingEvent
				{
					EndingCondition = EndingCondition.AllDead
				};
				ev.DoEvent(program);
			}
			else if (campaign.DaysLeft == 0)
			{
				EndingEvent ev2 = new EndingEvent
				{
					EndingCondition = EndingCondition.OutOfTime
				};
				ev2.DoEvent(program);
			}
			yield break;
		}
	}
}
