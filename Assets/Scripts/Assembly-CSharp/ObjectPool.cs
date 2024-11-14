using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Object
{
	private List<T> pool;

	private int nextIndex = -1;

	private bool setActiveOnPooling;

	private T prefabBackup;

	private bool autoInitOnPoolEmpty;

	private Transform autoPoolParent;

	private Vector3 nextAutoInitPosition = Vector3.zero;

	private Vector3 initPositionOffset;

	public bool SetActiveOnPooling
	{
		get
		{
			return setActiveOnPooling;
		}
		set
		{
			setActiveOnPooling = value;
		}
	}

	public ObjectPool(T prefab, int capacity, bool setActiveOnPooling, bool autoInitOnPoolEmpty, Transform parent, Vector3 initPositionOffset)
	{
		prefabBackup = prefab;
		this.autoInitOnPoolEmpty = autoInitOnPoolEmpty;
		this.initPositionOffset = initPositionOffset;
		pool = new List<T>(capacity);
		this.setActiveOnPooling = setActiveOnPooling;
		autoPoolParent = parent;
		if (parent == null)
		{
			nextAutoInitPosition = new Vector3(-500f, 0f, 0f);
		}
		for (int i = 0; i < capacity; i++)
		{
			pool.Add(Object.Instantiate(prefab));
			Transform itemTransform = GetItemTransform(pool[i]);
			GameObject gameObject = itemTransform.gameObject;
			string name = gameObject.name;
			gameObject.name = name.Remove(name.Length - "(Clone)".Length);
			if (itemTransform != null)
			{
				itemTransform.SetParent(parent, false);
				itemTransform.localPosition = nextAutoInitPosition;
			}
			if (setActiveOnPooling)
			{
				gameObject.SetActive(false);
			}
			nextAutoInitPosition += initPositionOffset;
		}
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

	private GameObject GetItemGameObject(T item)
	{
		if (item is GameObject)
		{
			return item as GameObject;
		}
		if (item is Component)
		{
			return (item as Component).gameObject;
		}
		return null;
	}

	public T Fetch()
	{
		T val = (T)null;
		if (nextIndex < pool.Count - 1)
		{
			val = pool[++nextIndex];
			Transform itemTransform = GetItemTransform(val);
			if (setActiveOnPooling)
			{
				itemTransform.gameObject.SetActive(true);
			}
		}
		else
		{
			Debug.LogWarning("Insufficent Pooled Objects:" + prefabBackup.name);
			if (autoInitOnPoolEmpty)
			{
				val = Object.Instantiate(prefabBackup);
				val.name = val.name.Remove(val.name.Length - "(Clone)".Length);
				Transform itemTransform2 = GetItemTransform(val);
				if (autoPoolParent != null)
				{
					itemTransform2.SetParent(autoPoolParent, false);
				}
				itemTransform2.localPosition = nextAutoInitPosition;
				nextAutoInitPosition += initPositionOffset;
				pool.Add(val);
				nextIndex++;
				if (setActiveOnPooling)
				{
					itemTransform2.gameObject.SetActive(true);
				}
			}
		}
		return val;
	}

	public T Fetch(Vector3 pos, Quaternion rot)
	{
		return Fetch(pos, rot, false, Vector3.one);
	}

	public T Fetch(Vector3 pos, Quaternion rot, bool setLocalScale, Vector3 localScale)
	{
		T val = (T)null;
		if (nextIndex < pool.Count - 1)
		{
			val = pool[++nextIndex];
			Transform itemTransform = GetItemTransform(val);
			itemTransform.position = pos;
			itemTransform.rotation = rot;
			if (setLocalScale)
			{
				itemTransform.localScale = localScale;
			}
			if (setActiveOnPooling)
			{
				itemTransform.gameObject.SetActive(true);
			}
		}
		else
		{
			Debug.LogWarning("Insufficent Pooled Objects:" + prefabBackup.name);
			if (autoInitOnPoolEmpty)
			{
				val = (T)Object.Instantiate(prefabBackup, pos, rot);
				val.name = val.name.Remove(val.name.Length - "(Clone)".Length);
				Transform itemTransform2 = GetItemTransform(val);
				if (autoPoolParent != null)
				{
					itemTransform2.SetParent(autoPoolParent);
					itemTransform2.position = pos;
					itemTransform2.rotation = rot;
					if (setLocalScale)
					{
						itemTransform2.localScale = localScale;
					}
					else
					{
						itemTransform2.localScale = GetItemTransform(prefabBackup).localScale;
					}
				}
				pool.Add(val);
				nextIndex++;
				if (setActiveOnPooling)
				{
					itemTransform2.gameObject.SetActive(true);
				}
			}
		}
		return val;
	}

	public T ManualGenerateNew(Vector3 pos, Quaternion rot)
	{
		return (T)Object.Instantiate(prefabBackup, pos, rot);
	}

	public void Release(T item)
	{
		pool[nextIndex--] = item;
		if (setActiveOnPooling)
		{
			GetItemGameObject(item).SetActive(false);
		}
	}
}
