using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class ChainController : MonoBehaviour
{
	[SerializeField]
	private GameObject[] chainSegments;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private Animation scrubAnimation;

	[SerializeField]
	private float animationMovementRatio = 0.3f;

	[SerializeField]
	private float maxScrubSpeed = 1f;

	private float targetScrubAmount;

	private bool isScrubbing;

	private float currentScrubAmount;

	private LinkedList<GameObject> chainSegmentsLinkedList;

	private float chainSegmentHeight;

	private float previousHeight;

	private float currentHeight;

	private float distanceFromLastMove;

	private float percentDoorOpen;

	private bool isActivated;

	private void Start()
	{
		chainSegmentsLinkedList = new LinkedList<GameObject>(chainSegments);
		chainSegmentHeight = chainSegmentsLinkedList.First.Value.GetComponentInChildren<Renderer>().bounds.size.y;
		previousHeight = base.transform.position.y;
	}

	private void Update()
	{
		currentHeight = base.transform.position.y;
		distanceFromLastMove += currentHeight - previousHeight;
		if (Mathf.Abs(distanceFromLastMove) >= chainSegmentHeight - 0.1f)
		{
			if (distanceFromLastMove < 0f)
			{
				chainSegmentsLinkedList.First.Value.transform.position = chainSegmentsLinkedList.Last.Value.transform.position + Vector3.up * (chainSegmentHeight - 0.02f);
				GameObject value = chainSegmentsLinkedList.First.Value;
				chainSegmentsLinkedList.RemoveFirst();
				chainSegmentsLinkedList.AddLast(value);
			}
			else if (distanceFromLastMove > 0f)
			{
				chainSegmentsLinkedList.Last.Value.transform.position = chainSegmentsLinkedList.First.Value.transform.position - Vector3.up * (chainSegmentHeight - 0.02f);
				GameObject value2 = chainSegmentsLinkedList.Last.Value;
				chainSegmentsLinkedList.RemoveLast();
				chainSegmentsLinkedList.AddFirst(value2);
			}
			distanceFromLastMove = 0f;
		}
		if (scrubAnimation != null)
		{
			percentDoorOpen = Mathf.Clamp(percentDoorOpen + (previousHeight - currentHeight) * animationMovementRatio, 0f, 1f);
			ScrubUpdate(percentDoorOpen);
			if (myWorldItem != null)
			{
				if (!isActivated && percentDoorOpen >= 0.9f)
				{
					GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
					isActivated = true;
				}
				else if (isActivated && percentDoorOpen < 0.9f)
				{
					GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
					isActivated = false;
				}
			}
		}
		previousHeight = currentHeight;
	}

	private void ScrubUpdate(float perc)
	{
		targetScrubAmount = perc;
		bool flag = isScrubbing;
		if (currentScrubAmount < targetScrubAmount)
		{
			currentScrubAmount = Mathf.Min(currentScrubAmount + Time.deltaTime * maxScrubSpeed, targetScrubAmount);
			isScrubbing = true;
		}
		else if (currentScrubAmount > targetScrubAmount)
		{
			currentScrubAmount = Mathf.Max(currentScrubAmount - Time.deltaTime * maxScrubSpeed, targetScrubAmount);
			isScrubbing = true;
		}
		else
		{
			isScrubbing = false;
		}
		if (flag)
		{
			float time = currentScrubAmount * scrubAnimation.clip.length;
			scrubAnimation[scrubAnimation.clip.name].enabled = true;
			scrubAnimation[scrubAnimation.clip.name].weight = 1f;
			scrubAnimation[scrubAnimation.clip.name].time = time;
			scrubAnimation.Sample();
			scrubAnimation[scrubAnimation.clip.name].enabled = false;
		}
	}
}
