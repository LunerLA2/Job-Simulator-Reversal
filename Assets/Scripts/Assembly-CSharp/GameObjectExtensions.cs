using UnityEngine;

public static class GameObjectExtensions
{
	public static void RemoveCloneFromName(this GameObject gameObject)
	{
		string text = "(Clone)";
		if (gameObject.name.EndsWith(text))
		{
			gameObject.name = gameObject.name.Remove(gameObject.name.Length - text.Length);
		}
	}
}
