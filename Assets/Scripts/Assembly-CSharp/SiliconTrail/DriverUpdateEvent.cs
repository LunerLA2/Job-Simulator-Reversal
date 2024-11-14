namespace SiliconTrail
{
	public class DriverUpdateEvent : CampaignEvent
	{
		protected override bool DoEvent()
		{
			campaign.DecrementDays(1);
			program.QueueTask(new InfoTask
			{
				GraphicName = "DriverUpdate",
				InfoText = "Your drivers need updating. Lose one day."
			});
			return true;
		}
	}
}
