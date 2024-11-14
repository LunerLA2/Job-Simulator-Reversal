using System;
using OwlchemyVR;
using UnityEngine;

public class DeliveryBoxController : MonoBehaviour
{
	private const string OpenboxClip = "DeliveryBoxOpening";

	[SerializeField]
	private WorldItem boxWorldItem;

	[SerializeField]
	private SegmentedCoverController segmentController;

	[SerializeField]
	private Animator anim;

	[SerializeField]
	private GameObjectPrefabSpawner[] spawner;

	private void OnEnable()
	{
		SegmentedCoverController segmentedCoverController = segmentController;
		segmentedCoverController.OnAllSegmentsUncovered = (Action)Delegate.Combine(segmentedCoverController.OnAllSegmentsUncovered, new Action(BoxCompletelyOpened));
		SegmentedCoverController segmentedCoverController2 = segmentController;
		segmentedCoverController2.OnSegmentsPartiallyUncovered = (Action<float>)Delegate.Combine(segmentedCoverController2.OnSegmentsPartiallyUncovered, new Action<float>(BoxPartiallyOpened));
	}

	private void OnDisable()
	{
		SegmentedCoverController segmentedCoverController = segmentController;
		segmentedCoverController.OnAllSegmentsUncovered = (Action)Delegate.Remove(segmentedCoverController.OnAllSegmentsUncovered, new Action(BoxCompletelyOpened));
		SegmentedCoverController segmentedCoverController2 = segmentController;
		segmentedCoverController2.OnSegmentsPartiallyUncovered = (Action<float>)Delegate.Remove(segmentedCoverController2.OnSegmentsPartiallyUncovered, new Action<float>(BoxPartiallyOpened));
	}

	private void BoxPartiallyOpened(float perc)
	{
		GameEventsManager.Instance.ItemActionOccurredWithAmount(boxWorldItem.Data, "USED_PARTIALLY", perc);
	}

	private void BoxCompletelyOpened()
	{
		anim.Play("DeliveryBoxOpening");
		for (int i = 0; i < spawner.Length; i++)
		{
			spawner[i].SpawnPrefab();
		}
		GameEventsManager.Instance.ItemActionOccurred(boxWorldItem.Data, "OPENED");
	}
}
