using OwlchemyVR;
using UnityEngine;

public class GameCodeComputerProgram : WordComputerProgram
{
	[SerializeField]
	private WorldItemData worldItemToUseOnCommit;

	private bool wasCommitReady;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.GameCode;
		}
	}

	public override void Update()
	{
		bool flag = (selectedTemplateIndex == 0 && numRevealedChars >= 10) || (numRevealedChars >= templates[selectedTemplateIndex].Content.Length && numRevealedChars > 0);
		if (flag != wasCommitReady)
		{
			if (flag)
			{
				GameEventsManager.Instance.ItemActionOccurred(worldItemToUseOnCommit, "ACTIVATED");
			}
			else
			{
				GameEventsManager.Instance.ItemActionOccurred(worldItemToUseOnCommit, "DEACTIVATED");
			}
			wasCommitReady = flag;
		}
		printButton.SetInteractive(flag);
	}

	protected override void OnClickableClicked(ComputerClickable clickable)
	{
		bool flag = false;
		if (clickable != null && clickable == printButton)
		{
			flag = true;
			GameEventsManager.Instance.ItemActionOccurred(worldItemToUseOnCommit, "USED");
		}
		if (!flag)
		{
			base.OnClickableClicked(clickable);
		}
	}
}
