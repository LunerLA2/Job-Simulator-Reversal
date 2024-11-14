namespace SiliconTrail
{
	public class TorrentEvent : CampaignEvent
	{
		private const int SURFING_COST = 4;

		protected override bool DoEvent()
		{
			program.QueueTask(new InfoTask
			{
				GraphicName = "Torrent",
				InfoText = "You come upon a torrent of memory."
			});
			program.QueueTask(new ChoiceTask
			{
				GraphicName = "Torrent",
				InfoText = "Would you like to go surfing for " + 4 + " " + program.DataUnit + " of memory?",
				DefaultText = "It's a deal",
				DefaultAction = Surf,
				AlternateText = "No thanks",
				AlternateAction = CarryOn
			});
			return false;
		}

		private void Surf()
		{
			if (campaign.Money >= 4)
			{
				campaign.DecrementMoney(4);
				program.QueueTask(new InfoTask
				{
					GraphicName = "Torrent",
					InfoText = "Watch out for <color=red>corrupted data</color>!"
				});
				SurfingTask surfingTask = new SurfingTask();
				surfingTask.TaskFinished += OnSurfingFinished;
				program.QueueTask(surfingTask);
			}
			else
			{
				program.QueueTask(new InfoTask
				{
					GraphicName = "Torrent",
					InfoText = "You don't have enough memory! Scram!"
				});
				isFinished = true;
			}
		}

		private void CarryOn()
		{
			isFinished = true;
		}

		private void OnSurfingFinished(Task task)
		{
			task.TaskFinished -= OnSurfingFinished;
			isFinished = true;
		}
	}
}
