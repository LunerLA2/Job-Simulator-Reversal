using System;
using UnityEngine;

public class OfficeComputerScreenController : MonoBehaviour
{
	[SerializeField]
	private Animation bootAnimation;

	[SerializeField]
	private AudioClip bootSound;

	[SerializeField]
	private Transform soundLocation;

	[SerializeField]
	private GameObject lightOn;

	[SerializeField]
	private GameObject lightOff;

	[SerializeField]
	private AttachableObject plugAttachable;

	private bool computerIsOn;

	private bool computerIsPluggedIn;

	public Action OnComputerBooted;

	public bool ComputerIsOn
	{
		get
		{
			return computerIsOn;
		}
	}

	private void OnEnable()
	{
		AttachableObject attachableObject = plugAttachable;
		attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(ComputerPluggedIn));
		AttachableObject attachableObject2 = plugAttachable;
		attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(ComputerUnplugged));
	}

	private void OnDisable()
	{
		AttachableObject attachableObject = plugAttachable;
		attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(ComputerPluggedIn));
		AttachableObject attachableObject2 = plugAttachable;
		attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(ComputerUnplugged));
	}

	public void Boot()
	{
		if (!computerIsOn && computerIsPluggedIn)
		{
			AudioManager.Instance.Play(soundLocation, bootSound, 1f, 1f);
			bootAnimation.gameObject.SetActive(true);
			bootAnimation.Play();
			if (OnComputerBooted != null)
			{
				OnComputerBooted();
			}
			computerIsOn = true;
		}
	}

	private void ComputerPluggedIn(AttachableObject o, AttachablePoint t)
	{
		SetStateOfPowerLight(true);
		computerIsPluggedIn = true;
	}

	private void ComputerUnplugged(AttachableObject o, AttachablePoint t)
	{
		if (computerIsOn)
		{
			bootAnimation.gameObject.SetActive(false);
			computerIsOn = false;
		}
		computerIsPluggedIn = false;
		SetStateOfPowerLight(false);
	}

	private void SetStateOfPowerLight(bool state)
	{
		lightOn.SetActive(state);
		lightOff.SetActive(!state);
	}
}
