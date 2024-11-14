using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class QuickLookupManager : MonoBehaviour
{
	private Dictionary<int, ColliderGrabbableItemPointer> cachedColliderPointerReferences;

	private static QuickLookupManager _instance;

	public static QuickLookupManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(QuickLookupManager)) as QuickLookupManager;
				if (_instance == null)
				{
					_instance = new GameObject("_QuickLookupManager").AddComponent<QuickLookupManager>();
				}
			}
			return _instance;
		}
	}

	public static QuickLookupManager _instanceNoCreate
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (_instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Setup();
	}

	private void OnLevelWasLoaded(int level)
	{
		Debug.Log("Clear quickLookupManager");
		if (cachedColliderPointerReferences != null)
		{
			cachedColliderPointerReferences.Clear();
		}
		else
		{
			cachedColliderPointerReferences = new Dictionary<int, ColliderGrabbableItemPointer>();
		}
	}

	private void Setup()
	{
		cachedColliderPointerReferences = new Dictionary<int, ColliderGrabbableItemPointer>();
	}

	public bool HasPointerKeyForColliderInstanceID(int id)
	{
		return cachedColliderPointerReferences.ContainsKey(id);
	}

	public ColliderGrabbableItemPointer GetPointerFromColliderInstanceID(int id)
	{
		return cachedColliderPointerReferences[id];
	}

	public void SetPointerForColliderInstanceID(ColliderGrabbableItemPointer pointer, int colliderID)
	{
		if (cachedColliderPointerReferences.ContainsKey(colliderID))
		{
			Debug.LogError("Somehow a collider id (" + colliderID + ") was assigned a pointer twice");
			if (cachedColliderPointerReferences[colliderID] != pointer)
			{
				Debug.LogError("..AND it was to a different pointer!");
			}
		}
		cachedColliderPointerReferences[colliderID] = pointer;
	}
}
