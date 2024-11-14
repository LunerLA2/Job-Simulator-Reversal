using System;
using OwlchemyVR;
using UnityEngine;

public class ComputerTowerController : PoweredComputerHardwareController
{
	private enum CDTrayState
	{
		Closed = 0,
		Opening = 1,
		Open = 2,
		Closing = 3
	}

	[SerializeField]
	private Rigidbody cdTrayRigidbody;

	[SerializeField]
	private AttachablePoint cdTrayAttachPoint;

	[SerializeField]
	private Vector3 fullOpenTrayPosition = new Vector3(0.21f, 0f, 0f);

	[SerializeField]
	private Vector3 closedTrayPosition = Vector3.zero;

	[SerializeField]
	private float openSpeed = 5f;

	[SerializeField]
	private float closeSpeed = 5f;

	[SerializeField]
	private float openToCloseThreshold = 0.8f;

	[SerializeField]
	private AudioSourceHelper trayAudio;

	[SerializeField]
	private AudioClip trayEjectSound;

	[SerializeField]
	private AudioClip trayClosingSound;

	[SerializeField]
	private AudioClip trayClosedSound;

	private CDTrayState cdTrayState;

	private ComputerCD trayCD;

	private float openness;

	public Action<ComputerCD> OnCDWasInserted;

	public Action<ComputerCD> OnCDWasEjected;

	public ComputerCD TrayCD
	{
		get
		{
			return trayCD;
		}
	}

	public bool IsCDInserted
	{
		get
		{
			return trayCD != null && cdTrayState == CDTrayState.Closed;
		}
	}

	public override void Awake()
	{
		base.Awake();
		LockCDTrayInteractions();
	}

	public override void OnEnable()
	{
		base.OnEnable();
		cdTrayAttachPoint.OnObjectWasAttached += CDWasPlaced;
		cdTrayAttachPoint.OnObjectWasDetached += CDWasRemoved;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		cdTrayAttachPoint.OnObjectWasAttached -= CDWasPlaced;
		cdTrayAttachPoint.OnObjectWasDetached -= CDWasRemoved;
	}

	protected override void TurnedPowerOn()
	{
		base.TurnedPowerOn();
		CloseTray();
	}

	private void CDWasPlaced(AttachablePoint point, AttachableObject cd)
	{
		trayCD = cd.GetComponent<ComputerCD>();
	}

	private void CDWasRemoved(AttachablePoint point, AttachableObject cd)
	{
		trayCD = null;
	}

	public void CDTrayButtonPressed()
	{
		if (isPoweredOn)
		{
			if (cdTrayState == CDTrayState.Closed)
			{
				OpenTray();
			}
			else if (cdTrayState == CDTrayState.Open)
			{
				CloseTray();
			}
		}
		else
		{
			DoNotPluggedInEffect();
		}
	}

	public void OpenTray()
	{
		if (cdTrayState != CDTrayState.Open && cdTrayState != CDTrayState.Opening)
		{
			if (trayEjectSound != null)
			{
				AudioManager.Instance.Play(trayAudio.transform, trayEjectSound, 1f, 1f);
			}
			cdTrayState = CDTrayState.Opening;
			if (trayCD != null && OnCDWasEjected != null)
			{
				OnCDWasEjected(trayCD);
			}
		}
	}

	public void CloseTray()
	{
		if (cdTrayState != 0 && cdTrayState != CDTrayState.Closing)
		{
			trayAudio.SetLooping(true);
			trayAudio.SetClip(trayClosingSound);
			trayAudio.Play();
			cdTrayState = CDTrayState.Closing;
			LockCDTrayInteractions();
		}
	}

	private void Update()
	{
		if (cdTrayState == CDTrayState.Opening)
		{
			openness = Mathf.Clamp01(openness + Time.deltaTime * openSpeed);
			cdTrayRigidbody.transform.localPosition = Vector3.Lerp(closedTrayPosition, fullOpenTrayPosition, openness);
			if (openness >= 1f)
			{
				cdTrayState = CDTrayState.Open;
				UnlockCDTrayInteractions();
			}
		}
		else if (cdTrayState == CDTrayState.Closing)
		{
			openness = Mathf.Clamp01(openness - Time.deltaTime * closeSpeed);
			cdTrayRigidbody.transform.localPosition = Vector3.Lerp(closedTrayPosition, fullOpenTrayPosition, openness);
			if (openness <= 0f)
			{
				cdTrayState = CDTrayState.Closed;
				trayAudio.Stop();
				AudioManager.Instance.Play(trayAudio.transform, trayClosedSound, 1f, 1f);
				if (trayCD != null && OnCDWasInserted != null)
				{
					OnCDWasInserted(trayCD);
				}
			}
		}
		else if (cdTrayState == CDTrayState.Open)
		{
			openness = (cdTrayRigidbody.transform.localPosition - closedTrayPosition).magnitude / (fullOpenTrayPosition - closedTrayPosition).magnitude;
			if (openness <= openToCloseThreshold)
			{
				CloseTray();
			}
		}
	}

	private void LockCDTrayInteractions()
	{
		cdTrayRigidbody.isKinematic = true;
		if (trayCD != null)
		{
			trayCD.GetComponent<GrabbableItem>().enabled = false;
		}
		else
		{
			cdTrayAttachPoint.gameObject.SetActive(false);
		}
	}

	private void UnlockCDTrayInteractions()
	{
		cdTrayRigidbody.isKinematic = false;
		if (trayCD != null)
		{
			trayCD.GetComponent<GrabbableItem>().enabled = true;
		}
		if (!cdTrayAttachPoint.gameObject.activeSelf)
		{
			cdTrayAttachPoint.gameObject.SetActive(true);
		}
	}
}
