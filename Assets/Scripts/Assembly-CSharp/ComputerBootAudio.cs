using System;
using System.Collections;
using UnityEngine;

public class ComputerBootAudio : MonoBehaviour
{
	[SerializeField]
	private PoweredComputerHardwareController pchc;

	[SerializeField]
	private AudioSourceHelper audioHelper;

	[SerializeField]
	private AudioClip bootUpSound;

	[SerializeField]
	private AudioClip idleSound;

	private void OnEnable()
	{
		PoweredComputerHardwareController poweredComputerHardwareController = pchc;
		poweredComputerHardwareController.OnWasTurnedOn = (Action)Delegate.Combine(poweredComputerHardwareController.OnWasTurnedOn, new Action(TurnedOn));
		PoweredComputerHardwareController poweredComputerHardwareController2 = pchc;
		poweredComputerHardwareController2.OnWasTurnedOff = (Action)Delegate.Combine(poweredComputerHardwareController2.OnWasTurnedOff, new Action(TurnedOff));
	}

	private void OnDisable()
	{
		PoweredComputerHardwareController poweredComputerHardwareController = pchc;
		poweredComputerHardwareController.OnWasTurnedOn = (Action)Delegate.Remove(poweredComputerHardwareController.OnWasTurnedOn, new Action(TurnedOn));
		PoweredComputerHardwareController poweredComputerHardwareController2 = pchc;
		poweredComputerHardwareController2.OnWasTurnedOff = (Action)Delegate.Remove(poweredComputerHardwareController2.OnWasTurnedOff, new Action(TurnedOff));
	}

	private void TurnedOn()
	{
		StartCoroutine(PowerOnAudioSequence());
	}

	private IEnumerator PowerOnAudioSequence()
	{
		if (audioHelper != null)
		{
			audioHelper.SetClip(bootUpSound);
			audioHelper.Play();
			yield return new WaitForSeconds(bootUpSound.length);
			audioHelper.SetLooping(true);
			audioHelper.SetClip(idleSound);
			audioHelper.Play();
		}
	}

	private void TurnedOff()
	{
		if (audioHelper != null)
		{
			audioHelper.Stop();
		}
	}
}
