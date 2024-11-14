using UnityEngine;

public class RecalibrateButtonMuseum : MonoBehaviour
{
	private bool wasPressed;

	[SerializeField]
	private TerminalManager terminalManager;

	public void GoToCalibrationScene()
	{
		if ((!(terminalManager != null) || !terminalManager.IsLoadingIntoJob) && !wasPressed)
		{
			wasPressed = true;
			Debug.Log("Go to recalibration scene pressed");
		}
	}
}
