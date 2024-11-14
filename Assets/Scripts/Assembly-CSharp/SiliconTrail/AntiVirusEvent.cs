namespace SiliconTrail
{
	public class AntiVirusEvent : CampaignEvent
	{
		private const int VIRUS_REMOVAL_COST = 16;

		protected override bool DoEvent()
		{
			program.QueueTask(new ChoiceTask
			{
				GraphicName = "AntiVirus",
				InfoText = "You found an Anti-virus program. Remove all viruses from your party for " + 16 + " " + program.DataUnit + " of memory?",
				DefaultText = "It's a deal",
				DefaultAction = RemoveVirus,
				AlternateText = "No thanks",
				AlternateAction = CarryOn
			});
			return false;
		}

		private void RemoveVirus()
		{
			if (campaign.Money >= 16)
			{
				bool flag = false;
				PartyMember[] livingPartyMembers = campaign.LivingPartyMembers;
				foreach (PartyMember partyMember in livingPartyMembers)
				{
					if (partyMember.NumViruses > 0)
					{
						program.QueueTask(new InfoTask
						{
							GraphicName = "AntiVirus",
							InfoText = "Removed " + partyMember.NumViruses + " " + ((partyMember.NumViruses <= 1) ? "virus" : "viruses") + " from " + partyMember.Name + "."
						});
						flag = true;
						partyMember.ClearViruses();
					}
				}
				if (flag)
				{
					campaign.DecrementMoney(16);
					program.QueueTask(new InfoTask
					{
						GraphicName = "AntiVirus",
						InfoText = "Thanks for your business."
					});
				}
				else
				{
					program.QueueTask(new InfoTask
					{
						GraphicName = "AntiVirus",
						InfoText = "You had no viruses to remove!"
					});
				}
			}
			else
			{
				program.QueueTask(new InfoTask
				{
					GraphicName = "AntiVirus",
					InfoText = "You don't have enough memory! Scram!"
				});
			}
			isFinished = true;
		}

		private void CarryOn()
		{
			isFinished = true;
		}
	}
}
