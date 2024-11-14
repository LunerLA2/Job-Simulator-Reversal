using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SiliconTrail
{
	public class PlaceTask : Task
	{
		private class RandomFind : WeightedProbableThing
		{
			public Action OnFind;
		}

		private const int MONEY_FIND_AMOUNT = 2;

		private const int FOOD_FIND_AMOUNT = 4;

		private RandomFind[] possibleRandomFinds;

		private List<string> waysOfSayingNothing = new List<string> { "nothing", "the courage to go on", "hope", "sadness and despair", "a nice view", "inner peace", "a new outlook on life", "the strength to forgive your past enemies", "a whole lot of nothing", "your calling" };

		public PlaceTask()
		{
			possibleRandomFinds = new RandomFind[3]
			{
				new RandomFind
				{
					OnFind = FindNothing,
					ProbabilityWeight = 2f
				},
				new RandomFind
				{
					OnFind = FindFood,
					ProbabilityWeight = 1f
				},
				new RandomFind
				{
					OnFind = FindMoney,
					ProbabilityWeight = 1f
				}
			};
		}

		protected override IEnumerator Execute()
		{
			CampaignEvent ev = campaign.CurrentPlace.SelectRandomEvent();
			if (ev != null)
			{
				ev.DoEvent(program, MakePromptTask(true));
			}
			else
			{
				program.QueueTask(MakePromptTask(true));
			}
			yield break;
		}

		private ChoiceTask MakePromptTask(bool canLook)
		{
			ChoiceTask choiceTask = new ChoiceTask();
			choiceTask.ShowTrail = true;
			choiceTask.DefaultText = "Travel for a day";
			choiceTask.DefaultAction = KeepMoving;
			choiceTask.AlternateText = ((!canLook) ? null : "Look around");
			choiceTask.AlternateAction = ((!canLook) ? null : new Action(LookAround));
			return choiceTask;
		}

		private void KeepMoving()
		{
			program.QueueTask(new TravelTask());
		}

		private void LookAround()
		{
			RandomFind randomFind = possibleRandomFinds.SelectRandom();
			randomFind.OnFind();
			program.QueueTask(MakePromptTask(false));
		}

		private void FindNothing()
		{
			string text = waysOfSayingNothing[UnityEngine.Random.Range(0, waysOfSayingNothing.Count - 2)];
			waysOfSayingNothing.Remove(text);
			waysOfSayingNothing.Add(text);
			program.QueueTask(new InfoTask
			{
				InfoText = "You found " + text + "."
			});
		}

		private void FindFood()
		{
			campaign.IncrementFood(4);
			program.QueueTask(new InfoTask
			{
				InfoText = "You found some food!"
			});
		}

		private void FindMoney()
		{
			campaign.IncrementMoney(2);
			program.QueueTask(new InfoTask
			{
				InfoText = "You found some memory!"
			});
		}
	}
}
