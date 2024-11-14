using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MechanicalPushButtonController : MonoBehaviour
{
	[Serializable]
	public class MechancialPushButtonEvent : UnityEvent<MechanicalPushButtonController>
	{
	}

	private enum ButtonStates
	{
		Up = 0,
		Down = 1,
		InTransit = 2
	}

	public delegate void ButtonPress();

	public MechancialPushButtonEvent OnButtonPressed;

	[SerializeField]
	private Rigidbody buttonRigidbody;

	private float buttonExtendedLocalZPos;

	[SerializeField]
	private AudioClip buttonPressedAudioClip;

	private bool isButtonPositionCoRoutineRunning;

	private bool isButtonDownReturnDelayInProgress;

	[SerializeField]
	private float depressedDelay = 0.4f;

	[SerializeField]
	private float buttonPushOutForceMultiplier = 1f;

	private float timeOfLastDepression;

	private ButtonStates lastCompleteButtonState;

	public Action<MechanicalPushButtonController> OnBeganMoving;

	public Action<MechanicalPushButtonController> OnStoppedMoving;

	public float NormalizedPushAmount
	{
		get
		{
			if (buttonExtendedLocalZPos != 0f)
			{
				return 1f - buttonRigidbody.transform.localPosition.z / buttonExtendedLocalZPos;
			}
			return 1f;
		}
	}

	public event ButtonPress OnButtonPress;

	private void Awake()
	{
		buttonExtendedLocalZPos = base.transform.localPosition.z;
	}

	private void OnDisable()
	{
		isButtonPositionCoRoutineRunning = false;
		isButtonDownReturnDelayInProgress = false;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!isButtonPositionCoRoutineRunning)
		{
			StartCoroutine(ButtonInMotionFixedUpdate());
		}
	}

	private IEnumerator ButtonInMotionFixedUpdate()
	{
		if (OnBeganMoving != null)
		{
			OnBeganMoving(this);
		}
		isButtonPositionCoRoutineRunning = true;
		ButtonStates activeButtonState = ButtonStates.InTransit;
		while (activeButtonState != 0 || !buttonRigidbody.IsSleeping())
		{
			Vector3 localVelocity = buttonRigidbody.transform.InverseTransformDirection(buttonRigidbody.velocity);
			if (localVelocity.x != 0f || localVelocity.y != 0f)
			{
				localVelocity.x = 0f;
				localVelocity.y = 0f;
				buttonRigidbody.velocity = buttonRigidbody.transform.TransformDirection(localVelocity);
			}
			Vector3 localPos = buttonRigidbody.transform.localPosition;
			if (localPos.x != 0f || localPos.y != 0f)
			{
				localPos.x = 0f;
				localPos.y = 0f;
				buttonRigidbody.transform.localPosition = localPos;
				buttonRigidbody.position = buttonRigidbody.transform.position;
			}
			yield return null;
			if (isButtonDownReturnDelayInProgress)
			{
				if (Time.time - timeOfLastDepression > depressedDelay)
				{
					isButtonDownReturnDelayInProgress = false;
					buttonRigidbody.isKinematic = false;
					buttonRigidbody.WakeUp();
					activeButtonState = ButtonStates.InTransit;
				}
				else
				{
					activeButtonState = ButtonStates.Down;
				}
			}
			else if (buttonRigidbody.transform.localPosition.z <= 0f)
			{
				Vector3 clamp = buttonRigidbody.transform.localPosition;
				clamp.z = 0f;
				buttonRigidbody.transform.localPosition = clamp;
				buttonRigidbody.position = buttonRigidbody.transform.position;
				if (buttonRigidbody.velocity.z < 0f)
				{
					buttonRigidbody.velocity = Vector3.zero;
				}
				activeButtonState = ButtonStates.Down;
			}
			else if (buttonRigidbody.transform.localPosition.z >= buttonExtendedLocalZPos)
			{
				Vector3 clamp = buttonRigidbody.transform.localPosition;
				clamp.z = buttonExtendedLocalZPos;
				buttonRigidbody.transform.localPosition = clamp;
				buttonRigidbody.position = buttonRigidbody.transform.position;
				if (buttonRigidbody.velocity.z > 0f)
				{
					buttonRigidbody.velocity = Vector3.zero;
				}
				activeButtonState = ButtonStates.Up;
			}
			else
			{
				activeButtonState = ButtonStates.InTransit;
			}
			switch (activeButtonState)
			{
			case ButtonStates.Down:
				if (lastCompleteButtonState == ButtonStates.Up)
				{
					buttonRigidbody.isKinematic = true;
					timeOfLastDepression = Time.time;
					lastCompleteButtonState = ButtonStates.Down;
					isButtonDownReturnDelayInProgress = true;
					if (buttonPressedAudioClip != null)
					{
						AudioManager.Instance.Play(base.transform.position, buttonPressedAudioClip, 1f, 1f);
					}
					OnButtonPressed.Invoke(this);
					if (this.OnButtonPress != null)
					{
						this.OnButtonPress();
					}
				}
				else
				{
					ApplyButtonPushBackForce();
				}
				break;
			case ButtonStates.Up:
				if (lastCompleteButtonState == ButtonStates.Down)
				{
					lastCompleteButtonState = ButtonStates.Up;
				}
				break;
			case ButtonStates.InTransit:
				ApplyButtonPushBackForce();
				break;
			}
		}
		isButtonPositionCoRoutineRunning = false;
		if (OnStoppedMoving != null)
		{
			OnStoppedMoving(this);
		}
	}

	private void ApplyButtonPushBackForce()
	{
		buttonRigidbody.AddRelativeForce(new Vector3(0f, 0f, 1f) * buttonPushOutForceMultiplier);
	}

	public void TestFire()
	{
		Debug.Log("Your button has fired");
	}
}
