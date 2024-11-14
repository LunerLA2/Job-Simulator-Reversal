using UnityEngine;

public class OVRResetOrientation : MonoBehaviour
{
	public OVRInput.RawButton resetButton = OVRInput.RawButton.Y;

	private void Update()
	{
		if (OVRInput.GetDown(resetButton))
		{
			OVRManager.display.RecenterPose();
		}
	}
}
