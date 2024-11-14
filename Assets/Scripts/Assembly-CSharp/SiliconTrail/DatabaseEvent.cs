using System;
using UnityEngine;

namespace SiliconTrail
{
	public class DatabaseEvent : CampaignEvent
	{
		private const float BUY_CHANCE = 1f;

		private const int BUY_MONEY_AMOUNT = 8;

		private const int BUY_FOOD_AMOUNT = 8;

		private const int SELL_MONEY_AMOUNT = 8;

		private const int SELL_FOOD_AMOUNT = 8;

		protected override bool DoEvent()
		{
			bool flag = UnityEngine.Random.Range(0f, 1f) < 1f;
			string text = string.Format("{0} {1} of food\nfor {2} of memory", (!flag) ? "sell" : "buy", ((!flag) ? 8 : 8) + " " + program.DataUnit, ((!flag) ? 8 : 8) + " " + program.DataUnit);
			program.QueueTask(new ChoiceTask
			{
				GraphicName = "Database",
				InfoText = "You found a database. Would you like to:\n" + text + "?",
				DefaultText = "It's a deal",
				DefaultAction = ((!flag) ? new Action(Sell) : new Action(Buy)),
				AlternateText = "No thanks",
				AlternateAction = CarryOn
			});
			return false;
		}

		private void Buy()
		{
			if (campaign.Money >= 8)
			{
				campaign.DecrementMoney(8);
				campaign.IncrementFood(8);
				program.QueueTask(new InfoTask
				{
					GraphicName = "Database",
					InfoText = "Thanks for your business."
				});
			}
			else
			{
				program.QueueTask(new InfoTask
				{
					GraphicName = "Database",
					InfoText = "You don't have enough memory! Scram!"
				});
			}
			isFinished = true;
		}

		private void Sell()
		{
			if (campaign.Food >= 8)
			{
				campaign.IncrementMoney(8);
				campaign.DecrementFood(8);
				program.QueueTask(new InfoTask
				{
					GraphicName = "Database",
					InfoText = "Thanks for your business."
				});
			}
			else
			{
				program.QueueTask(new InfoTask
				{
					GraphicName = "Database",
					InfoText = "You don't have enough food! Scram!"
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
