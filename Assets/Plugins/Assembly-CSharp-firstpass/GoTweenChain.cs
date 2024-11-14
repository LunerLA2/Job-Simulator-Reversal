using UnityEngine;

public class GoTweenChain : AbstractGoTweenCollection
{
	public GoTweenChain()
		: this(new GoTweenCollectionConfig())
	{
	}

	public GoTweenChain(GoTweenCollectionConfig config)
		: base(config)
	{
	}

	private void append(TweenFlowItem item)
	{
		if (item.tween != null && !item.tween.isValid())
		{
			return;
		}
		if (float.IsInfinity(item.duration))
		{
			Debug.LogError("adding a Tween with infinite iterations to a TweenChain is not permitted");
			return;
		}
		if (item.tween != null)
		{
			if (item.tween.isReversed != base.isReversed)
			{
				Debug.LogError("adding a Tween that doesn't match the isReversed property of the TweenChain is not permitted.");
				return;
			}
			Go.removeTween(item.tween);
			item.tween.play();
		}
		_tweenFlows.Add(item);
		base.duration += item.duration;
		if (base.iterations < 0)
		{
			base.totalDuration = float.PositiveInfinity;
		}
		else
		{
			base.totalDuration = base.duration * (float)base.iterations;
		}
	}

	private void prepend(TweenFlowItem item)
	{
		if (item.tween != null && !item.tween.isValid())
		{
			return;
		}
		if (float.IsInfinity(item.duration))
		{
			Debug.LogError("adding a Tween with infinite iterations to a TweenChain is not permitted");
			return;
		}
		if (item.tween != null)
		{
			if (item.tween.isReversed != base.isReversed)
			{
				Debug.LogError("adding a Tween that doesn't match the isReversed property of the TweenChain is not permitted.");
				return;
			}
			Go.removeTween(item.tween);
			item.tween.play();
		}
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			tweenFlow.startTime += item.duration;
		}
		_tweenFlows.Insert(0, item);
		base.duration += item.duration;
		if (base.iterations < 0)
		{
			base.totalDuration = float.PositiveInfinity;
		}
		else
		{
			base.totalDuration = base.duration * (float)base.iterations;
		}
	}

	public GoTweenChain append(AbstractGoTween tween)
	{
		TweenFlowItem item = new TweenFlowItem(base.duration, tween);
		append(item);
		return this;
	}

	public GoTweenChain appendDelay(float delay)
	{
		TweenFlowItem item = new TweenFlowItem(base.duration, delay);
		append(item);
		return this;
	}

	public GoTweenChain prepend(AbstractGoTween tween)
	{
		TweenFlowItem item = new TweenFlowItem(0f, tween);
		prepend(item);
		return this;
	}

	public GoTweenChain prependDelay(float delay)
	{
		TweenFlowItem item = new TweenFlowItem(0f, delay);
		prepend(item);
		return this;
	}
}
