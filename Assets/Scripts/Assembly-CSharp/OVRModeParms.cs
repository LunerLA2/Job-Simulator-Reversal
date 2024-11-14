using UnityEngine;

public class OVRModeParms : MonoBehaviour
{
	public OVRInput.RawButton resetButton = OVRInput.RawButton.X;

	private void Start()
	{
		if (!OVRManager.isHmdPresent)
		{
			base.enabled = false;
		}
		else
		{
			InvokeRepeating("TestPowerStateMode", 10f, 10f);
		}
	}

	private void Update()
	{
		if (OVRInput.GetDown(resetButton))
		{
			OVRPlugin.cpuLevel = 0;
			OVRPlugin.gpuLevel = 1;
		}
	}

	private void TestPowerStateMode()
	{
		if (OVRPlugin.powerSaving)
		{
			Debug.Log("POWER SAVE MODE ACTIVATED");
		}
	}
}
