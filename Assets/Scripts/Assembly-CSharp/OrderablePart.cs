using System;
using UnityEngine;

[Serializable]
public class OrderablePart
{
	[SerializeField]
	private GameObject prefab;

	[SerializeField]
	private Sprite sprite;

	public GameObject Prefab
	{
		get
		{
			return prefab;
		}
	}

	public Sprite Sprite
	{
		get
		{
			return sprite;
		}
	}

	public string ID
	{
		get
		{
			return prefab.name + "_" + sprite.name;
		}
	}
}
