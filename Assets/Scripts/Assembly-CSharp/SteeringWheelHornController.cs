using UnityEngine;

public class SteeringWheelHornController : MonoBehaviour
{
	private bool wasHands;

	public void PlayHornSound(AudioClip buttonPressedAudioClip)
	{
		if (buttonPressedAudioClip != null && wasHands)
		{
			AudioManager.Instance.Play(base.transform.position, buttonPressedAudioClip, 1f, 1f);
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		if (col.collider.gameObject.layer == 10 || col.collider.gameObject.layer == 10)
		{
			wasHands = true;
		}
		else
		{
			wasHands = false;
		}
	}

	private void OnCollisionExit(Collision col)
	{
		wasHands = false;
	}
}
