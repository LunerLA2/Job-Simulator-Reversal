using OwlchemyVR;
using UnityEngine;

public class TestHapticsOculus : MonoBehaviour
{
	[SerializeField]
	private float frequency = 1f;

	[SerializeField]
	private float amplitude = 1f;

	private bool hasBeenInit;

	private InteractionHandController[] handControllers;

	private InteractionHandController currHandController;

	private void Start()
	{
		handControllers = Object.FindObjectsOfType<InteractionHandController>();
	}

	private void Update()
	{
		if (!hasBeenInit)
		{
			for (int i = 0; i < handControllers.Length; i++)
			{
				if (handControllers[i].IsGrabInputButtonDown())
				{
					currHandController = handControllers[i];
					hasBeenInit = true;
					break;
				}
			}
		}
		else
		{
			OVRInput.SetControllerVibration(frequency, amplitude, currHandController.HandController.OculusTouchController.ControllerType);
		}
	}
}
