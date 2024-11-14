using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGoTween
{
	public int id;

	protected bool _didInit;

	protected bool _didBegin;

	protected bool _fireIterationStart;

	protected bool _fireIterationEnd;

	protected float _elapsedTime;

	protected float _totalElapsedTime;

	protected bool _isLoopingBackOnPingPong;

	protected bool _didIterateLastFrame;

	protected bool _didIterateThisFrame;

	protected int _deltaIterations;

	protected int _completedIterations;

	protected Action<AbstractGoTween> _onInit;

	protected Action<AbstractGoTween> _onBegin;

	protected Action<AbstractGoTween> _onIterationStart;

	protected Action<AbstractGoTween> _onUpdate;

	protected Action<AbstractGoTween> _onIterationEnd;

	protected Action<AbstractGoTween> _onComplete;

	public GoTweenState state { get; protected set; }

	public float duration { get; protected set; }

	public float totalDuration { get; protected set; }

	public float timeScale { get; set; }

	public GoUpdateType updateType { get; protected set; }

	public GoLoopType loopType { get; protected set; }

	public int iterations { get; protected set; }

	public bool autoRemoveOnComplete { get; set; }

	public bool isReversed { get; protected set; }

	public bool allowEvents { get; set; }

	public float totalElapsedTime
	{
		get
		{
			return _totalElapsedTime;
		}
	}

	public bool isLoopingBackOnPingPong
	{
		get
		{
			return _isLoopingBackOnPingPong;
		}
	}

	public int completedIterations
	{
		get
		{
			return _completedIterations;
		}
	}

	public void setOnInitHandler(Action<AbstractGoTween> onInit)
	{
		_onInit = onInit;
	}

	public void setOnBeginHandler(Action<AbstractGoTween> onBegin)
	{
		_onBegin = onBegin;
	}

	public void setonIterationStartHandler(Action<AbstractGoTween> onIterationStart)
	{
		_onIterationStart = onIterationStart;
	}

	public void setOnUpdateHandler(Action<AbstractGoTween> onUpdate)
	{
		_onUpdate = onUpdate;
	}

	public void setonIterationEndHandler(Action<AbstractGoTween> onIterationEnd)
	{
		_onIterationEnd = onIterationEnd;
	}

	public void setOnCompleteHandler(Action<AbstractGoTween> onComplete)
	{
		_onComplete = onComplete;
	}

	protected virtual void onInit()
	{
		if (allowEvents)
		{
			if (_onInit != null)
			{
				_onInit(this);
			}
			_didInit = true;
		}
	}

	protected virtual void onBegin()
	{
		if (allowEvents && (!isReversed || _totalElapsedTime == totalDuration) && (isReversed || _totalElapsedTime == 0f))
		{
			if (_onBegin != null)
			{
				_onBegin(this);
			}
			_didBegin = true;
		}
	}

	protected virtual void onIterationStart()
	{
		if (allowEvents && _onIterationStart != null)
		{
			_onIterationStart(this);
		}
	}

	protected virtual void onUpdate()
	{
		if (allowEvents && _onUpdate != null)
		{
			_onUpdate(this);
		}
	}

	protected virtual void onIterationEnd()
	{
		if (allowEvents && _onIterationEnd != null)
		{
			_onIterationEnd(this);
		}
	}

	protected virtual void onComplete()
	{
		if (allowEvents && _onComplete != null)
		{
			_onComplete(this);
		}
	}

	public virtual bool update(float deltaTime)
	{
		if (isReversed)
		{
			_totalElapsedTime -= deltaTime;
		}
		else
		{
			_totalElapsedTime += deltaTime;
		}
		_totalElapsedTime = Mathf.Clamp(_totalElapsedTime, 0f, totalDuration);
		_didIterateLastFrame = _didIterateThisFrame || (!isReversed && _totalElapsedTime == 0f) || (isReversed && _totalElapsedTime == totalDuration);
		if (isReversed)
		{
			_deltaIterations = Mathf.CeilToInt(_totalElapsedTime / duration) - _completedIterations;
		}
		else
		{
			_deltaIterations = Mathf.FloorToInt(_totalElapsedTime / duration) - _completedIterations;
		}
		_didIterateThisFrame = !_didIterateLastFrame && ((float)_deltaIterations != 0f || _totalElapsedTime % duration == 0f);
		_completedIterations += _deltaIterations;
		if (_didIterateLastFrame)
		{
			_elapsedTime = ((!isReversed) ? 0f : duration);
		}
		else if (_didIterateThisFrame)
		{
			_elapsedTime = ((!isReversed) ? duration : 0f);
		}
		else
		{
			_elapsedTime = _totalElapsedTime % duration;
			if (_elapsedTime == 0f && ((isReversed && _totalElapsedTime == totalDuration) || (!isReversed && _totalElapsedTime > 0f)))
			{
				_elapsedTime = duration;
			}
		}
		_isLoopingBackOnPingPong = false;
		if (loopType == GoLoopType.PingPong)
		{
			if (isReversed)
			{
				_isLoopingBackOnPingPong = _completedIterations % 2 == 0;
				if (_elapsedTime == 0f)
				{
					_isLoopingBackOnPingPong = !_isLoopingBackOnPingPong;
				}
			}
			else
			{
				_isLoopingBackOnPingPong = _completedIterations % 2 != 0;
				if (_elapsedTime == duration)
				{
					_isLoopingBackOnPingPong = !_isLoopingBackOnPingPong;
				}
			}
		}
		_fireIterationStart = _didIterateThisFrame || (!isReversed && _elapsedTime == duration) || (isReversed && _elapsedTime == 0f);
		_fireIterationEnd = _didIterateThisFrame;
		if ((!isReversed && iterations >= 0 && _completedIterations >= iterations) || (isReversed && _totalElapsedTime <= 0f))
		{
			state = GoTweenState.Complete;
		}
		if (state == GoTweenState.Complete)
		{
			_fireIterationStart = false;
			_didIterateThisFrame = false;
			return true;
		}
		return false;
	}

	public abstract bool isValid();

	public abstract bool removeTweenProperty(AbstractTweenProperty property);

	public abstract bool containsTweenProperty(AbstractTweenProperty property);

	public abstract List<AbstractTweenProperty> allTweenProperties();

	public virtual void destroy()
	{
		state = GoTweenState.Destroyed;
	}

	public virtual void pause()
	{
		state = GoTweenState.Paused;
	}

	public virtual void play()
	{
		state = GoTweenState.Running;
	}

	public void playForward()
	{
		if (isReversed)
		{
			reverse();
		}
		play();
	}

	public void playBackwards()
	{
		if (!isReversed)
		{
			reverse();
		}
		play();
	}

	protected virtual void reset(bool skipDelay = true)
	{
		goTo((!isReversed) ? 0f : totalDuration, skipDelay);
		_fireIterationStart = true;
	}

	public virtual void rewind(bool skipDelay = true)
	{
		reset(skipDelay);
		pause();
	}

	public void restart(bool skipDelay = true)
	{
		reset(skipDelay);
		play();
	}

	public virtual void reverse()
	{
		isReversed = !isReversed;
		_completedIterations = ((!isReversed) ? Mathf.FloorToInt(_totalElapsedTime / duration) : Mathf.CeilToInt(_totalElapsedTime / duration));
		if ((isReversed && _totalElapsedTime == totalDuration) || (!isReversed && _totalElapsedTime == 0f))
		{
			_didBegin = false;
			_fireIterationStart = true;
		}
	}

	public virtual void complete()
	{
		if (iterations >= 0)
		{
			goTo((!isReversed) ? totalDuration : 0f);
		}
	}

	public abstract void goTo(float time, bool skipDelay = true);

	public void goToAndPlay(float time, bool skipDelay = true)
	{
		goTo(time, skipDelay);
		play();
	}

	public IEnumerator waitForCompletion()
	{
		while (state != GoTweenState.Complete && state != GoTweenState.Destroyed)
		{
			yield return null;
		}
	}
}
