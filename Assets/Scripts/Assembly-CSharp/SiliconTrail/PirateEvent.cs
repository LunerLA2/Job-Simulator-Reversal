using UnityEngine;

namespace SiliconTrail
{
	public class PirateEvent : CampaignEvent
	{
		private const float FIGHT_WIN_CHANCE = 0.5f;

		private const int MIN_FIGHT_DAMAGE = 0;

		private const int MAX_FIGHT_DAMAGE = 512;

		private const int MIN_ROB_AMOUNT = 0;

		private const int MAX_ROB_AMOUNT = 16;

		protected override bool DoEvent()
		{
			program.QueueTask(new ChoiceTask
			{
				GraphicName = "Pirate",
				InfoText = "You encounter a data pirate! What will you do?",
				DefaultText = "Fight the pirate",
				AlternateText = "Let the pirate rob you",
				DefaultAction = FightPirate,
				AlternateAction = LetPirateRob
			});
			return false;
		}

		private void FightPirate()
		{
			if (Random.Range(0f, 1f) < 0.5f)
			{
				program.QueueTask(new InfoTask
				{
					GraphicName = "Pirate",
					InfoText = "You fended off the pirate."
				});
			}
			else
			{
				program.QueueTask(new InfoTask
				{
					GraphicName = "Pirate",
					InfoText = "The pirate won the fight."
				});
				LetPirateRob();
			}
			int num = Random.Range(0, 513);
			if (num > 0)
			{
				PartyMember partyMember = campaign.SelectRandomPartyMember();
				partyMember.Health -= num;
				if (partyMember.IsAlive)
				{
					program.QueueTask(new InfoTask
					{
						GraphicName = "Pirate",
						InfoText = partyMember.Name + " was injured."
					});
				}
			}
			isFinished = true;
		}

		private void LetPirateRob()
		{
			int num = Mathf.Min(campaign.Money, Random.Range(0, 17));
			campaign.DecrementMoney(num);
			program.QueueTask(new InfoTask
			{
				GraphicName = "Pirate",
				InfoText = "The pirate took " + ((num <= 0) ? "nothing" : (num + " " + program.DataUnit + " of memory")) + " from you."
			});
			isFinished = true;
		}
	}
}
