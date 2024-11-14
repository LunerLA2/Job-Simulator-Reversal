using System;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(GrabbableItem))]
public class DialSelector : MonoBehaviour
{
	public enum DialAxis
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	private const int HAPTICS_CLICK_PULSE_RATE_MCIRO_SEC = 1000;

	private const float HAPTICS_CLICK_LENGTH_SECONDS = 0.03f;

	[SerializeField]
	private DialAxis axis;

	[SerializeField]
	private float[] optionAngles;

	[SerializeField]
	private UnityEvent[] optionEvents;

	[SerializeField]
	private float snapStrength = 10f;

	[SerializeField]
	private float snapWithinAngle = 10f;

	[SerializeField]
	private int initialSelectionIndex;

	[SerializeField]
	private AudioClip clickSound;

	private GrabbableItem grabbable;

	private int selectionIndex = -1;

	private int prevClosestIndex = -1;

	private HapticInfoObject hapticObject;

	public GrabbableItem Grabbable
	{
		get
		{
			return grabbable;
		}
	}

	public int InitialSelectionIndex
	{
		get
		{
			return initialSelectionIndex;
		}
	}

	public event Action<DialSelector, int> OnSelectionChanged;

	private void Awake()
	{
		grabbable = GetComponent<GrabbableItem>();
		for (int i = 0; i < optionAngles.Length; i++)
		{
			optionAngles[i] = Mathf.Repeat(optionAngles[i], 360f);
		}
		hapticObject = new HapticInfoObject(1000f, 0.03f);
		hapticObject.DeactiveHaptic();
	}

	private void OnEnable()
	{
		GrabbableItem grabbableItem = grabbable;
		grabbableItem.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = grabbable;
		grabbableItem.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void Start()
	{
		Select(initialSelectionIndex);
	}

	private Quaternion CalculateLocalRotationFromAngle(float angle)
	{
		if (axis == DialAxis.X)
		{
			return Quaternion.Euler(angle, 0f, 0f);
		}
		if (axis == DialAxis.Y)
		{
			return Quaternion.Euler(0f, angle, 0f);
		}
		if (axis == DialAxis.Z)
		{
			return Quaternion.Euler(0f, 0f, angle);
		}
		return Quaternion.identity;
	}

	private void SetAngle(float angle)
	{
		base.transform.localRotation = CalculateLocalRotationFromAngle(angle);
	}

	public void Select(int newIndex, bool suppressEvents = false)
	{
		if (newIndex < 0 || newIndex >= optionAngles.Length)
		{
			return;
		}
		SetAngle(optionAngles[newIndex]);
		if (newIndex == selectionIndex)
		{
			return;
		}
		selectionIndex = newIndex;
		if (!suppressEvents)
		{
			if (this.OnSelectionChanged != null)
			{
				this.OnSelectionChanged(this, selectionIndex);
			}
			if (newIndex >= 0 && newIndex < optionEvents.Length)
			{
				optionEvents[newIndex].Invoke();
			}
		}
	}

	public int GetClosestSelectionIndex()
	{
		float currentAngle = GetCurrentAngle();
		float num = float.MaxValue;
		int num2 = -1;
		for (int i = 0; i < optionAngles.Length; i++)
		{
			float angleDelta = GetAngleDelta(currentAngle, optionAngles[i]);
			if (angleDelta < num)
			{
				num = angleDelta;
				num2 = i;
			}
		}
		if (num2 != prevClosestIndex)
		{
			prevClosestIndex = num2;
			AudioManager.Instance.Play(base.transform.position, clickSound, 1f, 1f);
			hapticObject.Restart();
			if (!grabbable.CurrInteractableHand.HapticsController.ContainHaptic(hapticObject))
			{
				grabbable.CurrInteractableHand.HapticsController.AddNewHaptic(hapticObject);
			}
		}
		return num2;
	}

	private float GetCurrentAngle()
	{
		if (axis == DialAxis.X)
		{
			return base.transform.localEulerAngles.x;
		}
		if (axis == DialAxis.Y)
		{
			return base.transform.localEulerAngles.y;
		}
		if (axis == DialAxis.Z)
		{
			return base.transform.localEulerAngles.z;
		}
		return 0f;
	}

	private float GetAngleDelta(float from, float to)
	{
		float num;
		for (num = to - from; num > 180f; num -= 360f)
		{
		}
		for (; num <= -180f; num += 360f)
		{
		}
		return Mathf.Abs(num);
	}

	private void Released(GrabbableItem grabbable)
	{
		Select(GetClosestSelectionIndex());
		if (hapticObject.IsRunning && grabbable.CurrInteractableHand != null)
		{
			grabbable.CurrInteractableHand.HapticsController.RemoveHaptic(hapticObject);
		}
	}

	private void Update()
	{
		if (grabbable.IsCurrInHand && snapStrength > 0f)
		{
			float num = optionAngles[GetClosestSelectionIndex()];
			float angleDelta = GetAngleDelta(GetCurrentAngle(), num);
			if (angleDelta <= snapWithinAngle)
			{
				Quaternion b = CalculateLocalRotationFromAngle(num);
				base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, b, Time.deltaTime * snapStrength);
			}
		}
	}
}
