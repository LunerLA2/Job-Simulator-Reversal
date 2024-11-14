using UnityEngine;

public class ShowRecenterAttemptError : MonoBehaviour
{
	[SerializeField]
	private GameObject recenterErrorMessageObject;

	[SerializeField]
	private float secsToDisplay;

	private bool hasShownMessage;

	private void Update()
	{
		if (OVRPlugin.shouldRecenter && !hasShownMessage)
		{
			OnRecenterAttempt();
		}
	}

	private void OnRecenterAttempt()
	{
		recenterErrorMessageObject.SetActive(true);
		TimeManager.Invoke(TurnOffMessage, secsToDisplay);
		hasShownMessage = true;
	}

	private void TurnOffMessage()
	{
		recenterErrorMessageObject.SetActive(false);
	}
}
