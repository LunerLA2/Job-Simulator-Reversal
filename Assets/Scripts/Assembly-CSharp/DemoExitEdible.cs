using System;
using UnityEngine;

[RequireComponent(typeof(EdibleItem))]
public class DemoExitEdible : MonoBehaviour
{
	private const string STORE_OPEN_PARAM = "=purchase";

	[SerializeField]
	private EdibleItem edibleItem;

	[SerializeField]
	private bool openStoreOnExit;

	private static bool isDemoExiting;

	private void OnEnable()
	{
		EdibleItem obj = edibleItem;
		obj.OnFullyConsumed = (Action<EdibleItem>)Delegate.Combine(obj.OnFullyConsumed, new Action<EdibleItem>(OnFullConsumption));
	}

	private void OnDisable()
	{
		EdibleItem obj = edibleItem;
		obj.OnFullyConsumed = (Action<EdibleItem>)Delegate.Remove(obj.OnFullyConsumed, new Action<EdibleItem>(OnFullConsumption));
	}

	private void OnFullConsumption(EdibleItem obj)
	{
		if (!isDemoExiting)
		{
			isDemoExiting = true;
			if (openStoreOnExit)
			{
				LevelLoader.Instance.ReturnToLauncher("=purchase");
			}
			else
			{
				LevelLoader.Instance.ReturnToLauncher(string.Empty);
			}
		}
	}
}
