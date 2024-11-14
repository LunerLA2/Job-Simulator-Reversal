using UnityEngine;

public class OVRChromaticAberration : MonoBehaviour
{
	public OVRInput.RawButton toggleButton = OVRInput.RawButton.X;

	private bool chromatic;

	private void Start()
	{
		OVRManager.instance.chromatic = chromatic;
	}

	private void Update()
	{
		if (OVRInput.GetDown(toggleButton))
		{
			chromatic = !chromatic;
			OVRManager.instance.chromatic = chromatic;
		}
	}
}
