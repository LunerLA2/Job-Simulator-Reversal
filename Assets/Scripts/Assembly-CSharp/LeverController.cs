using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(GrabbableHinge))]
public class LeverController : MonoBehaviour
{
	private enum LeverActivationDirection
	{
		Positive = 0,
		Negative = 1
	}

	[SerializeField]
	private float activationAngle;

	[SerializeField]
	private float resetAngle;

	[SerializeField]
	private LeverActivationDirection activationDirection;

	[SerializeField]
	private AudioClip onActionClip;

	[SerializeField]
	private AudioClip onResetClip;

	[SerializeField]
	private UnityEvent OnAction;

	[SerializeField]
	private UnityEvent OnReset;

	private GrabbableHinge hinge;

	private bool isActivated;

	public GrabbableHinge Hinge
	{
		get
		{
			return hinge;
		}
	}

	private void Awake()
	{
		hinge = GetComponent<GrabbableHinge>();
	}

	private void Update()
	{
		if (isActivated)
		{
			if ((activationDirection == LeverActivationDirection.Positive && hinge.Angle <= resetAngle) || (activationDirection == LeverActivationDirection.Negative && hinge.Angle >= resetAngle))
			{
				isActivated = false;
				OnReset.Invoke();
				if ((bool)onResetClip)
				{
					AudioManager.Instance.Play(base.transform.position, onResetClip, 1f, 1f);
				}
			}
		}
		else if ((activationDirection == LeverActivationDirection.Positive && hinge.Angle >= activationAngle) || (activationDirection == LeverActivationDirection.Negative && hinge.Angle <= activationAngle))
		{
			isActivated = true;
			OnAction.Invoke();
			if ((bool)onActionClip)
			{
				AudioManager.Instance.Play(base.transform.position, onActionClip, 1f, 1f);
			}
		}
	}
}
