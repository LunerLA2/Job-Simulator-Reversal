using System;
using System.Collections.Generic;
using UnityEngine;

public class MoCappableObject : MonoBehaviour
{
	public enum MoCapTypes
	{
		Head = 0,
		LeftHand = 1,
		RightHand = 2
	}

	[SerializeField]
	private Transform transformToRecord;

	[SerializeField]
	private MoCapTypes moCapType;

	[SerializeField]
	private PageData pageToRecordDuring;

	private bool isRecording;

	private List<Vector3> recordedPositions;

	private List<Quaternion> recordedRotations;

	public MoCapTypes MoCapType
	{
		get
		{
			return moCapType;
		}
	}

	public Vector3 GetPositionForFrame(int frame)
	{
		return recordedPositions[frame % recordedPositions.Count];
	}

	public Quaternion GetRotationForFrame(int frame)
	{
		return recordedRotations[frame % recordedRotations.Count];
	}

	private void OnEnable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageShown = (Action<PageStatusController>)Delegate.Combine(instance.OnPageShown, new Action<PageStatusController>(PageShown));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnPageComplete = (Action<PageStatusController>)Delegate.Combine(instance2.OnPageComplete, new Action<PageStatusController>(PageCompleted));
	}

	private void OnDisable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageShown = (Action<PageStatusController>)Delegate.Remove(instance.OnPageShown, new Action<PageStatusController>(PageShown));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnPageComplete = (Action<PageStatusController>)Delegate.Remove(instance2.OnPageComplete, new Action<PageStatusController>(PageCompleted));
	}

	private void PageShown(PageStatusController page)
	{
		if (page.Data == pageToRecordDuring)
		{
			StartRecording();
		}
	}

	private void PageCompleted(PageStatusController page)
	{
		if (page.Data == pageToRecordDuring)
		{
			StopRecording();
		}
	}

	private void Update()
	{
		if (isRecording)
		{
			RecordKeyframe();
		}
	}

	private void StartRecording()
	{
		recordedPositions = new List<Vector3>();
		recordedRotations = new List<Quaternion>();
		isRecording = true;
	}

	private void RecordKeyframe()
	{
		recordedPositions.Add(transformToRecord.position);
		recordedRotations.Add(transformToRecord.rotation);
	}

	private void StopRecording()
	{
		isRecording = false;
		if (GameDeveloperDataStorage.Instance != null && !GameDeveloperDataStorage.Instance.mocapObjects.Contains(this))
		{
			GameDeveloperDataStorage.Instance.mocapObjects.Add(this);
		}
	}
}
