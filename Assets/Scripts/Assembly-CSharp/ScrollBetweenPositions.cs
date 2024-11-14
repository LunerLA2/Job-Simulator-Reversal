using System;
using System.Collections;
using UnityEngine;

public class ScrollBetweenPositions : MonoBehaviour
{
	[SerializeField]
	private MechanicalPushButtonController downButton;

	[SerializeField]
	private MechanicalPushButtonController upButton;

	[SerializeField]
	private Transform transformToControl;

	[SerializeField]
	private Vector3 localPosAtMin = Vector3.zero;

	[SerializeField]
	private Vector3 localPosAtMax = Vector3.zero;

	[SerializeField]
	private Vector3 scaleAtMin = Vector3.one;

	[SerializeField]
	private Vector3 scaleAtMax = Vector3.one;

	[SerializeField]
	private float startAtPercentage = 1f;

	[SerializeField]
	private float scrollSpeed = 1f;

	private float currentPercentage;

	private bool isPushingDown;

	private bool isPushingUp;

	private void Awake()
	{
		SetPercentage(startAtPercentage);
	}

	private void OnEnable()
	{
		MechanicalPushButtonController mechanicalPushButtonController = downButton;
		mechanicalPushButtonController.OnBeganMoving = (Action<MechanicalPushButtonController>)Delegate.Combine(mechanicalPushButtonController.OnBeganMoving, new Action<MechanicalPushButtonController>(BeganPushingDown));
		MechanicalPushButtonController mechanicalPushButtonController2 = downButton;
		mechanicalPushButtonController2.OnStoppedMoving = (Action<MechanicalPushButtonController>)Delegate.Combine(mechanicalPushButtonController2.OnStoppedMoving, new Action<MechanicalPushButtonController>(StoppedPushingDown));
		MechanicalPushButtonController mechanicalPushButtonController3 = upButton;
		mechanicalPushButtonController3.OnBeganMoving = (Action<MechanicalPushButtonController>)Delegate.Combine(mechanicalPushButtonController3.OnBeganMoving, new Action<MechanicalPushButtonController>(BeganPushingUp));
		MechanicalPushButtonController mechanicalPushButtonController4 = upButton;
		mechanicalPushButtonController4.OnStoppedMoving = (Action<MechanicalPushButtonController>)Delegate.Combine(mechanicalPushButtonController4.OnStoppedMoving, new Action<MechanicalPushButtonController>(StoppedPushingUp));
	}

	private void OnDisable()
	{
		MechanicalPushButtonController mechanicalPushButtonController = downButton;
		mechanicalPushButtonController.OnBeganMoving = (Action<MechanicalPushButtonController>)Delegate.Remove(mechanicalPushButtonController.OnBeganMoving, new Action<MechanicalPushButtonController>(BeganPushingDown));
		MechanicalPushButtonController mechanicalPushButtonController2 = downButton;
		mechanicalPushButtonController2.OnStoppedMoving = (Action<MechanicalPushButtonController>)Delegate.Remove(mechanicalPushButtonController2.OnStoppedMoving, new Action<MechanicalPushButtonController>(StoppedPushingDown));
		MechanicalPushButtonController mechanicalPushButtonController3 = upButton;
		mechanicalPushButtonController3.OnBeganMoving = (Action<MechanicalPushButtonController>)Delegate.Remove(mechanicalPushButtonController3.OnBeganMoving, new Action<MechanicalPushButtonController>(BeganPushingUp));
		MechanicalPushButtonController mechanicalPushButtonController4 = upButton;
		mechanicalPushButtonController4.OnStoppedMoving = (Action<MechanicalPushButtonController>)Delegate.Remove(mechanicalPushButtonController4.OnStoppedMoving, new Action<MechanicalPushButtonController>(StoppedPushingUp));
	}

	private void BeganPushingUp(MechanicalPushButtonController button)
	{
		if (!isPushingUp)
		{
			isPushingUp = true;
			StartCoroutine(PushingUpCoroutine());
		}
	}

	private void StoppedPushingUp(MechanicalPushButtonController button)
	{
		isPushingUp = false;
	}

	private void BeganPushingDown(MechanicalPushButtonController button)
	{
		if (!isPushingDown)
		{
			isPushingDown = true;
			StartCoroutine(PushingUpCoroutine());
		}
	}

	private void StoppedPushingDown(MechanicalPushButtonController button)
	{
		isPushingDown = false;
	}

	private IEnumerator PushingUpCoroutine()
	{
		while (isPushingUp)
		{
			currentPercentage = Mathf.Min(1f, currentPercentage + Time.deltaTime * scrollSpeed * upButton.NormalizedPushAmount);
			SetPercentage(currentPercentage);
			yield return null;
		}
	}

	private IEnumerator PushingDownCoroutine()
	{
		while (isPushingDown)
		{
			currentPercentage = Mathf.Max(0f, currentPercentage - Time.deltaTime * scrollSpeed * downButton.NormalizedPushAmount);
			SetPercentage(currentPercentage);
			yield return null;
		}
	}

	private void SetPercentage(float perc)
	{
		transformToControl.localPosition = Vector3.Lerp(localPosAtMin, localPosAtMax, perc);
		transformToControl.localScale = Vector3.Lerp(scaleAtMin, scaleAtMax, perc);
		currentPercentage = perc;
	}
}
