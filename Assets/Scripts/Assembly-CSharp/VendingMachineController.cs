using System;
using System.Collections;
using OwlchemyVR;
using TMPro;
using UnityEngine;

public class VendingMachineController : MonoBehaviour
{
	private enum FlapState
	{
		Closed = 0,
		Opening = 1,
		Open = 2,
		Closing = 3
	}

	private const float BODY_RESET_STRENGTH = 15f;

	private const float FLAP_ANGULAR_SPEED = 720f;

	private const float CASH_RETRACT_DISTANCE = 0.18f;

	private const float CASH_RETRACT_DURATION = 0.5f;

	[SerializeField]
	private WorldItem worldItem;

	[SerializeField]
	private float shakeAngularSpeed = 30f;

	[SerializeField]
	private int numShakesToDislodge = 6;

	[SerializeField]
	private Vector3 dislodgeVelocity;

	[SerializeField]
	private AttachableStackPoint[] itemStacks;

	[SerializeField]
	private Transform[] coils;

	[SerializeField]
	private Vector3 coilRotationAxis = Vector3.forward;

	[SerializeField]
	private PlayerPartDetector itemRetrievalZoneHandRegion;

	[SerializeField]
	private GrabbableItem bodyGrabbable;

	[SerializeField]
	private Transform shakeTrackingTransform;

	[SerializeField]
	private AttachablePoint cashAttachPoint;

	[SerializeField]
	private TextMeshPro screenText;

	[SerializeField]
	private ButtonController[] buttons;

	[SerializeField]
	private Transform flap;

	[SerializeField]
	private AudioSourceHelper audioSourceHelper;

	[SerializeField]
	private AudioClip cashRetractSound;

	[SerializeField]
	private AudioClip dispensingSound;

	[SerializeField]
	private AudioClip stuckSound;

	[SerializeField]
	private AudioClip shakeSound;

	[SerializeField]
	private AudioClip flapOpenSound;

	[SerializeField]
	private AudioClip flapCloseSound;

	[SerializeField]
	private AudioClip errorSound;

	[SerializeField]
	private WorldItemData counterfeitDollarData;

	private bool hasMoney;

	private bool counterfeitGiven;

	private bool isVending;

	private bool isStuck;

	private FlapState flapState;

	private float displayFlashTimeLeft;

	private bool nextItemWillGetStuck;

	private float prevBodyAngle;

	private int prevBodySwayDirection;

	private int numShakesLeftToDislodge;

	private Vector3 initialCashLocalPos;

	private Coroutine flapCoroutine;

	private AttachableObject stuckItem;

	private float lastRequestTime;

	private void Awake()
	{
		initialCashLocalPos = cashAttachPoint.transform.localPosition;
		nextItemWillGetStuck = false;
	}

	private void OnEnable()
	{
		GrabbableItem grabbableItem = bodyGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(BodyGrabbed));
		GrabbableItem grabbableItem2 = bodyGrabbable;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem2.OnReleased, new Action<GrabbableItem>(BodyReleased));
		cashAttachPoint.OnObjectWasAttached += CashAttached;
		PlayerPartDetector playerPartDetector = itemRetrievalZoneHandRegion;
		playerPartDetector.OnHandEntered = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Combine(playerPartDetector.OnHandEntered, new Action<PlayerPartDetector, InteractionHandController>(HandEnteredItemRetrievalZone));
		PlayerPartDetector playerPartDetector2 = itemRetrievalZoneHandRegion;
		playerPartDetector2.OnHandExited = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Combine(playerPartDetector2.OnHandExited, new Action<PlayerPartDetector, InteractionHandController>(HandExitedItemRetrievalZone));
		hasMoney = false;
		RefreshDisplay();
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = bodyGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(BodyGrabbed));
		GrabbableItem grabbableItem2 = bodyGrabbable;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem2.OnReleased, new Action<GrabbableItem>(BodyReleased));
		cashAttachPoint.OnObjectWasAttached -= CashAttached;
		PlayerPartDetector playerPartDetector = itemRetrievalZoneHandRegion;
		playerPartDetector.OnHandEntered = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Remove(playerPartDetector.OnHandEntered, new Action<PlayerPartDetector, InteractionHandController>(HandEnteredItemRetrievalZone));
		PlayerPartDetector playerPartDetector2 = itemRetrievalZoneHandRegion;
		playerPartDetector2.OnHandExited = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Remove(playerPartDetector2.OnHandExited, new Action<PlayerPartDetector, InteractionHandController>(HandExitedItemRetrievalZone));
	}

	public void SetNextItemWillGetStuck(bool _nextItemWillGetStuck)
	{
		nextItemWillGetStuck = _nextItemWillGetStuck;
	}

	private void RefreshDisplay()
	{
		if (isStuck)
		{
			screenText.text = "STUCK";
		}
		else if (isVending)
		{
			screenText.text = "VENDING";
		}
		else if (counterfeitGiven)
		{
			screenText.text = "NICE TRY";
		}
		else if (!hasMoney)
		{
			screenText.text = "NO MONEY";
		}
		else
		{
			screenText.text = "READY";
		}
	}

	public void SelectItem(int index)
	{
		if (!(Time.time - lastRequestTime < 0.25f))
		{
			lastRequestTime = Time.time;
			TryPurchaseItem(index);
		}
	}

	public void DislodgeItem()
	{
		if (isStuck && stuckItem != null)
		{
			GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "OPENED");
			DropItem(stuckItem);
			stuckItem.GetComponent<Rigidbody>().velocity = base.transform.TransformVector(dislodgeVelocity);
			stuckItem = null;
			isStuck = false;
			RefreshDisplay();
		}
	}

	private void TryPurchaseItem(int i)
	{
		if (isVending)
		{
			return;
		}
		if (isStuck || !hasMoney)
		{
			if (errorSound != null)
			{
				audioSourceHelper.SetClip(errorSound);
				audioSourceHelper.Stop();
				audioSourceHelper.Play();
			}
			FlashDisplayForSeconds(1f);
		}
		else
		{
			GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "ACTIVATED");
			hasMoney = false;
			RefreshDisplay();
			StartCoroutine(DispenseItemAsync(i));
		}
	}

	private void FlashDisplayForSeconds(float duration)
	{
		displayFlashTimeLeft = Mathf.Max(displayFlashTimeLeft, duration);
	}

	private void Update()
	{
		if (bodyGrabbable.IsCurrInHand)
		{
			UpdateShake();
		}
		UpdateDisplayFlash();
	}

	private void UpdateShake()
	{
		float num;
		for (num = shakeTrackingTransform.transform.localEulerAngles.x; num > 180f; num -= 360f)
		{
		}
		for (; num < -180f; num += 360f)
		{
		}
		float num2 = (num - prevBodyAngle) / Time.deltaTime;
		int num3 = (int)Mathf.Clamp(num2 / shakeAngularSpeed, -1f, 1f);
		if (num3 != prevBodySwayDirection && num3 != 0)
		{
			if (shakeSound != null)
			{
				audioSourceHelper.SetClip(shakeSound);
				audioSourceHelper.Stop();
				audioSourceHelper.Play();
			}
			numShakesLeftToDislodge--;
			if (numShakesLeftToDislodge == 0)
			{
				DislodgeItem();
			}
			prevBodySwayDirection = num3;
		}
		prevBodyAngle = num;
	}

	private void UpdateDisplayFlash()
	{
		if (displayFlashTimeLeft > 0f)
		{
			displayFlashTimeLeft -= Time.deltaTime;
			Color color = screenText.color;
			color.a = (int)(displayFlashTimeLeft * 5f) % 2;
			screenText.color = color;
			if (displayFlashTimeLeft <= 0f)
			{
				displayFlashTimeLeft = 0f;
				color = screenText.color;
				color.a = 1f;
				screenText.color = color;
			}
		}
	}

	private void CashAttached(AttachablePoint point, AttachableObject o)
	{
		GrabbableItem component = o.GetComponent<GrabbableItem>();
		if (component != null)
		{
			component.enabled = false;
		}
		StartCoroutine(RetractCashAsync(o));
	}

	private void HandEnteredItemRetrievalZone(PlayerPartDetector playerPartDetector, InteractionHandController hand)
	{
		if (flapState == FlapState.Closed || flapState == FlapState.Closing)
		{
			if (flapCoroutine != null)
			{
				StopCoroutine(flapCoroutine);
			}
			flapCoroutine = StartCoroutine(OpenFlapAsync());
		}
	}

	private void HandExitedItemRetrievalZone(PlayerPartDetector playerPartDetector, InteractionHandController hand)
	{
		if ((flapState == FlapState.Open || flapState == FlapState.Opening) && itemRetrievalZoneHandRegion.DetectedHands.Count == 0)
		{
			if (flapCoroutine != null)
			{
				StopCoroutine(flapCoroutine);
			}
			flapCoroutine = StartCoroutine(CloseFlapAsync());
		}
	}

	private IEnumerator RetractCashAsync(AttachableObject cashObj)
	{
		WorldItemData cashWorldItemData = cashObj.GetComponent<WorldItem>().Data;
		if (cashRetractSound != null)
		{
			audioSourceHelper.SetClip(cashRetractSound);
			audioSourceHelper.Stop();
			audioSourceHelper.Play();
		}
		float t = 0f;
		while (t < 0.5f)
		{
			cashAttachPoint.transform.Translate(0f, 0f, Time.deltaTime * 0.18f / 0.5f);
			t += Time.deltaTime;
			yield return null;
		}
		cashAttachPoint.transform.localPosition = initialCashLocalPos;
		if (cashWorldItemData != counterfeitDollarData)
		{
			hasMoney = true;
			RefreshDisplay();
		}
		else
		{
			StartCoroutine(CounterfeitGivenRoutine());
		}
		cashObj.Detach();
		UnityEngine.Object.Destroy(cashObj.gameObject);
		cashAttachPoint.gameObject.SetActive(false);
	}

	private IEnumerator CounterfeitGivenRoutine()
	{
		yield return new WaitForEndOfFrame();
		counterfeitGiven = true;
		RefreshDisplay();
		if (errorSound != null)
		{
			audioSourceHelper.SetClip(errorSound);
			audioSourceHelper.Stop();
			audioSourceHelper.Play();
		}
		FlashDisplayForSeconds(2f);
		yield return new WaitForSeconds(2f);
		cashAttachPoint.gameObject.SetActive(true);
		counterfeitGiven = false;
		RefreshDisplay();
	}

	private IEnumerator OpenFlapAsync()
	{
		flapState = FlapState.Opening;
		if (flapOpenSound != null)
		{
			AudioManager.Instance.Play(flap, flapOpenSound, 1f, 1f);
		}
		Vector3 flapLocalEulers = flap.localEulerAngles;
		flapLocalEulers.y = 0f;
		flapLocalEulers.z = 0f;
		while (flapLocalEulers.x > -80f)
		{
			flapLocalEulers.x -= 720f * Time.deltaTime;
			flap.localEulerAngles = flapLocalEulers;
			yield return null;
		}
		flapLocalEulers.x = -80f;
		flap.localEulerAngles = flapLocalEulers;
		flapState = FlapState.Open;
	}

	private IEnumerator CloseFlapAsync()
	{
		flapState = FlapState.Closing;
		if (flapCloseSound != null)
		{
			AudioManager.Instance.Play(flap, flapCloseSound, 1f, 1f);
		}
		Vector3 flapLocalEulers = flap.localEulerAngles;
		flapLocalEulers.y = 0f;
		flapLocalEulers.z = 0f;
		while (flapLocalEulers.x > 180f)
		{
			flapLocalEulers.x -= 360f;
		}
		while (flapLocalEulers.x < -180f)
		{
			flapLocalEulers.x += 360f;
		}
		while (flapLocalEulers.x < 0f)
		{
			flapLocalEulers.x += 720f * Time.deltaTime;
			flap.localEulerAngles = flapLocalEulers;
			yield return null;
		}
		flapLocalEulers.x = 0f;
		flap.localEulerAngles = flapLocalEulers;
		flapState = FlapState.Closed;
	}

	private IEnumerator DispenseItemAsync(int i)
	{
		isVending = true;
		RefreshDisplay();
		AttachableStackPoint itemStack = itemStacks[i];
		Transform coil = coils[i];
		AttachableObject item = itemStack.TopmostAttachedObject;
		item.Detach();
		item.transform.SetParent(itemStack.transform, true);
		item.GetComponent<Rigidbody>().isKinematic = true;
		item.GetComponent<GrabbableItem>().enabled = false;
		item.GetComponent<EdibleItem>().enabled = false;
		Quaternion coilInitialRot = coil.localRotation;
		if (dispensingSound != null)
		{
			audioSourceHelper.SetClip(dispensingSound);
			audioSourceHelper.Stop();
			audioSourceHelper.Play();
		}
		float progress = 0f;
		while (progress < 1f)
		{
			coil.localRotation = coilInitialRot * Quaternion.AngleAxis(progress * 360f, coilRotationAxis);
			progress += Time.deltaTime / itemStack.refillDuration;
			yield return null;
		}
		coil.localRotation = coilInitialRot;
		if (nextItemWillGetStuck)
		{
			if (stuckSound != null)
			{
				audioSourceHelper.SetClip(stuckSound);
				audioSourceHelper.Stop();
				audioSourceHelper.Play();
			}
			nextItemWillGetStuck = false;
			stuckItem = item;
			isStuck = true;
			numShakesLeftToDislodge = numShakesToDislodge;
			RefreshDisplay();
		}
		else
		{
			DropItem(item);
		}
		cashAttachPoint.gameObject.SetActive(true);
		isVending = false;
		RefreshDisplay();
	}

	private void DropItem(AttachableObject item)
	{
		item.transform.SetParent(null);
		item.GetComponent<Rigidbody>().isKinematic = false;
		item.GetComponent<GrabbableItem>().enabled = true;
		item.GetComponent<EdibleItem>().enabled = true;
	}

	private void BodyGrabbed(GrabbableItem grabbable)
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].Slider.UnlockUpper();
			buttons[i].Slider.enabled = false;
			buttons[i].GetComponent<ConfigurableJoint>().zMotion = ConfigurableJointMotion.Locked;
			buttons[i].GetComponent<Rigidbody>().isKinematic = true;
		}
	}

	private void BodyReleased(GrabbableItem grabbable)
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].GetComponent<ConfigurableJoint>().zMotion = ConfigurableJointMotion.Free;
			buttons[i].Slider.enabled = true;
			buttons[i].GetComponent<Rigidbody>().isKinematic = false;
		}
		prevBodyAngle = 0f;
		prevBodySwayDirection = 0;
	}
}
