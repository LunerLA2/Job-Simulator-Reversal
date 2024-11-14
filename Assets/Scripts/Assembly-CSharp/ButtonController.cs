using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(GrabbableSlider))]
public class ButtonController : MonoBehaviour
{
	private enum ButtonActivationDirection
	{
		Positive = 0,
		Negative = 1
	}

	[SerializeField]
	private float activationOffset;

	[SerializeField]
	private float resetOffset;

	[SerializeField]
	private ButtonActivationDirection activationDirection;

	[SerializeField]
	private AudioClip onActionClip;

	[SerializeField]
	private AudioClip onResetClip;

	[SerializeField]
	private UnityEvent OnAction;

	[SerializeField]
	private UnityEvent OnReset;

	public UnityAction OnButtonDown;

	private GrabbableSlider slider;

	private bool isActivated;

	public GrabbableSlider Slider
	{
		get
		{
			return slider;
		}
	}

	private void Awake()
	{
		slider = GetComponent<GrabbableSlider>();
	}

	private void Update()
	{
		if (isActivated)
		{
			if ((activationDirection == ButtonActivationDirection.Positive && slider.Offset <= resetOffset) || (activationDirection == ButtonActivationDirection.Negative && slider.Offset >= resetOffset))
			{
				isActivated = false;
				OnReset.Invoke();
				if ((bool)onResetClip)
				{
					AudioManager.Instance.Play(base.transform.position, onResetClip, 1f, 1f);
				}
			}
		}
		else if ((activationDirection == ButtonActivationDirection.Positive && slider.Offset >= activationOffset) || (activationDirection == ButtonActivationDirection.Negative && slider.Offset <= activationOffset))
		{
			isActivated = true;
			OnAction.Invoke();
			if (OnButtonDown != null)
			{
				OnButtonDown();
			}
			if ((bool)onActionClip)
			{
				AudioManager.Instance.Play(base.transform.position, onActionClip, 1f, 1f);
			}
		}
	}
}
