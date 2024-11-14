using System.Collections.Generic;
using System.Linq;
using OwlchemyVR;
using UnityEngine;

public class VehicleBatteryController : MonoBehaviour
{
	[SerializeField]
	private BatteryGlowyBitData batteryGlowyBitData;

	[SerializeField]
	private AttachablePoint attachablePoint;

	[SerializeField]
	private UVScroll uvScroll;

	[SerializeField]
	private Renderer glowyBitRenderer;

	[SerializeField]
	private UVScroll secondaryUVScroll;

	[SerializeField]
	private Renderer secondaryGlowyBitRenderer;

	[SerializeField]
	private float glowChangeDuration = 1f;

	private GoTween tween;

	private GoTween secondaryTween;

	private void OnEnable()
	{
		if (attachablePoint != null)
		{
			attachablePoint.OnObjectWasAttached += OnObjectWasAttached;
			attachablePoint.OnObjectWasDetached += OnObjectWasDetached;
		}
		if (!(attachablePoint == null))
		{
			if (attachablePoint.IsOccupied)
			{
				OnObjectWasAttached(attachablePoint, attachablePoint.GetAttachedObject(0));
			}
			else
			{
				OnObjectWasDetached(attachablePoint, null);
			}
		}
	}

	private void OnDisable()
	{
		if (attachablePoint != null)
		{
			attachablePoint.OnObjectWasAttached -= OnObjectWasAttached;
			attachablePoint.OnObjectWasDetached -= OnObjectWasDetached;
		}
	}

	private void OnObjectWasDetached(AttachablePoint attachablePoint, AttachableObject attachableObject)
	{
		if (tween != null && tween.state == GoTweenState.Running)
		{
			tween.pause();
		}
		tween = Go.to(glowyBitRenderer, glowChangeDuration, new GoTweenConfig().materialColor(batteryGlowyBitData.OffColor).onComplete(delegate
		{
			uvScroll.y = false;
		}));
		if (secondaryUVScroll != null)
		{
			if (secondaryTween != null && secondaryTween.state == GoTweenState.Running)
			{
				secondaryTween.pause();
			}
			secondaryTween = Go.to(secondaryGlowyBitRenderer, glowChangeDuration, new GoTweenConfig().materialColor(batteryGlowyBitData.OffColor).onComplete(delegate
			{
				secondaryUVScroll.y = false;
			}));
		}
	}

	private void OnObjectWasAttached(AttachablePoint attachablePoint, AttachableObject attachableObject)
	{
		if (tween != null && tween.state == GoTweenState.Running)
		{
			tween.pause();
		}
		WorldItemData worldItem = attachableObject.PickupableItem.InteractableItem.WorldItemData;
		IEnumerable<BatteryGlowyBit> source = from batteryBits in batteryGlowyBitData.BatteryGlowyBits
			from wid in batteryBits.BatteryItemData
			where wid == worldItem
			select batteryBits;
		BatteryGlowyBit batteryGlowyBit = source.FirstOrDefault();
		if (batteryGlowyBit == null)
		{
			return;
		}
		tween = Go.to(glowyBitRenderer, glowChangeDuration, new GoTweenConfig().materialColor(batteryGlowyBit.GlowColor).onComplete(delegate
		{
			uvScroll.y = true;
		}));
		if (secondaryUVScroll != null)
		{
			if (secondaryTween != null && secondaryTween.state == GoTweenState.Running)
			{
				secondaryTween.pause();
			}
			secondaryTween = Go.to(secondaryGlowyBitRenderer, glowChangeDuration, new GoTweenConfig().materialColor(batteryGlowyBit.GlowColor).onComplete(delegate
			{
				secondaryUVScroll.y = true;
			}));
		}
	}
}
