using System;
using System.Collections;
using OwlchemyVR;
using TMPro;
using UnityEngine;

public class Magic1Ball : MonoBehaviour
{
	private const int HAPTICS_SHACK_PULSE_RATE_MICRO_SEC = 1250;

	private const float HAPTICS_SHACK_LENGTH_SECONDS = 0.1f;

	[SerializeField]
	private TextMeshPro textmesh;

	private HapticInfoObject shakeDispenseHaptic;

	private PickupableItem item;

	[SerializeField]
	private float shakeTime = 0.3f;

	[SerializeField]
	private float requiredDownwardVelocity = 10f;

	private bool isDispensing;

	private Vector3 prevPos;

	private float timeSinceLastRelease;

	private bool wasPrevDirectionNegative;

	[SerializeField]
	private AudioClip[] shakeClips;

	private ElementSequence<AudioClip> shakeClipSequence;

	private float timeOfLastJumble;

	private Coroutine textFadeIn;

	private Color targetColor;

	private void Awake()
	{
		shakeClipSequence = new ElementSequence<AudioClip>(shakeClips);
		item = GetComponent<PickupableItem>();
		if (item == null)
		{
			Debug.LogError("Magic1Ball needs PickupableItem!", base.transform);
		}
		shakeDispenseHaptic = new HapticInfoObject(1250f, 0.1f);
		shakeDispenseHaptic.DeactiveHaptic();
	}

	private void OnEnable()
	{
		PickupableItem pickupableItem = item;
		pickupableItem.OnReleased = (Action<GrabbableItem>)Delegate.Combine(pickupableItem.OnReleased, new Action<GrabbableItem>(ItemReleased));
		prevPos = base.transform.position;
	}

	private void OnDisable()
	{
		PickupableItem pickupableItem = item;
		pickupableItem.OnReleased = (Action<GrabbableItem>)Delegate.Remove(pickupableItem.OnReleased, new Action<GrabbableItem>(ItemReleased));
	}

	private void ItemReleased(GrabbableItem grabbedItem)
	{
		if (shakeDispenseHaptic.IsRunning && item.CurrInteractableHand != null)
		{
			item.CurrInteractableHand.HapticsController.RemoveHaptic(shakeDispenseHaptic);
		}
	}

	private void Update()
	{
		if (!item.IsCurrInHand)
		{
			return;
		}
		Vector3 direction = (base.transform.position - prevPos) / Time.deltaTime;
		prevPos = base.transform.position;
		float y = base.transform.InverseTransformDirection(direction).y;
		if (y < 0f)
		{
			wasPrevDirectionNegative = true;
		}
		if (isDispensing)
		{
			timeSinceLastRelease += Time.deltaTime;
			if (timeSinceLastRelease > shakeTime)
			{
				isDispensing = false;
			}
		}
		else
		{
			if (!wasPrevDirectionNegative || !(y > requiredDownwardVelocity))
			{
				return;
			}
			timeSinceLastRelease = 0f;
			isDispensing = true;
			wasPrevDirectionNegative = false;
			JumbleAnswer();
			AudioManager.Instance.Play(base.transform.position, shakeClipSequence.GetNext(), 1f, 1f);
			if (item != null)
			{
				if (item.CurrInteractableHand.HapticsController.ContainHaptic(shakeDispenseHaptic))
				{
					shakeDispenseHaptic.Restart();
					return;
				}
				shakeDispenseHaptic.Restart();
				item.CurrInteractableHand.HapticsController.AddNewHaptic(shakeDispenseHaptic);
			}
		}
	}

	private void JumbleAnswer()
	{
		float num = UnityEngine.Random.Range(0f, 1f);
		if (num < 0.4f)
		{
			textmesh.text = "YOU'RE \n FIRED";
			targetColor = Color.red;
		}
		else if (num < 0.8f)
		{
			textmesh.text = "YOU'RE \n HIRED";
			targetColor = Color.green;
		}
		else
		{
			textmesh.text = "ASK \n AGAIN";
			targetColor = Color.yellow;
		}
		textmesh.color = targetColor;
		timeOfLastJumble = Time.timeSinceLevelLoad;
		if (textFadeIn != null)
		{
			StopCoroutine(textFadeIn);
		}
		textFadeIn = StartCoroutine(FadeInText());
	}

	private IEnumerator FadeInText()
	{
		textmesh.color = Color.clear;
		while (timeOfLastJumble + 0.8f > Time.timeSinceLevelLoad)
		{
			yield return null;
		}
		while (textmesh.color.a < 0.98f)
		{
			textmesh.color = Color.Lerp(textmesh.color, targetColor, Time.deltaTime * 1.2f);
			yield return null;
		}
		yield return null;
	}
}
