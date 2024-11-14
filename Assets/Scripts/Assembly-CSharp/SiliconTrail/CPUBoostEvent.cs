namespace SiliconTrail
{
	public class CPUBoostEvent : CampaignEvent
	{
		private const int BOOST_DISTANCE = 3;

		protected override bool DoEvent()
		{
			program.QueueTask(new InfoTask
			{
				GraphicName = "CPUBoost",
				InfoText = "You received a CPU Boost! Travel " + 3 + " spaces!"
			});
			tasksOnFinish.Clear();
			program.QueueTask(new TravelTask
			{
				Distance = 3
			});
			return true;
		}
	}
}
