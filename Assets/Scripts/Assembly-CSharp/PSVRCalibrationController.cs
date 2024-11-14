using UnityEngine;

public class PSVRCalibrationController : MonoBehaviour
{
	public enum PSVRBuildType
	{
		DemoDisc = 0,
		ConferenceDemo = 1,
		FullGame = 2
	}

	public static PSVRBuildType CurrentBuildType { get; set; }
}
