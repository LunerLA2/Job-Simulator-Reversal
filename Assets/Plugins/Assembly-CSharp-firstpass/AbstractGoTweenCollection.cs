using System.Collections.Generic;
using UnityEngine;

public class AbstractGoTweenCollection : AbstractGoTween
{
	protected class TweenFlowItem
	{
		public float startTime;

		public float duration;

		public AbstractGoTween tween;

		public float endTime
		{
			get
			{
				return startTime + duration;
			}
		}

		public TweenFlowItem(float startTime, AbstractGoTween tween)
		{
			this.tween = tween;
			this.startTime = startTime;
			duration = tween.totalDuration;
		}

		public TweenFlowItem(float startTime, float duration)
		{
			this.duration = duration;
			this.startTime = startTime;
		}
	}

	protected List<TweenFlowItem> _tweenFlows = new List<TweenFlowItem>();

	public AbstractGoTweenCollection(GoTweenCollectionConfig config)
	{
		base.allowEvents = true;
		_didInit = false;
		_didBegin = false;
		_fireIterationStart = true;
		id = config.id;
		base.loopType = config.loopType;
		base.iterations = config.iterations;
		base.updateType = config.propertyUpdateType;
		base.timeScale = 1f;
		base.state = GoTweenState.Paused;
		_onInit = config.onInitHandler;
		_onBegin = config.onBeginHandler;
		_onIterationStart = config.onIterationStartHandler;
		_onUpdate = config.onUpdateHandler;
		_onIterationEnd = config.onIterationEndHandler;
		_onComplete = config.onCompleteHandler;
		Go.addTween(this);
	}

	public List<GoTween> tweensWithTarget(object target)
	{
		List<GoTween> list = new List<GoTween>();
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween == null)
			{
				continue;
			}
			GoTween goTween = tweenFlow.tween as GoTween;
			if (goTween != null && goTween.target == target)
			{
				list.Add(goTween);
			}
			if (goTween != null)
			{
				continue;
			}
			AbstractGoTweenCollection abstractGoTweenCollection = tweenFlow.tween as AbstractGoTweenCollection;
			if (abstractGoTweenCollection != null)
			{
				List<GoTween> list2 = abstractGoTweenCollection.tweensWithTarget(target);
				if (list2.Count > 0)
				{
					list.AddRange(list2);
				}
			}
		}
		return list;
	}

	public override bool removeTweenProperty(AbstractTweenProperty property)
	{
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween == null || !tweenFlow.tween.removeTweenProperty(property))
			{
				continue;
			}
			return true;
		}
		return false;
	}

	public override bool containsTweenProperty(AbstractTweenProperty property)
	{
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween == null || !tweenFlow.tween.containsTweenProperty(property))
			{
				continue;
			}
			return true;
		}
		return false;
	}

	public override List<AbstractTweenProperty> allTweenProperties()
	{
		List<AbstractTweenProperty> list = new List<AbstractTweenProperty>();
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween != null)
			{
				list.AddRange(tweenFlow.tween.allTweenProperties());
			}
		}
		return list;
	}

	public override bool isValid()
	{
		return true;
	}

	public override void play()
	{
		base.play();
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween != null)
			{
				tweenFlow.tween.play();
			}
		}
	}

	public override void pause()
	{
		base.pause();
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween != null)
			{
				tweenFlow.tween.pause();
			}
		}
	}

	public override bool update(float deltaTime)
	{
		if (!_didInit)
		{
			onInit();
		}
		if (!_didBegin)
		{
			onBegin();
		}
		if (_fireIterationStart)
		{
			onIterationStart();
		}
		base.update(deltaTime);
		float num = ((!_isLoopingBackOnPingPong) ? _elapsedTime : (base.duration - _elapsedTime));
		TweenFlowItem tweenFlowItem = null;
		if (_didIterateLastFrame && base.loopType == GoLoopType.RestartFromBeginning)
		{
			if (base.isReversed || _isLoopingBackOnPingPong)
			{
				for (int i = 0; i < _tweenFlows.Count; i++)
				{
					tweenFlowItem = _tweenFlows[i];
					if (tweenFlowItem.tween != null)
					{
						bool flag = tweenFlowItem.tween.allowEvents;
						tweenFlowItem.tween.allowEvents = false;
						tweenFlowItem.tween.restart();
						tweenFlowItem.tween.allowEvents = flag;
					}
				}
			}
			else
			{
				for (int num2 = _tweenFlows.Count - 1; num2 >= 0; num2--)
				{
					tweenFlowItem = _tweenFlows[num2];
					if (tweenFlowItem.tween != null)
					{
						bool flag2 = tweenFlowItem.tween.allowEvents;
						tweenFlowItem.tween.allowEvents = false;
						tweenFlowItem.tween.restart();
						tweenFlowItem.tween.allowEvents = flag2;
					}
				}
			}
		}
		else if ((base.isReversed && !_isLoopingBackOnPingPong) || (!base.isReversed && _isLoopingBackOnPingPong))
		{
			for (int num3 = _tweenFlows.Count - 1; num3 >= 0; num3--)
			{
				tweenFlowItem = _tweenFlows[num3];
				if (tweenFlowItem.tween != null)
				{
					if (_didIterateLastFrame && base.state != GoTweenState.Complete)
					{
						if (!tweenFlowItem.tween.isReversed)
						{
							tweenFlowItem.tween.reverse();
						}
						tweenFlowItem.tween.play();
					}
					if (tweenFlowItem.tween.state == GoTweenState.Running && tweenFlowItem.endTime >= num)
					{
						float deltaTime2 = Mathf.Abs(num - tweenFlowItem.startTime - tweenFlowItem.tween.totalElapsedTime);
						tweenFlowItem.tween.update(deltaTime2);
					}
				}
			}
		}
		else
		{
			for (int j = 0; j < _tweenFlows.Count; j++)
			{
				tweenFlowItem = _tweenFlows[j];
				if (tweenFlowItem.tween == null)
				{
					continue;
				}
				if (_didIterateLastFrame && base.state != GoTweenState.Complete)
				{
					if (tweenFlowItem.tween.isReversed)
					{
						tweenFlowItem.tween.reverse();
					}
					tweenFlowItem.tween.play();
				}
				if (tweenFlowItem.tween.state == GoTweenState.Running && tweenFlowItem.startTime <= num)
				{
					float deltaTime3 = num - tweenFlowItem.startTime - tweenFlowItem.tween.totalElapsedTime;
					tweenFlowItem.tween.update(deltaTime3);
				}
			}
		}
		onUpdate();
		if (_fireIterationEnd)
		{
			onIterationEnd();
		}
		if (base.state == GoTweenState.Complete)
		{
			onComplete();
			return true;
		}
		return false;
	}

	public override void reverse()
	{
		base.reverse();
		float num = ((!_isLoopingBackOnPingPong) ? _elapsedTime : (base.duration - _elapsedTime));
		foreach (TweenFlowItem tweenFlow in _tweenFlows)
		{
			if (tweenFlow.tween == null)
			{
				continue;
			}
			if (base.isReversed != tweenFlow.tween.isReversed)
			{
				tweenFlow.tween.reverse();
			}
			tweenFlow.tween.pause();
			if (base.isReversed || _isLoopingBackOnPingPong)
			{
				if (tweenFlow.startTime <= num)
				{
					tweenFlow.tween.play();
				}
			}
			else if (tweenFlow.endTime >= num)
			{
				tweenFlow.tween.play();
			}
		}
	}

	public override void goTo(float time, bool skipDelay = true)
	{
		time = Mathf.Clamp(time, 0f, base.totalDuration);
		if (time == _totalElapsedTime)
		{
			return;
		}
		if ((base.isReversed && time == base.totalDuration) || (!base.isReversed && time == 0f))
		{
			_didBegin = false;
			_fireIterationStart = true;
		}
		else
		{
			_didBegin = true;
			_fireIterationStart = false;
		}
		_didIterateThisFrame = false;
		_totalElapsedTime = time;
		_completedIterations = ((!base.isReversed) ? Mathf.FloorToInt(_totalElapsedTime / base.duration) : Mathf.CeilToInt(_totalElapsedTime / base.duration));
		base.update(0f);
		float num = ((!_isLoopingBackOnPingPong) ? _elapsedTime : (base.duration - _elapsedTime));
		TweenFlowItem tweenFlowItem = null;
		if (base.isReversed || _isLoopingBackOnPingPong)
		{
			for (int i = 0; i < _tweenFlows.Count; i++)
			{
				tweenFlowItem = _tweenFlows[i];
				if (tweenFlowItem != null)
				{
					if (tweenFlowItem.endTime >= num)
					{
						break;
					}
					changeTimeForFlowItem(tweenFlowItem, num);
				}
			}
			for (int num2 = _tweenFlows.Count - 1; num2 >= 0; num2--)
			{
				tweenFlowItem = _tweenFlows[num2];
				if (tweenFlowItem != null)
				{
					if (tweenFlowItem.endTime < num)
					{
						break;
					}
					changeTimeForFlowItem(tweenFlowItem, num);
				}
			}
			return;
		}
		for (int num3 = _tweenFlows.Count - 1; num3 >= 0; num3--)
		{
			tweenFlowItem = _tweenFlows[num3];
			if (tweenFlowItem != null)
			{
				if (tweenFlowItem.startTime <= num)
				{
					break;
				}
				changeTimeForFlowItem(tweenFlowItem, num);
			}
		}
		for (int j = 0; j < _tweenFlows.Count; j++)
		{
			tweenFlowItem = _tweenFlows[j];
			if (tweenFlowItem != null)
			{
				if (tweenFlowItem.startTime > num)
				{
					break;
				}
				changeTimeForFlowItem(tweenFlowItem, num);
			}
		}
	}

	private void changeTimeForFlowItem(TweenFlowItem flowItem, float time)
	{
		if (flowItem != null && flowItem.tween != null)
		{
			if (flowItem.tween.isReversed != (base.isReversed || _isLoopingBackOnPingPong))
			{
				flowItem.tween.reverse();
			}
			float time2 = Mathf.Clamp(time - flowItem.startTime, 0f, flowItem.endTime);
			if (flowItem.startTime <= time && flowItem.endTime >= time)
			{
				flowItem.tween.goToAndPlay(time2);
				return;
			}
			flowItem.tween.goTo(time2);
			flowItem.tween.pause();
		}
	}
}
