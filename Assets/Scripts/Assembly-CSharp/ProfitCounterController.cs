using System;
using System.Collections;
using OwlchemyVR;
using TMPro;
using UnityEngine;

public class ProfitCounterController : MonoBehaviour
{
	private const float maxProfits = 1E+10f;

	private const float TIME_TO_NEXT_SCROLL = 0.33f;

	[SerializeField]
	private TextMeshPro profitText;

	[SerializeField]
	private float dollarsPerSecond = 10f;

	[SerializeField]
	private float dollarsPerItemGrabbed = 100f;

	[SerializeField]
	private GarageChainController chainController;

	private float profitValue;

	private float secondCounter;

	private string scrollText;

	private bool isScrollingText;

	private float timeToLetter;

	private int currentLetter;

	public bool paused;

	private bool isEndlessMode;

	private int skipFrames;

	public string ProfitText
	{
		get
		{
			return profitText.text;
		}
	}

	private void OnEnable()
	{
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			isEndlessMode = true;
		}
		Reset();
	}

	private void OnDisable()
	{
	}

	public void ScrollText(string text)
	{
		scrollText = text;
		isScrollingText = true;
		timeToLetter = 0.33f;
		currentLetter = 0;
	}

	public void Reset()
	{
		profitValue = 0f;
		paused = true;
		UpdateText();
	}

	private void Update()
	{
		if (paused)
		{
			return;
		}
		secondCounter += Time.deltaTime;
		if (secondCounter >= 0.01f)
		{
			float num = ((!isEndlessMode) ? (dollarsPerSecond * secondCounter) : (dollarsPerSecond * secondCounter * 2f));
			profitValue += num;
			if (profitValue > 1E+10f)
			{
				profitValue = 1E+10f;
			}
			if (!isScrollingText)
			{
				skipFrames++;
				if (skipFrames >= 4)
				{
					skipFrames = 0;
					UpdateText();
				}
			}
			secondCounter = 0f;
		}
		if (isScrollingText)
		{
			UpdateTextScroll();
		}
	}

	private void UpdateText()
	{
		profitText.text = Mathf.RoundToInt(profitValue).ToString();
	}

	private void UpdateTextScroll()
	{
		int num = 12;
		timeToLetter -= Time.deltaTime;
		if (timeToLetter <= 0f)
		{
			timeToLetter = 0.33f;
			currentLetter++;
			if (currentLetter >= scrollText.Length)
			{
				isScrollingText = false;
				return;
			}
		}
		string text = ((currentLetter < num) ? scrollText.Substring(0, currentLetter).PadLeft(11) : ((currentLetter > scrollText.Length) ? scrollText.Substring(currentLetter - scrollText.Length - 1).PadRight(11) : scrollText.Substring(currentLetter, Mathf.Min(num, scrollText.Length - currentLetter))));
		profitText.text = text;
	}

	private void OnAnythingGrabbed(InteractionHandController hand, GrabbableItem grabbed)
	{
		profitValue += dollarsPerItemGrabbed;
		UpdateText();
	}

	private IEnumerator OnEnableInternal()
	{
		yield return new WaitForEndOfFrame();
		InteractionHandController rightHand = GlobalStorage.Instance.MasterHMDAndInputController.RightHand;
		rightHand.OnGrabSuccess = (Action<InteractionHandController, GrabbableItem>)Delegate.Combine(rightHand.OnGrabSuccess, new Action<InteractionHandController, GrabbableItem>(OnAnythingGrabbed));
		InteractionHandController leftHand = GlobalStorage.Instance.MasterHMDAndInputController.LeftHand;
		leftHand.OnGrabSuccess = (Action<InteractionHandController, GrabbableItem>)Delegate.Combine(leftHand.OnGrabSuccess, new Action<InteractionHandController, GrabbableItem>(OnAnythingGrabbed));
	}
}
