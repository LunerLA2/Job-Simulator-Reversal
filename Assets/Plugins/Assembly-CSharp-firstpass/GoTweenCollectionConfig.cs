using System;

public class GoTweenCollectionConfig
{
	public int id;

	public int iterations = 1;

	public GoLoopType loopType = Go.defaultLoopType;

	public GoUpdateType propertyUpdateType = Go.defaultUpdateType;

	public Action<AbstractGoTween> onInitHandler;

	public Action<AbstractGoTween> onBeginHandler;

	public Action<AbstractGoTween> onIterationStartHandler;

	public Action<AbstractGoTween> onUpdateHandler;

	public Action<AbstractGoTween> onIterationEndHandler;

	public Action<AbstractGoTween> onCompleteHandler;

	public GoTweenCollectionConfig setIterations(int iterations)
	{
		this.iterations = iterations;
		return this;
	}

	public GoTweenCollectionConfig setIterations(int iterations, GoLoopType loopType)
	{
		this.iterations = iterations;
		this.loopType = loopType;
		return this;
	}

	public GoTweenCollectionConfig setUpdateType(GoUpdateType setUpdateType)
	{
		propertyUpdateType = setUpdateType;
		return this;
	}

	public GoTweenCollectionConfig onInit(Action<AbstractGoTween> onInit)
	{
		onInitHandler = onInit;
		return this;
	}

	public GoTweenCollectionConfig onBegin(Action<AbstractGoTween> onBegin)
	{
		onBeginHandler = onBegin;
		return this;
	}

	public GoTweenCollectionConfig onIterationStart(Action<AbstractGoTween> onIterationStart)
	{
		onIterationStartHandler = onIterationStart;
		return this;
	}

	public GoTweenCollectionConfig onUpdate(Action<AbstractGoTween> onUpdate)
	{
		onUpdateHandler = onUpdate;
		return this;
	}

	public GoTweenCollectionConfig onIterationEnd(Action<AbstractGoTween> onIterationEnd)
	{
		onIterationEndHandler = onIterationEnd;
		return this;
	}

	public GoTweenCollectionConfig onComplete(Action<AbstractGoTween> onComplete)
	{
		onCompleteHandler = onComplete;
		return this;
	}

	public GoTweenCollectionConfig setId(int id)
	{
		this.id = id;
		return this;
	}
}
