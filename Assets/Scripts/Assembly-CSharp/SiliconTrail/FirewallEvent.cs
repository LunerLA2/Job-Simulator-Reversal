using UnityEngine;

namespace SiliconTrail
{
	public class FirewallEvent : CampaignEvent
	{
		private const float MIN_INITIAL_FIREWALL_INTEGRITY = 0.4f;

		private const float MAX_INITIAL_FIREWALL_INTEGRITY = 1f;

		private const float MIN_FIREWALL_INTEGRITY_CHANGE = -0.75f;

		private const float MAX_FIREWALL_INTEGRITY_CHANGE = -0.1f;

		private const int FIREWALL_DAMAGE = 128;

		private float firewallIntegrity;

		protected override bool DoEvent()
		{
			firewallIntegrity = Random.Range(0.4f, 1f);
			program.QueueTask(new InfoTask
			{
				GraphicName = "Firewall",
				InfoText = "You encounter a firewall."
			});
			program.QueueTask(MakePromptTask());
			return false;
		}

		private ChoiceTask MakePromptTask()
		{
			ChoiceTask choiceTask = new ChoiceTask();
			choiceTask.GraphicName = "Firewall";
			choiceTask.InfoText = "Firewall integrity: " + Mathf.RoundToInt(firewallIntegrity * 100f) + "%\nWhat will you do?";
			choiceTask.DefaultText = "Attempt to jump through it";
			choiceTask.DefaultAction = JumpThrough;
			choiceTask.AlternateText = "Wait a day to see what happens";
			choiceTask.AlternateAction = WaitOneDay;
			return choiceTask;
		}

		private void JumpThrough()
		{
			if (Random.Range(0f, 1f) > firewallIntegrity)
			{
				tasksOnFinish.Clear();
				program.QueueTask(new InfoTask
				{
					GraphicName = "Firewall",
					InfoText = "You made it through!"
				});
				program.QueueTask(new TravelTask());
				isFinished = true;
				return;
			}
			PartyMember[] livingPartyMembers = campaign.LivingPartyMembers;
			foreach (PartyMember partyMember in livingPartyMembers)
			{
				partyMember.DecrementHealth(128);
			}
			program.QueueTask(new InfoTask
			{
				GraphicName = "Firewall",
				InfoText = "You failed to make it through and got burnt!"
			});
			program.QueueTask(MakePromptTask());
		}

		private void WaitOneDay()
		{
			program.QueueTask(new ResolveDayTask());
			firewallIntegrity = Mathf.Clamp01(firewallIntegrity + Random.Range(-0.75f, -0.1f));
			program.QueueTask(MakePromptTask());
		}
	}
}
