using System;
using System.Collections;
using OwlchemyVR;
using TMPro;
using UnityEngine;

public class MathStationController : KitchenTool
{
	[SerializeField]
	private WorldItem mainWorldItem;

	[SerializeField]
	private WorldItemData button1WorldItemData;

	[SerializeField]
	private WorldItemData button2WorldItemData;

	[SerializeField]
	private WorldItemData button3WorldItemData;

	[SerializeField]
	private TextMeshPro outputLabel;

	[SerializeField]
	private GameObject[] simpleModeVisuals;

	[SerializeField]
	private GameObject[] complexModeVisuals;

	[SerializeField]
	private PageData pageToBecomeComplex;

	[SerializeField]
	private PageData[] pagesToVocalizeDuring;

	private bool vocalize;

	private bool isComplex;

	[SerializeField]
	private Rigidbody[] buttonRigidbodies;

	private string currentDisplayString = string.Empty;

	private string currentBinaryInputString = string.Empty;

	private int currentDecimalSum;

	private bool waitingForPlus;

	private bool isReciting;

	private void Awake()
	{
		SetMode(false);
	}

	private void OnEnable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageShown = (Action<PageStatusController>)Delegate.Combine(instance.OnPageShown, new Action<PageStatusController>(PageShown));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnPageComplete = (Action<PageStatusController>)Delegate.Combine(instance2.OnPageComplete, new Action<PageStatusController>(PageCompleted));
	}

	private void OnDisable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageShown = (Action<PageStatusController>)Delegate.Remove(instance.OnPageShown, new Action<PageStatusController>(PageShown));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnPageComplete = (Action<PageStatusController>)Delegate.Remove(instance2.OnPageComplete, new Action<PageStatusController>(PageCompleted));
	}

	private void PageShown(PageStatusController pageStatus)
	{
		if (pageStatus.Data == pageToBecomeComplex)
		{
			SetMode(true);
		}
		for (int i = 0; i < pagesToVocalizeDuring.Length; i++)
		{
			if ((bool)(pagesToVocalizeDuring[i] = pageStatus.Data))
			{
				vocalize = true;
			}
		}
	}

	private void PageCompleted(PageStatusController pageStatus)
	{
		if (pageStatus.Data == pageToBecomeComplex)
		{
			SetMode(false);
		}
		for (int i = 0; i < pagesToVocalizeDuring.Length; i++)
		{
			if ((bool)(pagesToVocalizeDuring[i] = pageStatus.Data))
			{
				vocalize = false;
			}
		}
	}

	public override void OnDismiss()
	{
		base.OnDismiss();
		for (int i = 0; i < buttonRigidbodies.Length; i++)
		{
			buttonRigidbodies[i].isKinematic = true;
		}
	}

	public override void OnSummon()
	{
		base.OnSummon();
		for (int i = 0; i < buttonRigidbodies.Length; i++)
		{
			buttonRigidbodies[i].isKinematic = false;
		}
	}

	private void SetMode(bool _isComplex)
	{
		isComplex = _isComplex;
		for (int i = 0; i < simpleModeVisuals.Length; i++)
		{
			simpleModeVisuals[i].SetActive(!_isComplex);
		}
		for (int j = 0; j < complexModeVisuals.Length; j++)
		{
			complexModeVisuals[j].SetActive(_isComplex);
		}
		Reset();
	}

	private void Reset()
	{
		isReciting = false;
		waitingForPlus = false;
		currentDisplayString = string.Empty;
		currentBinaryInputString = string.Empty;
		currentDecimalSum = 0;
		UpdateLabel();
	}

	private void UpdateLabel()
	{
		outputLabel.text = currentDisplayString;
	}

	public void Button1Pressed()
	{
		if (!isReciting)
		{
			GameEventsManager.Instance.ItemActionOccurred(button1WorldItemData, "USED");
			currentBinaryInputString += "0";
			if (waitingForPlus)
			{
				waitingForPlus = false;
				currentDisplayString += " + ";
			}
			currentDisplayString += ((!isComplex) ? "0" : "[MATH]");
			UpdateLabel();
		}
	}

	public void Button2Pressed()
	{
		if (!isReciting)
		{
			GameEventsManager.Instance.ItemActionOccurred(button2WorldItemData, "USED");
			currentBinaryInputString += "1";
			if (waitingForPlus)
			{
				waitingForPlus = false;
				currentDisplayString += " + ";
			}
			currentDisplayString += ((!isComplex) ? "1" : "[MATH]");
			UpdateLabel();
		}
	}

	public void Button3Pressed()
	{
		if (!isReciting && currentBinaryInputString.Length != 0)
		{
			GameEventsManager.Instance.ItemActionOccurred(button3WorldItemData, "USED");
			AddCurrentBinaryInputStringToTotal();
			currentBinaryInputString = string.Empty;
			waitingForPlus = true;
			UpdateLabel();
		}
	}

	public void SubmitButtonPressed()
	{
		if (!isReciting)
		{
			AddCurrentBinaryInputStringToTotal();
			isReciting = true;
			outputLabel.text = "---";
			StartCoroutine(ReciteAsync());
		}
	}

	private IEnumerator ReciteAsync()
	{
		if (vocalize)
		{
			yield return new WaitForSeconds(0.5f);
			if (isComplex)
			{
				GameEventsManager.Instance.ScriptedCauseOccurred("kidsSayEquation");
			}
			else
			{
				string finalResultInBinary = Convert.ToString(currentDecimalSum, 2);
				for (int i = 0; i < finalResultInBinary.Length; i++)
				{
					string letter = finalResultInBinary.Substring(i, 1);
					if (letter == "1")
					{
						GameEventsManager.Instance.ScriptedCauseOccurred("kidsSayOne");
					}
					else
					{
						GameEventsManager.Instance.ScriptedCauseOccurred("kidsSayZero");
					}
					yield return new WaitForSeconds(0.4f);
				}
			}
		}
		GameEventsManager.Instance.ItemActionOccurred(mainWorldItem.Data, "USED");
		Reset();
	}

	private void AddCurrentBinaryInputStringToTotal()
	{
		if (currentBinaryInputString.Length > 0)
		{
			int num = Convert.ToInt32(currentBinaryInputString, 2);
			currentDecimalSum += num;
		}
	}
}
