using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class PhoneController : MonoBehaviour
{
	private const string jobBotNumber = "BF5";

	private AudioSourceHelper phoneSpeaker;

	[SerializeField]
	private AttachablePoint handsetAttachPoint;

	[SerializeField]
	private AudioSourceHelper phoneBase;

	[SerializeField]
	private Animation phoneRingAnimation;

	[SerializeField]
	private GameObject ringLightOn;

	[SerializeField]
	private GameObject ringLightOff;

	[SerializeField]
	private Renderer lightsOffRenderer;

	[SerializeField]
	private AudioClip line0Voice;

	[SerializeField]
	private AudioClip line1Voice;

	[SerializeField]
	private AudioClip phoneRing;

	[SerializeField]
	private AudioClip successVoice;

	[SerializeField]
	private AudioClip failVoice;

	[SerializeField]
	private AudioClip dialTone;

	[SerializeField]
	private AudioClip line0Button;

	[SerializeField]
	private AudioClip line1Button;

	[SerializeField]
	private AudioClip phoneExtraClip;

	[SerializeField]
	private AudioClip[] sweetenTheDealClips;

	[SerializeField]
	private AudioClip[] powerMoveClips;

	[SerializeField]
	private PageData sweetenTheDealPage;

	[SerializeField]
	private PageData powerMovePage;

	private bool isPhoneOnHook = true;

	private bool isPhoneRinging;

	private bool isCallInProgress;

	private int requiredLine = -1;

	private string dialedDigits;

	public Action OnPhoneSuccess;

	public Action OnPhoneFailure;

	private bool isEndlessMode;

	private EndlessModeStatusController endlessStatusController;

	private PickupableItem phonePickup;

	private void Awake()
	{
		StartCoroutine(StartHeadset());
	}

	private IEnumerator StartHeadset()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		isEndlessMode = GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode);
		if (isEndlessMode)
		{
			StartCoroutine(WaitAFrameAndSubscribeToTaskSkipped());
		}
		StartHeadset(handsetAttachPoint.GetAttachedObject(0));
	}

	private void OnEnable()
	{
		handsetAttachPoint.OnObjectWasAttached += AttachedHandset;
		handsetAttachPoint.OnObjectWasDetached += DetachedHandset;
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageStarted = (Action<PageStatusController>)Delegate.Combine(instance.OnPageStarted, new Action<PageStatusController>(OnPageStarted));
	}

	private IEnumerator WaitAFrameAndSubscribeToTaskSkipped()
	{
		yield return null;
		endlessStatusController = JobBoardManager.instance.EndlessModeStatusController;
		EndlessModeStatusController endlessModeStatusController = endlessStatusController;
		endlessModeStatusController.OnTaskComplete = (Action<TaskStatusController>)Delegate.Combine(endlessModeStatusController.OnTaskComplete, new Action<TaskStatusController>(TaskSkipped));
		Debug.Log("Skip Task Subscribed for phone");
	}

	private void OnDisable()
	{
		handsetAttachPoint.OnObjectWasAttached -= AttachedHandset;
		handsetAttachPoint.OnObjectWasDetached -= DetachedHandset;
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageStarted = (Action<PageStatusController>)Delegate.Remove(instance.OnPageStarted, new Action<PageStatusController>(OnPageStarted));
		if (isEndlessMode && endlessStatusController != null)
		{
			EndlessModeStatusController endlessModeStatusController = endlessStatusController;
			endlessModeStatusController.OnTaskComplete = (Action<TaskStatusController>)Delegate.Remove(endlessModeStatusController.OnTaskComplete, new Action<TaskStatusController>(TaskSkipped));
		}
	}

	public void ForceStartRinging(float delay = 0f)
	{
		Invoke("StartRinging", delay);
	}

	private void StartRinging()
	{
		if (!(phoneSpeaker == null))
		{
			if (!isPhoneOnHook && isEndlessMode)
			{
				StartCoroutine(RespawnPhoneAndStartRingingAgain());
			}
			else if (!isPhoneRinging && isPhoneOnHook)
			{
				isPhoneRinging = true;
				phoneBase.Stop();
				phoneBase.SetLooping(true);
				phoneBase.SetClip(phoneRing);
				phoneBase.Play();
				ringLightOff.SetActive(true);
				ringLightOn.SetActive(true);
				phoneRingAnimation.Play();
			}
		}
	}

	private void AttachedHandset(AttachablePoint point, AttachableObject obj)
	{
		StartHeadset(obj);
	}

	private void StartHeadset(AttachableObject obj)
	{
		phonePickup = obj.PickupableItem;
		phoneSpeaker = obj.GetComponentInChildren<AudioSourceHelper>();
		if (!(phoneSpeaker == null))
		{
			isCallInProgress = false;
			requiredLine = -1;
			phoneSpeaker.Stop();
			isPhoneOnHook = true;
		}
	}

	private void DetachedHandset(AttachablePoint point, AttachableObject obj)
	{
		if (phoneSpeaker == null)
		{
			return;
		}
		isPhoneOnHook = false;
		if (isPhoneRinging)
		{
			isCallInProgress = true;
			phoneBase.Stop();
			isPhoneRinging = false;
			phoneRingAnimation.Stop();
			ringLightOff.SetActive(true);
			ringLightOn.SetActive(false);
			if (!isEndlessMode)
			{
				requiredLine = UnityEngine.Random.Range(0, 2);
				if (requiredLine == 0)
				{
					phoneSpeaker.SetClip(line0Voice);
				}
				else
				{
					phoneSpeaker.SetClip(line1Voice);
				}
				phoneSpeaker.SetLooping(true);
				phoneSpeaker.Play();
			}
			else
			{
				TimeManager.Invoke(phoneSpeaker.Play, 1f);
			}
		}
		else
		{
			phoneSpeaker.SetClip(dialTone);
			phoneSpeaker.SetLooping(true);
			phoneSpeaker.Play();
		}
	}

	public void Line0Selected()
	{
		if (phoneSpeaker == null)
		{
			return;
		}
		if (!isPhoneOnHook)
		{
			if (requiredLine == 0)
			{
				phoneSpeaker.Stop();
				phoneSpeaker.SetClip(successVoice);
				phoneSpeaker.SetLooping(false);
				phoneSpeaker.Play();
				PhoneSuccessEvent();
				StartCoroutine(EndOfCallVoiceDuration(successVoice));
			}
			else if (requiredLine == 1)
			{
				phoneSpeaker.Stop();
				phoneSpeaker.SetClip(failVoice);
				phoneSpeaker.SetLooping(false);
				phoneSpeaker.Play();
				PhoneFailureEvent();
				StartCoroutine(EndOfCallVoiceDuration(failVoice));
			}
			requiredLine = -1;
		}
		if (!isPhoneOnHook && requiredLine == -1 && !isCallInProgress)
		{
			StartCoroutine(DialToneButtons(0));
		}
	}

	public void Line1Selected()
	{
		if (phoneSpeaker == null)
		{
			return;
		}
		if (!isPhoneOnHook)
		{
			if (requiredLine == 1)
			{
				phoneSpeaker.Stop();
				phoneSpeaker.SetClip(successVoice);
				phoneSpeaker.SetLooping(false);
				phoneSpeaker.Play();
				PhoneSuccessEvent();
				StartCoroutine(EndOfCallVoiceDuration(successVoice));
			}
			else if (requiredLine == 0)
			{
				phoneSpeaker.Stop();
				phoneSpeaker.SetClip(failVoice);
				phoneSpeaker.SetLooping(false);
				phoneSpeaker.Play();
				PhoneFailureEvent();
				StartCoroutine(EndOfCallVoiceDuration(failVoice));
			}
			requiredLine = -1;
		}
		if (!isPhoneOnHook && requiredLine == -1 && !isCallInProgress)
		{
			StartCoroutine(DialToneButtons(1));
		}
	}

	private IEnumerator EndOfCallVoiceDuration(AudioClip ac)
	{
		yield return new WaitForSeconds(ac.length);
		isCallInProgress = false;
	}

	private IEnumerator DialToneButtons(int button)
	{
		if (phoneSpeaker == null)
		{
			yield return null;
		}
		AudioClip targetClip = ((button != 0) ? line1Button : line0Button);
		if (CheckSum(button))
		{
			targetClip = phoneExtraClip;
			if (ExtraPrefs.ExtraProgress < 1)
			{
				ExtraPrefs.ExtraProgress = 1;
			}
		}
		phoneSpeaker.Stop();
		phoneSpeaker.SetClip(targetClip);
		phoneSpeaker.SetLooping(false);
		phoneSpeaker.Play();
		yield return new WaitForSeconds(targetClip.length);
		if (!isPhoneOnHook && requiredLine == -1)
		{
			phoneSpeaker.Stop();
			phoneSpeaker.SetClip(dialTone);
			phoneSpeaker.SetLooping(true);
			phoneSpeaker.Play();
		}
	}

	private bool CheckSum(int button)
	{
		int num = "BF5".Length * 4;
		dialedDigits += button;
		if (dialedDigits.Length < num)
		{
			return false;
		}
		if (dialedDigits.Length > num)
		{
			dialedDigits = dialedDigits.Substring(1);
		}
		string text = Convert.ToString(Convert.ToInt64("BF5", 16), 2).PadLeft(num, '0');
		if (text == dialedDigits)
		{
			return true;
		}
		return false;
	}

	private void PhoneSuccessEvent()
	{
		if (OnPhoneSuccess != null)
		{
			OnPhoneSuccess();
		}
	}

	private void PhoneFailureEvent()
	{
		if (OnPhoneFailure != null)
		{
			OnPhoneFailure();
		}
	}

	private void OnPageStarted(PageStatusController pageStatus)
	{
		if (isEndlessMode)
		{
			if (pageStatus.Data == sweetenTheDealPage)
			{
				phoneSpeaker.SetClip(sweetenTheDealClips[UnityEngine.Random.Range(0, sweetenTheDealClips.Length)]);
				StartRinging();
				phoneSpeaker.SetLooping(false);
			}
			else if (pageStatus.Data == powerMovePage)
			{
				phoneSpeaker.SetClip(powerMoveClips[UnityEngine.Random.Range(0, sweetenTheDealClips.Length)]);
				StartRinging();
				phoneSpeaker.SetLooping(false);
			}
			else
			{
				phoneSpeaker.SetClip(null);
				phoneSpeaker.SetLooping(true);
			}
		}
	}

	private IEnumerator RespawnPhoneAndStartRingingAgain()
	{
		PageData page = JobBoardManager.instance.GetCurrentPageData();
		if (phonePickup != null)
		{
			if (phonePickup.IsCurrInHand)
			{
				phonePickup.CurrInteractableHand.TryRelease();
				yield return null;
			}
			UnityEngine.Object.Destroy(phonePickup.gameObject);
		}
		yield return new WaitForSeconds(0.1f);
		if (page == sweetenTheDealPage)
		{
			phoneSpeaker.SetClip(sweetenTheDealClips[UnityEngine.Random.Range(0, sweetenTheDealClips.Length)]);
			phoneSpeaker.SetLooping(false);
		}
		else if (page == powerMovePage)
		{
			phoneSpeaker.SetClip(powerMoveClips[UnityEngine.Random.Range(0, sweetenTheDealClips.Length)]);
			phoneSpeaker.SetLooping(false);
		}
		StartRinging();
	}

	private void TaskSkipped(TaskStatusController task)
	{
		if (task.IsSkipped && isPhoneRinging)
		{
			isCallInProgress = true;
			phoneBase.Stop();
			isPhoneRinging = false;
			phoneRingAnimation.Stop();
			ringLightOff.SetActive(true);
			lightsOffRenderer.enabled = true;
			ringLightOn.SetActive(false);
			phoneSpeaker.Stop();
			phoneSpeaker.SetClip(dialTone);
			phoneSpeaker.SetLooping(true);
		}
	}
}
