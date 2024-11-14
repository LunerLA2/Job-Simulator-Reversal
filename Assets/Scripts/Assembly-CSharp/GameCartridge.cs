using UnityEngine;

public class GameCartridge : MonoBehaviour
{
	protected TerminalManager terminalReference;

	public virtual void SetTerminalReference(TerminalManager terminal)
	{
		terminalReference = terminal;
	}

	public virtual JobCartridgeWithGenieFlags GetJobCartridgeWithGenieFlags(JobGenieCartridge.GenieModeTypes types = JobGenieCartridge.GenieModeTypes.None)
	{
		return null;
	}
}
