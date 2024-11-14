namespace SiliconTrail
{
	public class Place : WeightedProbableThing
	{
		public string Name = string.Empty;

		public CampaignEvent[] PossibleEvents = new CampaignEvent[1]
		{
			new UneventfulEvent()
		};

		public CampaignEvent SelectRandomEvent()
		{
			return PossibleEvents.SelectRandom();
		}
	}
}
