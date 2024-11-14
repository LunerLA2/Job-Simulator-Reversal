using System;
using UnityEngine;

public class EnergyDisplayShelfController : MonoBehaviour
{
	[SerializeField]
	private Transform parentWhenOnShelf;

	[SerializeField]
	private Animation doorsAnimation;

	[SerializeField]
	private AnimationClip doorsOpenClip;

	[SerializeField]
	private AnimationClip doorsCloseClip;

	[SerializeField]
	private int toolIndexOfDisplay;

	private KitchenToolStasher lastStasher;

	public void ReceiveDisplay(Transform displayTransform, KitchenToolStasher stasherToWatch)
	{
		displayTransform.SetParent(parentWhenOnShelf);
		displayTransform.SetToDefaultPosRotScale();
		displayTransform.gameObject.SetActive(true);
		doorsAnimation.Play(doorsOpenClip.name);
		if (lastStasher != null)
		{
			KitchenToolStasher kitchenToolStasher = lastStasher;
			kitchenToolStasher.OnWillSwitchToToolIndex = (Action<int>)Delegate.Remove(kitchenToolStasher.OnWillSwitchToToolIndex, new Action<int>(StasherWillSwitchModes));
		}
		stasherToWatch.OnWillSwitchToToolIndex = (Action<int>)Delegate.Combine(stasherToWatch.OnWillSwitchToToolIndex, new Action<int>(StasherWillSwitchModes));
		lastStasher = stasherToWatch;
	}

	private void StasherWillSwitchModes(int newModeIndex)
	{
		if (newModeIndex == toolIndexOfDisplay)
		{
			if (lastStasher != null)
			{
				KitchenToolStasher kitchenToolStasher = lastStasher;
				kitchenToolStasher.OnWillSwitchToToolIndex = (Action<int>)Delegate.Remove(kitchenToolStasher.OnWillSwitchToToolIndex, new Action<int>(StasherWillSwitchModes));
			}
			doorsAnimation.Play(doorsCloseClip.name);
		}
	}
}
