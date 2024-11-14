using System;
using UnityEngine;

public class EngineCheckerController : MonoBehaviour
{
	private const string animTriggerGood = "Good";

	private const string animTriggerBad = "Bad";

	private const string animTriggerIdle = "Idle";

	[SerializeField]
	private Animator anim;

	[SerializeField]
	private AttachableObject checkerConnection;

	private void Awake()
	{
		if (checkerConnection.CurrentlyAttachedTo == null)
		{
			Debug.LogWarning("Engine Checker Controller has to find checkerConnection's attach point, should be in the engine");
		}
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		AttachableObject attachableObject = checkerConnection;
		attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(CheckerConnectionAttached));
		AttachableObject attachableObject2 = checkerConnection;
		attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(CheckerConnectionDetached));
	}

	private void OnDisable()
	{
		AttachableObject attachableObject = checkerConnection;
		attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(CheckerConnectionAttached));
		AttachableObject attachableObject2 = checkerConnection;
		attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(CheckerConnectionDetached));
	}

	private void CheckerConnectionAttached(AttachableObject attachable, AttachablePoint t)
	{
		Debug.Log("CheckConnection Attached");
		int num = UnityEngine.Random.Range(0, 2);
		if (num == 1)
		{
			EngineCheckerGood();
		}
		else
		{
			EngineCheckerBad();
		}
	}

	private void CheckerConnectionDetached(AttachableObject attachable, AttachablePoint t)
	{
		Debug.Log("CheckConnection Detached");
		EngineCheckerIdle();
	}

	public void EngineCheckerGood()
	{
		ResetTriggers();
		anim.SetTrigger("Good");
	}

	public void EngineCheckerBad()
	{
		ResetTriggers();
		anim.SetTrigger("Bad");
	}

	public void EngineCheckerIdle()
	{
		ResetTriggers();
		anim.SetTrigger("Idle");
	}

	private void ResetTriggers()
	{
		anim.ResetTrigger("Good");
		anim.ResetTrigger("Bad");
		anim.ResetTrigger("Idle");
	}
}
