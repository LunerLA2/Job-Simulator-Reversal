namespace SiliconTrail
{
	public class EndingEvent : CampaignEvent
	{
		public EndingCondition EndingCondition;

		protected override bool DoEvent()
		{
			GameEventsManager.Instance.ItemActionOccurred(program.WorldItemData, "ACTIVATED");
			string graphicName = string.Empty;
			string infoText = string.Empty;
			if (EndingCondition == EndingCondition.Win)
			{
				program.QueueTask(new CinematicTask
				{
					CinematicName = "Win"
				});
				string text = "You";
				PartyMember[] livingPartyMembers = campaign.LivingPartyMembers;
				for (int i = 0; i < livingPartyMembers.Length; i++)
				{
					PartyMember partyMember = livingPartyMembers[i];
					text += ((i != livingPartyMembers.Length - 1) ? ", " : " and ");
					text += partyMember.Name;
				}
				graphicName = "Internet";
				infoText = text + " reached the Internet before the reformatting!";
			}
			else if (EndingCondition == EndingCondition.AllDead)
			{
				program.QueueTask(new CinematicTask
				{
					CinematicName = "AllDead"
				});
				graphicName = "Death";
				infoText = "You failed! All your party members are dead!";
			}
			else if (EndingCondition == EndingCondition.OutOfTime)
			{
				program.QueueTask(new CinematicTask
				{
					CinematicName = "TooLate"
				});
				graphicName = "Reformat";
				infoText = "You were too late! The world was reformatted!";
			}
			program.QueueTask(new ChoiceTask
			{
				GraphicName = graphicName,
				InfoText = infoText,
				DefaultText = "Play again",
				DefaultAction = PlayAgain,
				AlternateText = "Quit",
				AlternateAction = Quit
			});
			return false;
		}

		private void PlayAgain()
		{
			program.Restart();
		}

		private void Quit()
		{
			program.Finish();
		}
	}
}
