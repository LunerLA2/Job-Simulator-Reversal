using System;
using UnityEngine;

public class BotLocomotion : MonoBehaviour
{
	private BotPath currentPath;

	private bool useStraightLines;

	private AbstractGoTween _tween;

	private int wayPointIndex;

	private bool inTransit;

	public Action<BotLocomotion> OnLocomotionCompleted;

	public BotPath CurrentPath
	{
		get
		{
			return currentPath;
		}
	}

	public bool IsInTransit
	{
		get
		{
			return inTransit;
		}
	}

	public void DoPath(BotPath path, BrainEffect.PathTypes pathingType, BrainEffect.PathLookTypes pathLookType)
	{
		currentPath = path;
		switch (pathingType)
		{
		case BrainEffect.PathTypes.Forwards:
			StartTransit(currentPath.GetPathAsGoSpline(useStraightLines, false), currentPath.timeToCompletePath, currentPath.easingType, pathLookType);
			break;
		case BrainEffect.PathTypes.ForwardsSlideIn:
			StartTransit(currentPath.GetPathAsGoSpline(useStraightLines, base.transform.position, false), currentPath.timeToCompletePath, currentPath.easingType, pathLookType);
			break;
		case BrainEffect.PathTypes.Backwards:
			StartTransit(currentPath.GetPathAsGoSpline(useStraightLines, true), currentPath.timeToCompletePath, currentPath.easeTypeIfUsedBackwards, pathLookType);
			break;
		case BrainEffect.PathTypes.BackwardsSlideIn:
			StartTransit(currentPath.GetPathAsGoSpline(useStraightLines, base.transform.position, true), currentPath.timeToCompletePath, currentPath.easeTypeIfUsedBackwards, pathLookType);
			break;
		}
	}

	private void StartTransit(GoSpline path, float timeToCompletePath, GoEaseType easeType, BrainEffect.PathLookTypes pathLookType)
	{
		if (inTransit)
		{
			Go.killAllTweensWithTarget(base.transform);
		}
		inTransit = true;
		GoLookAtType lookAtType = GoLookAtType.NextPathNode;
		if (pathLookType == BrainEffect.PathLookTypes.ContinueLookAtLogic)
		{
			lookAtType = GoLookAtType.None;
		}
		_tween = Go.to(base.transform, timeToCompletePath, new GoTweenConfig().positionPath(path, false, lookAtType).setEaseType(easeType));
		_tween.setOnCompleteHandler(TweenComplete);
	}

	private void TweenComplete(AbstractGoTween at)
	{
		if (OnLocomotionCompleted != null)
		{
			OnLocomotionCompleted(this);
		}
		inTransit = false;
	}

	public void CancelLocomotion()
	{
		inTransit = false;
		if (_tween != null)
		{
			_tween.destroy();
			_tween = null;
		}
	}

	public void pauseTween()
	{
		inTransit = false;
		if (_tween != null)
		{
			_tween.pause();
		}
	}

	public void resumeTween()
	{
		inTransit = true;
		if (_tween != null)
		{
			_tween.play();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(base.transform.position, base.transform.forward * 1f);
	}
}
