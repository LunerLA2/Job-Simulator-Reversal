using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCallsPerFrame : MonoBehaviour
{
	private static Dictionary<string, int> labelCounts = new Dictionary<string, int>();

	private Coroutine clearAndLogCoroutine;

	public static void Log(string label)
	{
		if (!labelCounts.ContainsKey(label))
		{
			labelCounts[label] = 0;
		}
		Dictionary<string, int> dictionary;
		Dictionary<string, int> dictionary2 = (dictionary = labelCounts);
		string key;
		string key2 = (key = label);
		int num = dictionary[key];
		dictionary2[key2] = num + 1;
	}

	private void OnEnable()
	{
		clearAndLogCoroutine = StartCoroutine(ClearAndLogRoutine());
	}

	private void OnDisable()
	{
		StopCoroutine(clearAndLogCoroutine);
	}

	private IEnumerator ClearAndLogRoutine()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			if (labelCounts.Count == 0)
			{
				continue;
			}
			Debug.Log("----- Frame: " + Time.frameCount + " -----");
			foreach (KeyValuePair<string, int> keyValue in labelCounts)
			{
				Debug.Log(keyValue.Key + ": " + keyValue.Value + "\n");
			}
			labelCounts.Clear();
		}
	}
}
