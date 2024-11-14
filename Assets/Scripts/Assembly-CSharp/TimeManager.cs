using System.Collections.Generic;
using UnityEngine;

public static class TimeManager
{
	public delegate void DelegateTimeComplete();

	public delegate void DelegateTimeComplete_Int(int value);

	private const int INIT_POOL_SIZE = 50;

	private static float timeElapsed = 0f;

	private static float realTimeElapsed = 0f;

	private static float realDeltaTime = 0f;

	private static List<DelegateTimeManagerContainer> sortedTimeEvent = new List<DelegateTimeManagerContainer>();

	private static List<DelegateTimeManagerContainer> sortedRealTimeEvent = new List<DelegateTimeManagerContainer>();

	private static bool doesTimeEventExist = false;

	private static bool doesRealTimeEventExist = false;

	private static DelegateTimeManagerContainer tempDelegateTimeManagerContainer;

	private static List<DelegateTimeManagerContainer> pool = new List<DelegateTimeManagerContainer>();

	private static int nextPoolIndex = -1;

	private static DelegateTimeManagerContainer instantInvokeDelegateTimeManagerContainer = new DelegateTimeManagerContainer();

	public static int TestingNextPoolIndex
	{
		get
		{
			return nextPoolIndex;
		}
	}

	public static float RealDeltaTime
	{
		get
		{
			return realDeltaTime;
		}
	}

	public static void Init()
	{
		Clear();
		while (pool.Count < 50)
		{
			pool.Add(new DelegateTimeManagerContainer());
		}
	}

	public static void ManualUpdate()
	{
		if (Time.timeScale > 0f)
		{
			realDeltaTime = Time.unscaledDeltaTime;
			timeElapsed += Time.deltaTime;
		}
		else
		{
			realDeltaTime = 0f;
		}
		realTimeElapsed += realDeltaTime;
		CheckForTimeEvents();
	}

	private static void CheckForTimeEvents()
	{
		if (doesTimeEventExist && Time.timeScale > 0f)
		{
			while (sortedTimeEvent.Count > 0 && timeElapsed > sortedTimeEvent[0].DelayUntilSeconds)
			{
				tempDelegateTimeManagerContainer = sortedTimeEvent[0];
				sortedTimeEvent.RemoveAt(0);
				doesTimeEventExist = sortedTimeEvent.Count > 0;
				InvokeDelegateTimeManagerContainer(tempDelegateTimeManagerContainer);
				ReleaseDelegateTimeContainer(tempDelegateTimeManagerContainer);
				tempDelegateTimeManagerContainer = null;
			}
		}
		if (doesRealTimeEventExist)
		{
			while (sortedRealTimeEvent.Count > 0 && realTimeElapsed > sortedRealTimeEvent[0].DelayUntilSeconds)
			{
				tempDelegateTimeManagerContainer = sortedRealTimeEvent[0];
				sortedRealTimeEvent.RemoveAt(0);
				doesRealTimeEventExist = sortedRealTimeEvent.Count > 0;
				InvokeDelegateTimeManagerContainer(tempDelegateTimeManagerContainer);
				ReleaseDelegateTimeContainer(tempDelegateTimeManagerContainer);
				tempDelegateTimeManagerContainer = null;
			}
		}
	}

	public static void Invoke(DelegateTimeComplete delegateTimeComplete, float delaySeconds)
	{
		if (delaySeconds > 0f)
		{
			InsertTimeSortedEvent(FetchDelegateTimeContainer().Init(delegateTimeComplete, delaySeconds + timeElapsed));
		}
		else
		{
			instantInvokeDelegateTimeManagerContainer.Init(delegateTimeComplete, 0f).Invoke();
		}
	}

	public static void CancelInvoke(DelegateTimeComplete delegateTimeComplete)
	{
		bool flag = false;
		for (int i = 0; i < sortedTimeEvent.Count; i++)
		{
			if (sortedTimeEvent[i].IsTheSame(delegateTimeComplete))
			{
				ReleaseDelegateTimeContainer(sortedTimeEvent[i]);
				sortedTimeEvent.RemoveAt(i);
				i--;
				flag = true;
			}
		}
		doesTimeEventExist = sortedTimeEvent.Count > 0;
		if (!flag)
		{
			Debug.LogWarning("No invoke to cancel" + delegateTimeComplete.Method.Name);
		}
	}

	public static void InvokeRealtime(DelegateTimeComplete delegateTimeComplete, float delaySeconds)
	{
		if (delaySeconds > 0f)
		{
			InsertRealTimeSortedEvent(FetchDelegateTimeContainer().Init(delegateTimeComplete, delaySeconds + realTimeElapsed));
		}
		else
		{
			instantInvokeDelegateTimeManagerContainer.Init(delegateTimeComplete, 0f).Invoke();
		}
	}

	public static void CancelInvokeRealtime(DelegateTimeComplete delegateTimeComplete)
	{
		bool flag = false;
		for (int i = 0; i < sortedRealTimeEvent.Count; i++)
		{
			if (sortedRealTimeEvent[i].IsTheSame(delegateTimeComplete))
			{
				ReleaseDelegateTimeContainer(sortedRealTimeEvent[i]);
				sortedRealTimeEvent.RemoveAt(i);
				i--;
				flag = true;
			}
		}
		doesRealTimeEventExist = sortedRealTimeEvent.Count > 0;
		if (!flag)
		{
			Debug.LogWarning("No invoke to cancel:" + delegateTimeComplete.Method.Name);
		}
	}

	public static void Invoke(DelegateTimeComplete_Int delegateTimeCompleteInt, int value, float delaySeconds)
	{
		if (delaySeconds > 0f)
		{
			InsertTimeSortedEvent(FetchDelegateTimeContainer().Init(delegateTimeCompleteInt, value, delaySeconds + timeElapsed));
		}
		else
		{
			instantInvokeDelegateTimeManagerContainer.Init(delegateTimeCompleteInt, value, 0f).Invoke();
		}
	}

	public static void CancelInvoke(DelegateTimeComplete_Int delegateTimeCompleteInt, int value)
	{
		bool flag = false;
		for (int i = 0; i < sortedTimeEvent.Count; i++)
		{
			if (sortedTimeEvent[i].IsTheSame(delegateTimeCompleteInt, value))
			{
				ReleaseDelegateTimeContainer(sortedTimeEvent[i]);
				sortedTimeEvent.RemoveAt(i);
				i--;
				flag = true;
			}
		}
		doesTimeEventExist = sortedTimeEvent.Count > 0;
		if (!flag)
		{
			Debug.LogWarning("No invoke to cancel" + delegateTimeCompleteInt.Method.Name + ":" + value);
		}
	}

	private static void InsertTimeSortedEvent(DelegateTimeManagerContainer delegateTimeCompleteContainer)
	{
		doesTimeEventExist = true;
		float delayUntilSeconds = delegateTimeCompleteContainer.DelayUntilSeconds;
		for (int i = 0; i < sortedTimeEvent.Count; i++)
		{
			if (sortedTimeEvent[i].DelayUntilSeconds > delayUntilSeconds)
			{
				sortedTimeEvent.Insert(i, delegateTimeCompleteContainer);
				return;
			}
		}
		sortedTimeEvent.Add(delegateTimeCompleteContainer);
	}

	private static void InsertRealTimeSortedEvent(DelegateTimeManagerContainer delegateTimeCompleteContainer)
	{
		doesRealTimeEventExist = true;
		float delayUntilSeconds = delegateTimeCompleteContainer.DelayUntilSeconds;
		for (int i = 0; i < sortedRealTimeEvent.Count; i++)
		{
			if (sortedRealTimeEvent[i].DelayUntilSeconds > delayUntilSeconds)
			{
				sortedRealTimeEvent.Insert(i, delegateTimeCompleteContainer);
				return;
			}
		}
		sortedRealTimeEvent.Add(delegateTimeCompleteContainer);
	}

	private static void InvokeDelegateTimeManagerContainer(DelegateTimeManagerContainer delegateTimeManagerContainer)
	{
		if (delegateTimeManagerContainer != null)
		{
			delegateTimeManagerContainer.Invoke();
		}
	}

	private static DelegateTimeManagerContainer FetchDelegateTimeContainer()
	{
		if (nextPoolIndex < pool.Count - 1)
		{
			return pool[++nextPoolIndex];
		}
		Debug.LogWarning("Insufficent Pooled TimeManager Objects");
		DelegateTimeManagerContainer delegateTimeManagerContainer = new DelegateTimeManagerContainer();
		pool.Add(delegateTimeManagerContainer);
		nextPoolIndex++;
		return delegateTimeManagerContainer;
	}

	private static void ReleaseDelegateTimeContainer(DelegateTimeManagerContainer delegateTimeManagerContainer)
	{
		delegateTimeManagerContainer.Clear();
		pool[nextPoolIndex--] = delegateTimeManagerContainer;
	}

	public static void Clear()
	{
		timeElapsed = 0f;
		realTimeElapsed = 0f;
		realDeltaTime = 0f;
		for (int i = 0; i < sortedTimeEvent.Count; i++)
		{
			ReleaseDelegateTimeContainer(sortedTimeEvent[i]);
		}
		sortedTimeEvent.Clear();
		for (int j = 0; j < sortedRealTimeEvent.Count; j++)
		{
			ReleaseDelegateTimeContainer(sortedRealTimeEvent[j]);
		}
		sortedRealTimeEvent.Clear();
		doesTimeEventExist = false;
		doesRealTimeEventExist = false;
		tempDelegateTimeManagerContainer = null;
	}
}
