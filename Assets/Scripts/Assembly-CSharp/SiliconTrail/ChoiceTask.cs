using System;
using System.Collections;

namespace SiliconTrail
{
	public class ChoiceTask : Task
	{
		public string GraphicName;

		public string InfoText;

		public string DefaultText;

		public string AlternateText;

		public Action DefaultAction;

		public Action AlternateAction;

		public bool ShowTrail;

		private bool choiceMade;

		protected override IEnumerator Execute()
		{
			program.HideAll();
			if (ShowTrail)
			{
				program.ShowTrail();
			}
			else
			{
				program.ShowGraphic(GraphicName);
				program.ShowInfo(InfoText);
			}
			program.ShowChoice(DefaultText, AlternateText);
			while (!choiceMade)
			{
				yield return null;
			}
		}

		public override void OnKeyPress(string code)
		{
			GameEventsManager.Instance.ItemActionOccurred(program.WorldItemData, "USED");
			program.UsedWID();
			if (code == "1")
			{
				if (DefaultAction != null)
				{
					DefaultAction();
					choiceMade = true;
				}
			}
			else if (code == "0" && AlternateAction != null)
			{
				AlternateAction();
				choiceMade = true;
			}
		}
	}
}
