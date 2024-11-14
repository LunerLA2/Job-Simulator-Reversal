namespace SiliconTrail
{
	public class VirusEvent : CampaignEvent
	{
		protected override bool DoEvent()
		{
			PartyMember partyMember = campaign.SelectRandomLivingPartyMember();
			Virus virus = campaign.SelectRandomVirus();
			if (!partyMember.HasVirus(virus))
			{
				partyMember.CatchVirus(virus);
				program.QueueTask(new InfoTask
				{
					GraphicName = "Virus",
					InfoText = string.Format(virus.Message, partyMember.Name)
				});
			}
			return true;
		}
	}
}
