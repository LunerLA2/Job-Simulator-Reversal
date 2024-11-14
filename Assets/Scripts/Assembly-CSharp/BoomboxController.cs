using System.Collections;
using OwlchemyVR;
using OwlchemyVR2;
using UnityEngine;

public class BoomboxController : MonoBehaviour
{
	public const float MAX_DISC_SPIN_SPEED = 1000f;

	public const float VOLUME_MAX = 0.9f;

	public const float VOLUME_MIN = 0.2f;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private BoomboxMusicDefinition[] musicDefinitions;

	[SerializeField]
	private AudioClip undefinedMusicSound;

	[SerializeField]
	private AudioSourceHelper[] musicSources;

	[SerializeField]
	private PushableButton playbutton;

	[SerializeField]
	private PushableButton ejectbutton;

	[SerializeField]
	private Material powerLightOff;

	[SerializeField]
	private Material powerLightOn;

	[SerializeField]
	private AudioClip cdTrayCloseSound;

	[SerializeField]
	private GrabbableHinge trayHinge;

	[SerializeField]
	private GrabbableItem trayGrabbable;

	[SerializeField]
	private OwlchemyVR2.GrabbableHinge volumeKnob;

	[SerializeField]
	private Transform cdRotateTransform;

	[SerializeField]
	private Collider attachPointCollider;

	[SerializeField]
	private AttachablePoint cdAttachPoint;

	private bool isPoweredOn = true;

	private bool isTrayOpen;

	private bool isMusicPlaying;

	private float discSpinSpeed;

	private bool isTrayAnimating;

	private void Awake()
	{
		TrayClosed(trayHinge, true);
		SetPowerState(true);
	}

	private void Start()
	{
		volumeKnob.SetNormalizedAmount(0.3f);
		RefreshVolume();
	}

	private void SetPowerState(bool state)
	{
		isPoweredOn = state;
		RefreshMusicClip();
		StopMusic();
		if (isPoweredOn)
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
		}
		else
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
		}
	}

	private void OnEnable()
	{
		trayHinge.OnLowerLocked += TrayClosed;
		cdAttachPoint.OnObjectWasAttached += CDAttached;
		cdAttachPoint.OnObjectWasDetached += CDDetached;
		volumeKnob.SetNormalizedAmount(0.3f);
		RefreshVolume();
		playbutton.OnButtonPushed += Playbutton_OnButtonPushed;
		ejectbutton.OnButtonPushed += Ejectbutton_OnButtonPushed;
	}

	private void OnDisable()
	{
		trayHinge.OnLowerLocked -= TrayClosed;
		cdAttachPoint.OnObjectWasAttached -= CDAttached;
		cdAttachPoint.OnObjectWasDetached -= CDDetached;
		playbutton.OnButtonPushed -= Playbutton_OnButtonPushed;
		ejectbutton.OnButtonPushed -= Ejectbutton_OnButtonPushed;
	}

	private void Update()
	{
		if (isMusicPlaying && isPoweredOn)
		{
			discSpinSpeed = Mathf.Lerp(discSpinSpeed, 1000f, Time.deltaTime * 2.5f);
		}
		else
		{
			discSpinSpeed = Mathf.Lerp(discSpinSpeed, 0f, Time.deltaTime * 2.5f);
		}
		cdRotateTransform.Rotate(Vector3.forward * discSpinSpeed * Time.deltaTime);
		if (volumeKnob.Grabbable.IsCurrInHand)
		{
			RefreshVolume();
		}
	}

	private void RefreshVolume()
	{
		float normalizedAxisValue = volumeKnob.NormalizedAxisValue;
		for (int i = 0; i < musicSources.Length; i++)
		{
			musicSources[i].SetVolume(normalizedAxisValue);
		}
	}

	private void CDAttached(AttachablePoint point, AttachableObject obj)
	{
		RefreshMusicClip();
	}

	private void CDDetached(AttachablePoint point, AttachableObject obj)
	{
		StopMusic();
		RefreshMusicClip();
	}

	private void TrayClosed(GrabbableHinge hinge, bool isInitial)
	{
		if (isTrayOpen || isInitial)
		{
			if (!isInitial)
			{
				AudioManager.Instance.Play(cdAttachPoint.transform.position, cdTrayCloseSound, 1f, 1f);
			}
			isTrayOpen = false;
			trayGrabbable.enabled = false;
			attachPointCollider.enabled = false;
			if (cdAttachPoint.AttachedObjects.Count > 0)
			{
				cdAttachPoint.AttachedObjects[0].PickupableItem.enabled = false;
			}
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "CLOSED");
		}
	}

	public void PowerPushed()
	{
		SetPowerState(!isPoweredOn);
	}

	private void Playbutton_OnButtonPushed(PushableButton obj)
	{
		PlayPausePushed();
	}

	public void PlayPausePushed()
	{
		if (isPoweredOn)
		{
			if (isMusicPlaying)
			{
				StopMusic();
			}
			else if (cdAttachPoint.AttachedObjects.Count > 0)
			{
				RefreshMusicClip();
				PlayMusic();
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
			}
			else
			{
				StopMusic();
			}
		}
	}

	private void Ejectbutton_OnButtonPushed(PushableButton obj)
	{
		EjectPushed();
	}

	public void EjectPushed()
	{
		if (!isTrayOpen && !isTrayAnimating)
		{
			isTrayOpen = true;
			StartCoroutine(OpenTrayInternal());
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "OPENED");
		}
	}

	private IEnumerator OpenTrayInternal()
	{
		isTrayAnimating = true;
		trayHinge.UnlockLower();
		bool previousKinematic = false;
		if (trayGrabbable.Rigidbody != null)
		{
			previousKinematic = trayGrabbable.Rigidbody.isKinematic;
			trayGrabbable.Rigidbody.isKinematic = true;
		}
		Go.to(trayHinge.transform, 0.5f, new GoTweenConfig().localEulerAngles(Vector3.right * 90f).setEaseType(GoEaseType.QuadInOut));
		yield return new WaitForSeconds(0.5f);
		if (trayGrabbable.Rigidbody != null)
		{
			trayGrabbable.Rigidbody.isKinematic = previousKinematic;
		}
		isTrayAnimating = false;
		trayGrabbable.enabled = true;
		attachPointCollider.enabled = true;
		if (cdAttachPoint.AttachedObjects.Count > 0)
		{
			cdAttachPoint.AttachedObjects[0].PickupableItem.enabled = true;
		}
	}

	private void RefreshMusicClip()
	{
		AudioClip musicClip = undefinedMusicSound;
		if (cdAttachPoint.AttachedObjects.Count > 0)
		{
			WorldItem component = cdAttachPoint.AttachedObjects[0].GetComponent<WorldItem>();
			if (component != null)
			{
				for (int i = 0; i < musicDefinitions.Length; i++)
				{
					if (musicDefinitions[i].DiscWorldItem == component.Data)
					{
						musicClip = musicDefinitions[i].MusicClip;
					}
				}
			}
		}
		for (int j = 0; j < musicSources.Length; j++)
		{
			musicSources[j].SetClip(musicClip);
			musicSources[j].SetLooping(true);
		}
	}

	private void PlayMusic()
	{
		if (!isMusicPlaying)
		{
			for (int i = 0; i < musicSources.Length; i++)
			{
				musicSources[i].Play();
			}
			isMusicPlaying = true;
		}
	}

	private void StopMusic()
	{
		if (isMusicPlaying)
		{
			for (int i = 0; i < musicSources.Length; i++)
			{
				musicSources[i].Stop();
			}
			isMusicPlaying = false;
		}
	}
}
