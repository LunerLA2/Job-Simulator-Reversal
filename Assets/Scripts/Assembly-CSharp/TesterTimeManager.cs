using UnityEngine;

public class TesterTimeManager : MonoBehaviour
{
	private void Start()
	{
		Test();
	}

	private void Test()
	{
		TimeManager.Invoke(Complete, 1f);
		TimeManager.Invoke(CompleteInt, 2, 0.5f);
		TimeManager.Invoke(CompleteInt, 3, 0.75f);
		TimeManager.Invoke(CompleteInt, 1, 0.25f);
		TimeManager.Invoke(Begin, 0.1f);
		Debug.Log("Test");
	}

	private void Begin()
	{
		Debug.Log("Begin");
		TimeManager.CancelInvoke(CompleteInt, 2);
	}

	private void Complete()
	{
		Debug.Log("Complete");
		Debug.Log("Next Index Pool:" + TimeManager.TestingNextPoolIndex);
		Test();
	}

	private void CompleteInt(int value)
	{
		Debug.Log("Complete Value:" + value);
	}
}
