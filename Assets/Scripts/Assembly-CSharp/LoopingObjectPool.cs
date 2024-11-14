using System.Collections.Generic;
using UnityEngine;

public class LoopingObjectPool<T> where T : Object
{
	private List<T> objects;

	private int nextIndex = -1;

	public LoopingObjectPool(T prefab, int capacity, Transform parent)
	{
		objects = new List<T>();
		for (int i = 0; i < capacity; i++)
		{
			objects.Add(Object.Instantiate(prefab));
			Transform itemTransform = GetItemTransform(objects[i]);
			GameObject gameObject = itemTransform.gameObject;
			string name = gameObject.name;
			gameObject.name = name.Remove(name.Length - "(Clone)".Length);
			if (itemTransform != null)
			{
				itemTransform.SetParent(parent, false);
			}
			gameObject.SetActive(false);
		}
	}

	public T Fetch(Vector3 pos, Quaternion rot)
	{
		T val = (T)null;
		if (nextIndex < objects.Count - 1)
		{
			val = objects[++nextIndex];
		}
		else
		{
			nextIndex = 0;
			val = objects[nextIndex];
		}
		Transform itemTransform = GetItemTransform(val);
		itemTransform.position = pos;
		itemTransform.rotation = rot;
		itemTransform.gameObject.SetActive(true);
		return val;
	}

	private Transform GetItemTransform(T item)
	{
		if (item is GameObject)
		{
			return (item as GameObject).transform;
		}
		if (item is Component)
		{
			return (item as Component).transform;
		}
		return null;
	}
}
