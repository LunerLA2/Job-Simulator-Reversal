using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class ReceiptPrinter : MonoBehaviour
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private GameObject printerObject;

	private PageStatusController currentPage;

	[SerializeField]
	private PageData[] requirePrinterPages;

	[SerializeField]
	private Animation printerAnimation;

	[SerializeField]
	private AnimationClip printerAnimationIn;

	[SerializeField]
	private AnimationClip printerAnimationOut;

	[SerializeField]
	private Rigidbody buttonRB;

	[SerializeField]
	private Transform printerTransform;

	[SerializeField]
	private AudioClip printerInSound;

	[SerializeField]
	private AudioClip printerOutSound;

	[SerializeField]
	private AudioClip printReceiptSound;

	[SerializeField]
	private PageData PrintReceiptData;

	private ProfitCounterController profitCounter;

	private bool wantPrinterForCurrentPage;

	private bool printerIsShown;

	[SerializeField]
	private MechanicReceiptController receiptPrefab;

	[SerializeField]
	private MechanicReceiptController receiptPrefabEndless;

	[SerializeField]
	private Transform receiptSpawnLocation;

	[SerializeField]
	private AudioClip receiptRemovedClip;

	private bool printerFull;

	private MechanicReceiptController currentReceipt;

	public bool PrinterFull
	{
		get
		{
			return printerFull;
		}
	}

	private void Awake()
	{
		StartCoroutine(HidePrinter(true, 0f));
	}

	private void OnEnable()
	{
		if (AutoMechanicManager.Instance != null)
		{
			profitCounter = AutoMechanicManager.Instance.ProfitCounterController;
		}
		if (JobBoardManager.instance != null)
		{
			JobBoardManager instance = JobBoardManager.instance;
			instance.OnPageShown = (Action<PageStatusController>)Delegate.Combine(instance.OnPageShown, new Action<PageStatusController>(PageShown));
			JobBoardManager instance2 = JobBoardManager.instance;
			instance2.OnPageComplete = (Action<PageStatusController>)Delegate.Combine(instance2.OnPageComplete, new Action<PageStatusController>(PageCompleted));
			JobBoardManager instance3 = JobBoardManager.instance;
			instance3.OnTaskComplete = (Action<TaskStatusController>)Delegate.Combine(instance3.OnTaskComplete, new Action<TaskStatusController>(TaskCompleted));
			JobBoardManager instance4 = JobBoardManager.instance;
			instance4.OnBeganWaitingForSkipAction = (Action)Delegate.Combine(instance4.OnBeganWaitingForSkipAction, new Action(OnBeganWaitingForSkipAction));
		}
	}

	private void OnDisable()
	{
		if (JobBoardManager.instance != null)
		{
			JobBoardManager instance = JobBoardManager.instance;
			instance.OnPageShown = (Action<PageStatusController>)Delegate.Remove(instance.OnPageShown, new Action<PageStatusController>(PageShown));
			JobBoardManager instance2 = JobBoardManager.instance;
			instance2.OnPageComplete = (Action<PageStatusController>)Delegate.Remove(instance2.OnPageComplete, new Action<PageStatusController>(PageCompleted));
			JobBoardManager instance3 = JobBoardManager.instance;
			instance3.OnBeganWaitingForSkipAction = (Action)Delegate.Remove(instance3.OnBeganWaitingForSkipAction, new Action(OnBeganWaitingForSkipAction));
		}
	}

	public void PrintReceipt(string aText)
	{
		MechanicReceiptController mechanicReceiptController = null;
		mechanicReceiptController = ((!GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode)) ? (UnityEngine.Object.Instantiate(receiptPrefab, receiptSpawnLocation.position, receiptSpawnLocation.rotation) as MechanicReceiptController) : ((JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal() == null || !(JobBoardManager.instance.EndlessModeStatusController.GetCurrentPageData() == PrintReceiptData)) ? (UnityEngine.Object.Instantiate(receiptPrefabEndless, receiptSpawnLocation.position, receiptSpawnLocation.rotation) as MechanicReceiptController) : (UnityEngine.Object.Instantiate(receiptPrefab, receiptSpawnLocation.position, receiptSpawnLocation.rotation) as MechanicReceiptController)));
		if ((bool)mechanicReceiptController)
		{
			currentReceipt = mechanicReceiptController;
			GrabbableItem component = mechanicReceiptController.GetComponent<GrabbableItem>();
			component.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(component.OnGrabbed, new Action<GrabbableItem>(ReceiptWasGrabbed));
			component.enabled = false;
			printerFull = true;
			AudioManager.Instance.Play(currentReceipt.transform.position, printReceiptSound, 1f, 1f);
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
			mechanicReceiptController.SetText(aText);
			mechanicReceiptController.transform.SetParent(receiptSpawnLocation.transform);
			WorldItem component2 = mechanicReceiptController.GetComponent<WorldItem>();
			if (component2 != null)
			{
				GameEventsManager.Instance.ItemActionOccurred(component2.Data, "CREATED");
			}
			StartCoroutine(ReceiptPrintingLogic(component));
		}
	}

	private IEnumerator ReceiptPrintingLogic(GrabbableItem aReceiptObj)
	{
		aReceiptObj.transform.positionTo(1f, new Vector3(0f, 0.129f, 0f), true);
		yield return new WaitForSeconds(1f);
		if (aReceiptObj != null)
		{
			aReceiptObj.enabled = true;
		}
	}

	private void ReceiptWasGrabbed(GrabbableItem grabbableItem)
	{
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(ReceiptWasGrabbed));
		if ((bool)receiptRemovedClip)
		{
			AudioManager.Instance.Play(currentReceipt.transform.position, receiptRemovedClip, 1f, 1f);
		}
		grabbableItem.transform.SetParent(GlobalStorage.Instance.ContentRoot);
		currentReceipt = null;
		printerFull = false;
	}

	private void PageShown(PageStatusController page)
	{
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			return;
		}
		currentPage = page;
		wantPrinterForCurrentPage = false;
		for (int i = 0; i < requirePrinterPages.Length; i++)
		{
			if (requirePrinterPages[i] == currentPage.Data)
			{
				wantPrinterForCurrentPage = true;
				break;
			}
		}
		AnimatePrinter(wantPrinterForCurrentPage);
	}

	private void PageCompleted(PageStatusController page)
	{
		if ((GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode)) || currentPage == null || page.Data != currentPage.Data)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < requirePrinterPages.Length; i++)
		{
			if (requirePrinterPages[i] == currentPage.Data)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			profitCounter.paused = true;
			AnimatePrinter(false);
		}
	}

	private void TaskCompleted(TaskStatusController task)
	{
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			profitCounter.Reset();
			AnimatePrinter(false);
		}
	}

	private void OnBeganWaitingForSkipAction()
	{
		profitCounter.paused = false;
	}

	public void ButtonPushed()
	{
		if (printerIsShown && !printerFull)
		{
			PrintReceipt("$" + profitCounter.ProfitText);
		}
	}

	public void AnimatePrinter(bool state)
	{
		if (state == printerIsShown)
		{
			return;
		}
		if (!state)
		{
			buttonRB.isKinematic = true;
		}
		printerIsShown = state;
		printerAnimation.Stop();
		printerAnimation.clip = ((!state) ? printerAnimationOut : printerAnimationIn);
		printerAnimation.Play();
		if (printerIsShown)
		{
			StartCoroutine(HidePrinter(false, 0f));
			if (printerInSound != null)
			{
				AudioManager.Instance.Play(printerTransform, printerInSound, 1f, 1f);
			}
			return;
		}
		StartCoroutine(HidePrinter(true, printerAnimation.clip.length));
		if (printerFull)
		{
			UnityEngine.Object.Destroy(currentReceipt.gameObject);
			printerFull = false;
		}
		if (printerOutSound != null)
		{
			AudioManager.Instance.Play(printerTransform, printerOutSound, 1f, 1f);
		}
	}

	private IEnumerator HidePrinter(bool hide, float waitTime)
	{
		if (waitTime > 0f)
		{
			yield return new WaitForSeconds(waitTime);
		}
		printerObject.SetActive(!hide);
		if (!hide)
		{
			buttonRB.isKinematic = false;
		}
	}
}
