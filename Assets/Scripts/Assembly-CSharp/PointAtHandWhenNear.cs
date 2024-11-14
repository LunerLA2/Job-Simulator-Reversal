using System;
using OwlchemyVR;
using UnityEngine;

public class PointAtHandWhenNear : MonoBehaviour
{
	public enum Axis
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	[SerializeField]
	private PlayerPartDetector playerPartDetector;

	[SerializeField]
	private Transform transformToPoint;

	[SerializeField]
	private Axis onlyAllowRotationOn = Axis.Y;

	[SerializeField]
	private GrabbableItem grabbableItem;

	[SerializeField]
	private bool onlyPointIfNotGrabbed = true;

	private InteractionHandController currentHand;

	private void OnEnable()
	{
		PlayerPartDetector obj = playerPartDetector;
		obj.OnHandEntered = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Combine(obj.OnHandEntered, new Action<PlayerPartDetector, InteractionHandController>(HandEntered));
		PlayerPartDetector obj2 = playerPartDetector;
		obj2.OnHandExited = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Combine(obj2.OnHandExited, new Action<PlayerPartDetector, InteractionHandController>(HandExited));
	}

	private void OnDisable()
	{
		PlayerPartDetector obj = playerPartDetector;
		obj.OnHandEntered = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Remove(obj.OnHandEntered, new Action<PlayerPartDetector, InteractionHandController>(HandEntered));
		PlayerPartDetector obj2 = playerPartDetector;
		obj2.OnHandExited = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Remove(obj2.OnHandExited, new Action<PlayerPartDetector, InteractionHandController>(HandExited));
	}

	private void Update()
	{
		if (currentHand != null)
		{
			bool flag = true;
			if (onlyPointIfNotGrabbed && grabbableItem.IsCurrInHand)
			{
				flag = false;
			}
			if (flag)
			{
				transformToPoint.LookAt(currentHand.transform.position);
				transformToPoint.localEulerAngles = new Vector3((onlyAllowRotationOn != 0) ? 0f : transformToPoint.localEulerAngles.x, (onlyAllowRotationOn != Axis.Y) ? 0f : transformToPoint.localEulerAngles.y, (onlyAllowRotationOn != Axis.Z) ? 0f : transformToPoint.localEulerAngles.z);
			}
		}
	}

	private void HandEntered(PlayerPartDetector ppd, InteractionHandController hand)
	{
		currentHand = hand;
	}

	private void HandExited(PlayerPartDetector ppd, InteractionHandController hand)
	{
		if (currentHand == hand)
		{
			currentHand = null;
		}
	}
}
