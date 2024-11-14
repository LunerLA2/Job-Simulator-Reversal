using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class PoweredComputerHardwareController : MonoBehaviour
{
	[SerializeField]
	private GameObject lightOff;

	[SerializeField]
	private GameObject lightReady;

	[SerializeField]
	private GameObject lightOn;

	[SerializeField]
	private bool startOn;

	[SerializeField]
	private AttachableObject attachablePlug;

	[SerializeField]
	private Animation notPluggedInAnimation;

	[SerializeField]
	private AudioClip notPluggedInSound;

	[SerializeField]
	private ParticleSystem notPluggedInParticle;

	[SerializeField]
	private ComputerController notifyComputerAboutNotPluggedAnimation;

	protected bool isPluggedIn;

	protected bool isPoweredOn;

	public Action OnWasTurnedOn;

	public Action OnWasTurnedOff;

	private WorldItem worldItem;

	private float notConnectedSoundThreshold = 0.1f;

	private bool canPlayNotConnectedSounds;

	public virtual void Awake()
	{
		worldItem = GetComponent<WorldItem>();
		SetLightState(-1);
	}

	public virtual void OnEnable()
	{
		if (attachablePlug != null)
		{
			AttachableObject attachableObject = attachablePlug;
			attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(PluggedIn));
			AttachableObject attachableObject2 = attachablePlug;
			attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(Unplugged));
		}
	}

	public virtual void OnDisable()
	{
		if (attachablePlug != null)
		{
			AttachableObject attachableObject = attachablePlug;
			attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(PluggedIn));
			AttachableObject attachableObject2 = attachablePlug;
			attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(Unplugged));
		}
	}

	private void Start()
	{
		if (startOn)
		{
			isPluggedIn = true;
			TurnedPowerOn();
		}
	}

	private IEnumerator WaitAndAllowNotConnectedSounds()
	{
		yield return new WaitForSeconds(notConnectedSoundThreshold);
		canPlayNotConnectedSounds = true;
	}

	protected virtual void PluggedIn(AttachableObject o, AttachablePoint p)
	{
		isPluggedIn = true;
		SetLightState(0);
	}

	protected virtual void Unplugged(AttachableObject o, AttachablePoint p)
	{
		isPluggedIn = false;
		SetLightState(-1);
		if (isPoweredOn)
		{
			isPoweredOn = false;
			TurnedOffEvent();
			DoNotPluggedInEffect();
		}
	}

	protected virtual void TurnedPowerOn()
	{
		isPoweredOn = true;
		SetLightState(1);
		TurnedOnEvent();
	}

	protected virtual void TurnedPowerOff()
	{
		isPoweredOn = false;
		SetLightState(0);
		TurnedOffEvent();
	}

	public void PowerButtonPressed()
	{
		if (isPluggedIn)
		{
			if (!isPoweredOn)
			{
				TurnedPowerOn();
			}
			else
			{
				TurnedPowerOff();
			}
		}
		else
		{
			DoNotPluggedInEffect();
		}
	}

	public void ForceTurnOffNoEvent()
	{
		isPoweredOn = false;
		if (isPluggedIn)
		{
			SetLightState(0);
		}
		else
		{
			SetLightState(-1);
		}
	}

	private void SetLightState(int s)
	{
		lightOff.SetActive(s == -1);
		lightReady.SetActive(s == 0);
		lightOn.SetActive(s == 1);
	}

	protected void DoNotPluggedInEffect()
	{
		if (!canPlayNotConnectedSounds)
		{
			return;
		}
		if (notPluggedInAnimation != null)
		{
			notPluggedInAnimation.Play();
			if (notifyComputerAboutNotPluggedAnimation != null)
			{
				notifyComputerAboutNotPluggedAnimation.EnsureRenderingForTime(notPluggedInAnimation.clip.length + 0.1f);
			}
		}
		if (notPluggedInParticle != null)
		{
			notPluggedInParticle.Play();
		}
		if (notPluggedInSound != null)
		{
			AudioManager.Instance.Play(base.transform.position, notPluggedInSound, 1f, 1f);
		}
	}

	private void TurnedOnEvent()
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "ACTIVATED");
		if (OnWasTurnedOn != null)
		{
			OnWasTurnedOn();
		}
	}

	private void TurnedOffEvent()
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "DEACTIVATED");
		if (OnWasTurnedOff != null)
		{
			OnWasTurnedOff();
		}
	}
}
