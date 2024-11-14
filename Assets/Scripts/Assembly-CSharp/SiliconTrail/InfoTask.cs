using System.Collections;

namespace SiliconTrail
{
	public class InfoTask : Task
	{
		public string GraphicName;

		public string InfoText;

		private bool dismissed;

		protected override IEnumerator Execute()
		{
			program.HideAll();
			program.ShowGraphic(GraphicName);
			program.ShowInfo(InfoText);
			program.ShowChoice("OK");
			while (!dismissed)
			{
				yield return null;
			}
		}

		public override void OnKeyPress(string code)
		{
			if (code == "1")
			{
				dismissed = true;
			}
		}
	}
}
