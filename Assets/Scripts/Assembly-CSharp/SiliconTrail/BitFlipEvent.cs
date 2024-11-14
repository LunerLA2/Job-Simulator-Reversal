namespace SiliconTrail
{
	public class BitFlipEvent : CampaignEvent
	{
		protected override bool DoEvent()
		{
			int food = campaign.Food;
			int money = campaign.Money;
			campaign.SetFood(money);
			campaign.SetMoney(food);
			program.QueueTask(new InfoTask
			{
				GraphicName = "BitFlip",
				InfoText = "Your data has been bit-flipped! Food becomes memory and memory becomes food!"
			});
			return true;
		}
	}
}
