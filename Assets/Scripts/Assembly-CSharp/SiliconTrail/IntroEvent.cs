namespace SiliconTrail
{
	public class IntroEvent : CampaignEvent
	{
		protected override bool DoEvent()
		{
			program.QueueTask(new CinematicTask
			{
				CinematicName = "Intro"
			});
			program.QueueTask(new InfoTask
			{
				GraphicName = "Reformat",
				InfoText = "Our universe will be reformatted in " + campaign.DaysLeft + " days!"
			});
			program.QueueTask(new InfoTask
			{
				GraphicName = "Internet",
				InfoText = "You must reach the Internet before then to make your escape!"
			});
			string text = string.Empty;
			for (int i = 0; i < campaign.NumPartyMembers; i++)
			{
				text = text + "\n- " + campaign.GetPartyMember(i).Name;
			}
			program.QueueTask(new InfoTask
			{
				GraphicName = "Internet",
				InfoText = "Bring your friends:" + text
			});
			program.QueueTask(new InfoTask
			{
				GraphicName = "Internet",
				InfoText = "To get there, you must face the dangers on the Silicon Trail!\nGood luck..."
			});
			return true;
		}
	}
}
